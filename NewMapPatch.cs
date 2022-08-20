using BepInEx;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapSystem;
using Timberborn.MapSystemUI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Core;

namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(MapEditorSceneLoader), nameof(MapEditorSceneLoader.StartNewMap))]
    class NewMapPatch
    {
        public static System.Random rand = new System.Random();
        public static FastNoiseLite noise = new FastNoiseLite();
        public static int mapSizeX;
        public static int mapSizeY;
        public static int seed;
        //BEGIN EXTERNAL SETTINGS
        public static int TerrainMinHeight = 10;
        public static int TerrainMaxHeight = 20;
        public static FastNoiseLite.NoiseType TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
        public static float TerrainAmplitude = 0.7125f;
        public static int TerrainFrequencyMult = 1;
        public static int RiverNodes = 2;
        public static float RiverWindiness = 0.4125f;
        public static int RiverWidth = 8;
        public static float RiverDepth = 0.2f;
        public static int MaxMineCount = 4;
        public static int MinMineCount = 0;
        public static int RuinCount = 500;
        public static int PineTreeCount = 2400;
        public static int BirchTreeCount = 800;
        public static int ChestnutTreeCount = 1000;
        public static int MapleTreeCount = 600;
        public static int BlueberryBushCount = 3200;
        public static int DandelionBushCount = 1600;
        public static int SlopeCount = 128;
        //END EXTERNAL SETTINGS
        public static bool Prefix(Vector2Int mapSize, MapEditorSceneLoader __instance)
        {
            //Try load settings
            try
            {
                IniParser iniParser = new IniParser(Statics.PluginPath + "/settings.ini");
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
                else
                {
                    TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
                    Debug.LogWarning("Unable to determine noise settings, proceeding with Perlin Noise.");
                }
                TerrainAmplitude = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainAmplitude"));
                TerrainFrequencyMult = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "TerrainFrequencyMult"));
                RiverNodes = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverNodes"));
                RiverWindiness = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverWindiness"));
                RiverWidth = int.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverWidth"));
                RiverDepth = float.Parse(iniParser.GetSetting("TimberbornTerrainGenerator", "RiverDepth"));
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
            mapSizeX = mapSize.x;
            mapSizeY = mapSize.x;
            seed = mapSize.y;
            if (mapSizeX != mapSize.y)
            {
                rand = new System.Random(seed);
            }
            else
            {
                seed = UnityEngine.Random.Range(-8192, 8192);
                rand = new System.Random(seed);
            }
            noise = new FastNoiseLite(seed);
            List<Dictionary<String, System.Object>> jsonEntities = new List<Dictionary<String, System.Object>>();
            List<float[,]> floatMapCombiner = new List<float[,]>();
            floatMapCombiner.Add(generateMultiLayerNoise(mapSizeX, mapSizeY, TerrainAmplitude, seed));
            floatMapCombiner.Add(GenerateSlopeMap(mapSizeX, mapSizeY, 0.8f));
            float[,] finalFloatMap = GenerateFinalRiverSlopeMap(Utilities.ReturnMeanedMap(floatMapCombiner), out jsonEntities, mapSizeX, mapSizeY, RiverNodes, RiverWindiness, RiverWidth, RiverDepth);
            int[,] normalizedMap = new int[mapSizeX, mapSizeY];
            normalizedMap = ConvertMap(finalFloatMap, TerrainMinHeight, TerrainMaxHeight);
            //NDArray finalMap = sealRiverSources(normalizedMap, riverCenter);
            //entities = placeEntities(finalMap,entities,int(seedInt))
            //#########REMEBER THIS IS finalMap not normalizedMap!
            MapFileTools.SaveTerrainMap(normalizedMap, mapSizeX, mapSizeY, jsonEntities);
            //now load the file
            while (!File.Exists(Statics.PluginPath + "/newMap.json"))
            {
                Thread.Sleep(500);
            }
            Statics.Logger.LogInfo("Loading randomised map");
            MapFileReference mapFileReference = MapFileReference.FromDisk(Statics.PluginPath + "/newMap");
            __instance.LoadMap(mapFileReference);
            Statics.Logger.LogInfo("Finished loading");
            return false;
        }
        private static float[,] GenerateFinalRiverSlopeMap(float[,] map, out List<Dictionary<String, System.Object>> jsonEntities, int xSize, int ySize, int nodes, float windiness, float width, float depth)
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
            //Placing edge water features.
            // Calculate our X center for our map edge.
            int Y = ySize - 1;
            float omega1 = (Y / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle1;
            float omega2 = (Y / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle2;
            float omega3 = (Y / ySize * 2.0f * 3.14159f * nodes * 0.4f) + angle3;
            float center = 0.5f * MathF.Sin(omega1 * 1) * windiness + 0.3f * MathF.Sin(omega2 * 3) * windiness + 0.1f * MathF.Sin(omega3 * 5) * windiness;
            center = center * ySize / 2 + ySize / 2;
            jsonEntities = new List<Dictionary<String, System.Object>>();
            int counter = 0;
            while (counter < xSize)
            {
                Dictionary<String, System.Object> riverSourceProperty = new Dictionary<String, System.Object>();
                Dictionary<String, System.Object> riverSourceComponentsDictionary = new Dictionary<String, System.Object>();
                Dictionary<String, System.Object> waterSourceComponentsDictionary = new Dictionary<String, System.Object>();
                Dictionary<String, System.Object> waterBlockComponentsDictionary = new Dictionary<String, System.Object>();
                Dictionary<String, int> riverSourceCoordinates = new Dictionary<String, int>();
                riverSourceProperty.Add("Id", Guid.NewGuid().ToString());
                //jsonEntities.Add(riverSourceProperty);
                //riverSourceProperty = new Dictionary<String, System.Object>();
                riverSourceProperty.Add("Template", "WaterSource");
                //jsonEntities.Add(riverSourceProperty);
                //riverSourceProperty = new Dictionary<String, System.Object>();
                waterSourceComponentsDictionary.Add("SpecifiedStrength", 3.25f);
                waterSourceComponentsDictionary.Add("CurrentStrength", 3.25f);
                riverSourceCoordinates.Add("X", counter);
                riverSourceCoordinates.Add("Y", Y);
                riverSourceCoordinates.Add("Z", (int)Utilities.ReturnScaledIntFromFloat(map[Y, counter] - depth));
                waterBlockComponentsDictionary.Add("Coordinates", riverSourceCoordinates);
                riverSourceComponentsDictionary.Add("WaterSource", waterSourceComponentsDictionary);
                riverSourceComponentsDictionary.Add("BlockObject", waterBlockComponentsDictionary);
                riverSourceProperty.Add("Components", riverSourceComponentsDictionary);
                jsonEntities.Add(riverSourceProperty);
                counter++;
            }
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
                            riverMap[x2, y] = depth * (-1);
                        }
                    }
                }
                x += 0.2f;
            }
            List<float[,]> addingList = new List<float[,]>();
            addingList.Add(map);
            addingList.Add(riverMap);
            return Utilities.ReturnAdditiveMap(addingList);
        }
        private static float[,] GenerateSlopeMap(int xSize, int ySize, float slope)
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
        private static int[,] ConvertMap(float[,] map, int lower, int upper)
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
        private static Dictionary<String, String> WriteValue(Dictionary<String, String> dictTree, string[] path, string value)
        {
            return dictTree;
        }
        private static float[,] generateMultiLayerNoise(int xSize, int ySize, float hills, int seed)
        {
            List<float[,]> noiseList = new List<float[,]>();
            noiseList.Add(GenerateNoiseMap(xSize, ySize, 48, 0.6f * hills, seed));
            noiseList.Add(GenerateNoiseMap(xSize, ySize, 32, 0.4f * hills, seed));
            noiseList.Add(GenerateNoiseMap(xSize, ySize, 24, 0.3f * hills, seed));
            noiseList.Add(GenerateNoiseMap(xSize, ySize, 16, 0.2f * hills, seed));
            noiseList.Add(GenerateNoiseMap(xSize, ySize, 12, 0.15f * hills, seed));
            return Utilities.ReturnAdditiveMap(noiseList);
        }
        private static float[,] GenerateNoiseMap(int xSize, int ySize, int scale, float amplitude, int seed)
        {
            float[,] result = new float[xSize, ySize];
            int xCounter = 0;
            int yCounter = 0;
            int xTrueScale = rand.Next(1, scale);
            int yTrueScale = rand.Next(1, scale);
            while (xCounter <= xSize)
            {
                while (yCounter <= ySize)
                {
                    float dataResult = GenerateRawNoise(xCounter, yCounter,TerrainNoiseType, TerrainFrequencyMult) * amplitude;
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
        private static float GenerateRawNoise(int x, int y, FastNoiseLite.NoiseType NoiseType, int FrequencyMult)
        {
            noise.SetNoiseType(NoiseType);
            noise.SetFractalType(FastNoiseLite.FractalType.None);
            noise.SetFrequency(0.02f * FrequencyMult);
            //Get the noise.
            return noise.GetNoise(x, y);

        }
    }

    [BepInPlugin("org.bepinex.plugins.timberbornterraingenerator", "TimberbornTerrainGenerator", "0.4.0")]
    public class TimberbornTerrainGeneratorPlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            var maxsize = typeof(NewMapBox).GetField("MaxMapSize", BindingFlags.Static | BindingFlags.NonPublic);
            var minsize = typeof(NewMapBox).GetField("MinMapSize", BindingFlags.Static | BindingFlags.NonPublic);
            maxsize.SetValue(null, (int)Int32.MaxValue);
            minsize.SetValue(null, (int)Int32.MinValue);
        }
    }
}
