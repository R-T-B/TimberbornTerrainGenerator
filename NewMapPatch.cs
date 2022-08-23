using HarmonyLib;
using System;
using System.IO;
using System.Threading;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapSystem;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static FastNoiseLite;
using static TimberbornTerrainGenerator.MapFileTools;
using static TimberbornTerrainGenerator.Utilities;
using static TimberbornTerrainGenerator.Statics;
using static TimberbornTerrainGenerator.EntityManager;
namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(MapEditorSceneLoader), nameof(MapEditorSceneLoader.StartNewMap))]
    class NewMapPatch
    {
        public static System.Random rand = new System.Random();
        public static FastNoiseLite noise = new FastNoiseLite();
        public static int MapSizeX;
        public static int MapSizeY;
        public static int seed;
        //BEGIN EXTERNAL LOADABLE INI SETTINGS
        public static int TerrainMinHeight = 10;
        public static int TerrainMaxHeight = 20;
        public static FastNoiseLite.NoiseType TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
        public static float TerrainAmplitude = 2.5f;
        public static int TerrainFrequencyMult = 10;
        public static float TerrainSlopeLevel = 0.0025f;
        public static int RiverNodes = 2;
        public static float RiverSourceStrength = 1.5f;
        public static float RiverWindiness = 0.4125f;
        public static int RiverWidth = 4;
        public static float RiverElevation = -0.7125f;
        public static int RiverMapWeight = 5;
        public static int MaxMineCount = 4;
        public static int MinMineCount = 0;
        public static int RuinCount = 500;
        public static int PineTreeCount = 3600;
        public static int BirchTreeCount = 1200;
        public static int ChestnutTreeCount = 1500;
        public static int MapleTreeCount = 900;
        public static int BlueberryBushCount = 500;
        public static int DandelionBushCount = 250;
        public static int SlopeCount = 128;
        public static bool[,] RiverMapper = new bool[32,32];
        //END EXTERNAL LOADABLE INI SETTINGS
        public static bool Prefix(Vector2Int mapSize, MapEditorSceneLoader __instance)
        {
            //Try load .ini settings
            try
            {
                IniParser iniParser = new IniParser(PluginPath + "/settings.ini");
                TerrainMinHeight = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainMinHeight"));
                TerrainMaxHeight = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainMaxHeight"));
                if (iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainNoiseType").ToLower().Equals("perlin"))
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
                }
                else if (iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainNoiseType").ToLower().Equals("opensimplex2"))
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.OpenSimplex2;
                }
                else if (iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainNoiseType").ToLower().Equals("cellular"))
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.Cellular;
                }
                else
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
                    Debug.LogWarning("Unable to determine noise settings, proceeding with Perlin Noise.");
                }
                TerrainAmplitude = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainAmplitude"));
                TerrainFrequencyMult = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainFrequencyMult"));
                TerrainSlopeLevel = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainSlopeLevel"));
                RiverNodes = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverNodes"));
                RiverSourceStrength = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverSourceStrength"));
                RiverWindiness = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverWindiness"));
                RiverWidth = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverWidth"));
                RiverElevation = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverElevation"));
                RiverMapWeight = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverMapWeight"));
                MaxMineCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MaxMineCount"));
                MinMineCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MinMineCount"));
                RuinCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RuinCount"));
                PineTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "PineTreeCount"));
                BirchTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "BirchTreeCount"));
                ChestnutTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "ChestnutTreeCount"));
                MapleTreeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "MapleTreeCount"));
                BlueberryBushCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "BlueberryBushCount"));
                DandelionBushCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "DandelionBushCount"));
                SlopeCount = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "SlopeCount"));
            }
            catch
            {
                Debug.LogWarning("Unable to load settings file, using default parameters!"); //Fail?
            }
            //Woo I hope that went ok!
            MapSizeX = mapSize.x;
            MapSizeY = mapSize.x;
            seed = mapSize.y;
            if (MapSizeX != mapSize.y)
            {
                rand = new System.Random(seed);
            }
            else
            {
                seed = UnityEngine.Random.Range(-8192, 8192);
                rand = new System.Random(seed);
            }
            RiverMapper = new bool[MapSizeX, MapSizeY];
            EntityMapper = new bool[MapSizeX, MapSizeY];
            noise = new FastNoiseLite(seed);
            List<Dictionary<string, object>> jsonEntities = new List<Dictionary<string, object>>();
            List<float[,]> floatMapCombiner = new List<float[,]>();
            floatMapCombiner.Add(GenerateBaseLayerNoise(MapSizeX, MapSizeY, TerrainAmplitude, seed));
            floatMapCombiner.Add(GenerateSlopeMap(MapSizeX, MapSizeY, TerrainSlopeLevel));
            float[,] finalFloatMap = GenerateFinalRiverMap(ReturnMeanedMap(floatMapCombiner, false), out jsonEntities, MapSizeX, MapSizeY, RiverNodes, RiverWindiness, RiverWidth, RiverElevation);
            int[,] normalizedMap = new int[MapSizeX, MapSizeY];
            normalizedMap = ConvertMap(finalFloatMap, TerrainMinHeight, TerrainMaxHeight);
            jsonEntities = PlaceEntities(normalizedMap, jsonEntities);
            SaveTerrainMap(normalizedMap, MapSizeX, MapSizeY, jsonEntities);
            //now load the file
            while (!File.Exists(PluginPath + "/newMap.json"))
            {
                Thread.Sleep(500);
            }
            Statics.Logger.LogInfo("Loading randomised map");
            MapFileReference mapFileReference = MapFileReference.FromDisk(PluginPath + "/newMap");
            __instance.LoadMap(mapFileReference);
            Statics.Logger.LogInfo("Finished loading");
            return false;
        }
        public static float[,] GenerateFinalRiverMap(float[,] map, out List<Dictionary<string, object>> jsonEntities, int xSize, int ySize, int nodes, float windiness, float width, float elevation)
        {
            float scaledWidth = width * (xSize / 256);
            if (scaledWidth < 2)
            {
                scaledWidth = 2;
            }
            // First, random phase angles (radians)
            float angle1 = (float)rand.NextDouble() * 2.0f * 3.14159f;
            float angle2 = (float)rand.NextDouble() * 2.0f * 3.14159f;
            float angle3 = (float)rand.NextDouble() * 2.0f * 3.14159f;
            float[,] riverMap = new float[xSize, ySize];
            // Calculate our X center for our map edge.
            int Y = ySize - 1;
            float omega1 = (Y / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle1;
            float omega2 = (Y / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle2;
            float omega3 = (Y / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle3;
            float center = 0.5f * MathF.Sin(omega1 * 1) * windiness + 0.3f * MathF.Sin(omega2 * 3) * windiness + 0.1f * MathF.Sin(omega3 * 5) * windiness;
            center = center * ySize / 2 + ySize / 2;
            jsonEntities = new List<Dictionary<string, object>>();
            float x = 0f;
            while (x < xSize)
            {
                // Calculate center
                // Using three sin functions for additional meandering
                // They add together proportionally to come up to -1 to 1
                omega1 = (x / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle1;
                omega2 = (x / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle2;
                omega3 = (x / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle3;
                center = 0.5f * MathF.Sin(omega1 * 1) * windiness + 0.3f * MathF.Sin(omega2 * 3) * windiness + 0.1f * MathF.Sin(omega3 * 5) * windiness;

                float center2 = (int)(center * ySize / 2 + ySize / 2);
                float riverWidth = scaledWidth + scaledWidth * 1.2f * Math.Abs(center);
                int xrangeStart = 0;
                int xrangeEnd = (int)Math.Round(x + riverWidth);
                if ((int)Math.Round(x - riverWidth) > 0)
                {
                    xrangeStart = (int)Math.Round(x - riverWidth);
                }
                if ((int)Math.Round(x + riverWidth) > xSize)
                {
                    xrangeEnd = xSize;
                }
                foreach (int x2 in Enumerable.Range(xrangeStart, xrangeEnd))
                {
                    int yrangeStart = 0;
                    int yrangeEnd = (int)Math.Round(center2 + riverWidth);
                    if ((int)Math.Round(center2 - riverWidth) > 0)
                    {
                        yrangeStart = (int)Math.Round(center2 - riverWidth);
                    }
                    if ((int)Math.Round(center2 + riverWidth) > ySize)
                    {
                        yrangeEnd = ySize;
                    }
                    foreach (int y in Enumerable.Range(yrangeStart, yrangeEnd))
                    {
                        UnityEngine.Vector2 a = new UnityEngine.Vector2(x, center2);
                        UnityEngine.Vector2 b = new UnityEngine.Vector2(x2, y);

                        float dist = UnityEngine.Vector2.Distance(a, b);
                        if (dist < riverWidth)
                        {
                            if (x2 >= xSize)
                            {
                                continue;
                            }
                            if (y >= ySize)
                            {
                                continue;
                            }
                            riverMap[x2, y] = RiverElevation;
                            RiverMapper[x2,y] = true; //gotta mark that rivermap
                        }
                    }
                }
                x += 0.2f;
            }
            List<float[,]> computeList = new List<float[,]>();
            computeList.Add(map);
            int counter = 0;
            while (counter < RiverMapWeight) //We weight the river map over the terrain map according to the ini parameter.
            {
                computeList.Add(riverMap);
                counter++;
            }
            float[,] finalMap = ReturnMeanedMap(computeList, true);
            counter = 0;
            //Now add water sources
            int lastTargetZ = Int32.MinValue;
            while (counter < xSize)
            {
                int targetZ = ReturnScaledIntFromFloat(finalMap[Y, counter]);
                if (RiverMapper[Y,counter] || (lastTargetZ == targetZ)) //We are in the riverbed!  (Or it's the same height as an adjacent block we just placed that had a riversource tile) Either way, place a source block.
                {
                    Dictionary<string, object> riverSourceProperty = new Dictionary<string, object>();
                    Dictionary<string, object> riverSourceComponentsDictionary = new Dictionary<string, object>();
                    Dictionary<string, object> waterSourceComponentsDictionary = new Dictionary<string, object>();
                    Dictionary<string, object> waterBlockComponentsDictionary = new Dictionary<string, object>();
                    Dictionary<string, int> riverSourceCoordinates = new Dictionary<string, int>();
                    riverSourceProperty.Add("Id", Guid.NewGuid().ToString());
                    riverSourceProperty.Add("Template", "WaterSource");
                    waterSourceComponentsDictionary.Add("SpecifiedStrength", RiverSourceStrength);
                    waterSourceComponentsDictionary.Add("CurrentStrength", RiverSourceStrength);
                    riverSourceCoordinates.Add("X", counter);
                    riverSourceCoordinates.Add("Y", Y);
                    riverSourceCoordinates.Add("Z", targetZ);
                    waterBlockComponentsDictionary.Add("Coordinates", riverSourceCoordinates);
                    riverSourceComponentsDictionary.Add("WaterSource", waterSourceComponentsDictionary);
                    riverSourceComponentsDictionary.Add("BlockObject", waterBlockComponentsDictionary);
                    riverSourceProperty.Add("Components", riverSourceComponentsDictionary);
                    jsonEntities.Add(riverSourceProperty);
                    lastTargetZ = targetZ;
                }
                else
                {
                    lastTargetZ = Int32.MinValue; //Used to signal the last block was not a river source, nothing will ever equal this.
                }
                counter++;
            }
            return finalMap;
        }
        public static float[,] GenerateSlopeMap(int xSize, int ySize, float slope)
        {
            float[,] slopeMap = new float[xSize, ySize];
            float value = 0;
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter < xSize)
            {
                value = slope / xSize * xCounter;
                value = (value * 2) - 1;
                while (yCounter < ySize)
                {
                    slopeMap[xCounter, yCounter] = value;
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return slopeMap;
        }
        public static int[,] ConvertMap(float[,] map, int lower, int upper)
        {
            int size = map.GetLength(0);
            int[,] finalResult = new int[size, size];
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter < size)
            {
                while (yCounter < size)
                {
                    float value = map[xCounter, yCounter];
                    int range = upper - lower;
                    value = (value * range) + lower;
                    int intValue = (int)Math.Round(value);
                    if (intValue > upper)
                    {
                        intValue = upper;
                    }
                    finalResult[xCounter, yCounter] = intValue;
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return finalResult;
        }
        public static float[,] GenerateBaseLayerNoise(int xSize, int ySize, float hills, int seed)
        {
            
            return GenerateNoiseMap(xSize, ySize, -1, 1, 0.2f * hills, TerrainNoiseType);
        }
        public static float[,] GenerateNoiseMap(int xSize, int ySize, int freqMult, int cubicScale, float amplitude, FastNoiseLite.NoiseType noiseType)
        {
            if (freqMult == -1)
            {
                freqMult = TerrainFrequencyMult;
            }
            float[,] result = new float[xSize, ySize];
            int xCounter = 0;
            int yCounter = 0;
            int xTrueScale = cubicScale;
            int yTrueScale = cubicScale;
            if (cubicScale > 10)
            {
                xTrueScale = rand.Next(1, cubicScale);
                yTrueScale = rand.Next(1, cubicScale);
            }
            while (xCounter <= xSize)
            {
                while (yCounter <= ySize)
                {
                    float dataResult = GenerateRawNoise(xCounter, yCounter, noiseType, freqMult) * amplitude;
                    if (dataResult > 1)
                    {
                        dataResult = 1;
                    }
                    else if (dataResult < -1)
                    {
                        dataResult = -1;
                    }
                    int xScaleCounter = 0;
                    int yScaleCounter = 0;
                    while (xScaleCounter <= xTrueScale)
                    {
                        while (yScaleCounter <= yTrueScale)
                        {
                            if (((xCounter + xTrueScale) < xSize) && ((yCounter + yTrueScale) < ySize))
                            {
                                result[xCounter + xScaleCounter, yCounter + yScaleCounter] = dataResult;
                            }
                            yScaleCounter++;
                        }
                        yScaleCounter = 0;
                        xScaleCounter++;
                    }
                    yCounter = yCounter + yTrueScale;
                }
                yCounter = 0;
                xCounter = xCounter + xTrueScale;
            }
            return result;
        }
        public static float GenerateRawNoise(int x, int y, FastNoiseLite.NoiseType NoiseType, int FrequencyMult)
        {
            noise.SetNoiseType(NoiseType);
            noise.SetFractalType(FastNoiseLite.FractalType.PingPong);
            if (NoiseType.Equals(FastNoiseLite.NoiseType.Cellular))
            {
                noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
                noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            }
            noise.SetFrequency(0.001f * FrequencyMult);
            //Get the noise.
            return noise.GetNoise(x, y);

        }
    }
}
