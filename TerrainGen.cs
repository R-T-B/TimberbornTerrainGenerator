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
using static TimberbornTerrainGenerator.RandomMapSettingsBox;

namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(MapEditorSceneLoader), nameof(MapEditorSceneLoader.StartNewMap))]
    public class TerrainGen
    {
        public static System.Random rand = new System.Random();
        public static FastNoiseLite noise = new FastNoiseLite();
        public static bool[,] RiverMapper = new bool[32, 32];
        public static bool customMapEnabled = false;
        public static bool Prefix(MapEditorSceneLoader __instance, Vector2Int mapSize)
        {
            if (customMapEnabled)
            {
                if (Seed == -1)
                {
                    Seed = UnityEngine.Random.Range(-8192, 8192);
                    rand = new System.Random(Seed);
                }
                else
                {
                    rand = new System.Random(Seed);
                }
                RiverMapper = new bool[MapSizeX, MapSizeY];
                EntityMapper = new bool[MapSizeX, MapSizeY];
                noise = new FastNoiseLite(Seed);
                List<Dictionary<string, object>> jsonEntities;
                float[,] riverlessMap;
                if (TerrainSlopeEnabled)
                {
                    List<float[,]> floatMapCombiner = new List<float[,]>();
                    floatMapCombiner.Add(GenerateBaseLayerNoise(TerrainAmplitude, Seed));
                    floatMapCombiner.Add(GenerateSlopeMap(MapSizeX, MapSizeY, TerrainSlopeLevel, false));
                    riverlessMap = ReturnMeanedMap(floatMapCombiner, false);
                }
                else
                {
                    riverlessMap = GenerateBaseLayerNoise(TerrainAmplitude, Seed);
                }
                float[,] finalFloatMap = GenerateFinalRiverMap(riverlessMap, out jsonEntities, RiverNodes, RiverWindiness, RiverWidth, RiverElevation);
                int[,] normalizedMap = new int[MapSizeX, MapSizeY];
                normalizedMap = ConvertMapToIntArray(finalFloatMap);
                jsonEntities = PlaceEntities(normalizedMap, jsonEntities);
                SaveTerrainMap(normalizedMap, MapSizeX, MapSizeY, jsonEntities);
                //now load the file
                while (!File.Exists(MapFileTools.fileName))
                {
                    Thread.Sleep(100);
                }
                Thread.Sleep(500); //give slow HDD users time to completely serialize the file.
                Statics.Logger.LogInfo("Loading randomised map");
                MapFileReference mapFileReference = MapFileReference.FromDisk(PluginPath + "/newMap");
                __instance.LoadMap(mapFileReference);
                Statics.Logger.LogInfo("Finished loading");
                customMapEnabled = false; //This flag must be reset.
                return false;
            }
            else
            {
                __instance._sceneLoader.LoadScene(MapEditorSceneLoader.MapEditorSceneIndex, MapEditorSceneLoader.ExtraWaitDuration, MapEditorSceneParameters.CreateNewMapParameters(mapSize), __instance.Tip());
                return false;
            }
        }
        public static float[,] GenerateFinalRiverMap(float[,] map, out List<Dictionary<string, object>> jsonEntities, int nodes, float windiness, float width, float elevation) //this old ported python is preventing us from using nonsquare maps, needs improvement!
        {
            float scaledWidth = width * (((MapSizeX + MapSizeY) / 2) / 256);
            jsonEntities = new List<Dictionary<string, object>>();
            if (scaledWidth < 2)
            {
                scaledWidth = 2;
            }
            // First, random phase angles (radians)
            float angle1 = (float)rand.NextDouble() * 2.0f * 3.14159f;
            float angle2 = (float)rand.NextDouble() * 2.0f * 3.14159f;
            float angle3 = (float)rand.NextDouble() * 2.0f * 3.14159f;
            float[,] riverMap = new float[MapSizeX, MapSizeY];
            // Calculate our X center for our map edge.
            int Y = MapSizeY - 1;
            float omega1 = (Y / MapSizeY * 2.0f * 3.14159f * nodes * 0.4f) + angle1;
            float omega2 = (Y / MapSizeY * 2.0f * 3.14159f * nodes * 0.4f) + angle2;
            float omega3 = (Y / MapSizeY * 2.0f * 3.14159f * nodes * 0.4f) + angle3;
            float center = 0.5f * MathF.Sin(omega1 * 1) * windiness + 0.3f * MathF.Sin(omega2 * 3) * windiness + 0.1f * MathF.Sin(omega3 * 5) * windiness;
            center = center * MapSizeY / 2 + MapSizeY / 2;
            float x = 0f;
            while (x < MapSizeX)
            {
                // Calculate center
                // Using three sin functions for additional meandering
                // They add together proportionally to come up to -1 to 1
                omega1 = (x / MapSizeY * 2.0f * 3.14159f * nodes * 0.4f) + angle1;
                omega2 = (x / MapSizeY * 2.0f * 3.14159f * nodes * 0.4f) + angle2;
                omega3 = (x / MapSizeY * 2.0f * 3.14159f * nodes * 0.4f) + angle3;
                center = 0.5f * MathF.Sin(omega1 * 1) * windiness + 0.3f * MathF.Sin(omega2 * 3) * windiness + 0.1f * MathF.Sin(omega3 * 5) * windiness;

                float center2 = (int)(center * MapSizeY / 2 + MapSizeY / 2);
                float riverWidth = scaledWidth + scaledWidth * 1.2f * Math.Abs(center);
                int xrangeStart = 0;
                int xrangeEnd = (int)Math.Round(x + riverWidth);
                if ((int)Math.Round(x - riverWidth) > 0)
                {
                    xrangeStart = (int)Math.Round(x - riverWidth);
                }
                if ((int)Math.Round(x + riverWidth) > MapSizeY) //This is one of those places where inverting coordinates makes things all wonky, be advised
                {
                    xrangeEnd = MapSizeY;
                }
                foreach (int y in Enumerable.Range(xrangeStart, xrangeEnd))
                {
                    int yrangeStart = 0;
                    int yrangeEnd = (int)Math.Round(center2 + riverWidth);
                    if ((int)Math.Round(center2 - riverWidth) > 0)
                    {
                        yrangeStart = (int)Math.Round(center2 - riverWidth);
                    }
                    if ((int)Math.Round(center2 + riverWidth) > MapSizeX)
                    {
                        yrangeEnd = MapSizeX;
                    }
                    foreach (int x2 in Enumerable.Range(yrangeStart, yrangeEnd))
                    {
                        UnityEngine.Vector2 a = new UnityEngine.Vector2(x, center2);
                        UnityEngine.Vector2 b = new UnityEngine.Vector2(y, x2); //End of coordinate wonkiness, for now.

                        float dist = UnityEngine.Vector2.Distance(a, b);
                        if (dist < riverWidth)
                        {
                            if (y >= MapSizeY)
                            {
                                continue;
                            }
                            if (x2 >= MapSizeX)
                            {
                                continue;
                            }
                            if (y == Y)
                            {
                                riverMap[x2, y] = RiverElevation / 2;
                            }
                            else
                            {
                                riverMap[x2, y] = RiverElevation;
                            }
                            RiverMapper[x2, y] = true; //gotta mark that rivermap
                        }
                    }
                }
                x += 0.2f;
            }
            List<float[,]> computeList = new List<float[,]>();
            if (RiverSlopeEnabled)
            {
                computeList.Add(riverMap);
                computeList.Add(GenerateSlopeMap(MapSizeX, MapSizeY, RiverSlopeLevel, false));
                riverMap = ReturnMeanedMapUsingMask(computeList, RiverMapper);
            }
            computeList.Clear();
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
            while ((counter < MapSizeX) && (Y < MapSizeY))
            {
                int targetZ = ReturnScaledIntFromFloat(finalMap[counter, Y]);
                if (RiverMapper[counter, Y] || (lastTargetZ == targetZ)) //We are in the riverbed!  (Or it's the same height as an adjacent block we just placed that had a riversource tile) Either way, place a source block.
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
                    riverSourceCoordinates.Add("X", Y); //Remember, never serialize in order kids...  another wonky place because we are serializing to the actual map data pool.
                    riverSourceCoordinates.Add("Y", counter);
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
        public static float[,] GenerateSlopeMap(int MapSizeX, int MapSizeY, float slope, bool inverted)
        {
            if (inverted)
            {
                float[,] slopeMap = new float[MapSizeX, MapSizeY];
                float value = 0;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        value = slope / MapSizeY * (MapSizeY - yCounter);
                        value = (value * 2) - 1;
                        slopeMap[xCounter, yCounter] = value;
                        yCounter++;
                    }
                    yCounter = 0;
                    xCounter++;
                }
                return slopeMap;
            }
            else
            {
                float[,] slopeMap = new float[MapSizeX, MapSizeY];
                float value = 0;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        value = slope / MapSizeY * yCounter;
                        value = (value * 2) - 1;
                        slopeMap[xCounter, yCounter] = value;
                        yCounter++;
                    }
                    yCounter = 0;
                    xCounter++;
                }
                return slopeMap;
            }
        }
        public static int[,] ConvertMapToIntArray(float[,] map)
        { //When you update this method please please please update Utilities.ReturnScaledIntFromFloat to make the math match.
            int[,] finalResult = new int[MapSizeX, MapSizeY];
            int xCounter = 0;
            int yCounter = 0;
            int range = (TerrainMaxHeight - TerrainMinHeight) / 2; //We divide by two because to make TerrainMinHeight the absolute map floor, we need to prepare for the possibility that there may be negative values needing z space.  An even split makes sense.
            while (xCounter < MapSizeX)
            {
                while (yCounter < MapSizeY)
                {
                    float value = map[xCounter, yCounter];
                    if (value > 1)
                    {
                        value = 1;
                    }
                    else if (value < -1)
                    {
                        value = -1;
                    }
                    if (value > 0)
                    {
                        value = (TerrainMaxHeight - range) + (value * range);
                        int intValue = (int)Math.Round(value);
                        if (intValue > TerrainMaxHeight) //If we clip, we dip
                        {
                            intValue--;
                            finalResult[xCounter, yCounter] = intValue;
                        }
                        else
                        {
                            finalResult[xCounter, yCounter] = intValue;
                        }
                    }
                    else
                    {
                        value = 1 - Mathf.Abs(value);
                        value = value * range + TerrainMinHeight;
                        int intValue = (int)Math.Round(value);
                        if (intValue < TerrainMinHeight) //If we clip...  assign the lowest possible value
                        {
                            finalResult[xCounter, yCounter] = TerrainMinHeight;
                        }
                        else
                        {
                            finalResult[xCounter, yCounter] = intValue;
                        }
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return finalResult;
        }
        public static float[,] GenerateBaseLayerNoise(float hills, int Seed)
        {

            return GenerateNoiseMap(MapSizeX, MapSizeY, -1, 0.2f * hills, TerrainNoiseType);
        }
        public static float[,] GenerateNoiseMap(int mapSizeX, int mapSizeY, int freqMult, float amplitude, FastNoiseLite.NoiseType noiseType)
        {
            if (freqMult == -1)
            {
                freqMult = TerrainFrequencyMult;
            }
            float[,] result = new float[mapSizeX, mapSizeY];
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter <= mapSizeX - 1)
            {
                while (yCounter <= mapSizeY - 1)
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
                    result[xCounter, yCounter] = dataResult;
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float GenerateRawNoise(int xInt, int yInt, FastNoiseLite.NoiseType NoiseType, int FrequencyMult)
        {
            noise.SetNoiseType(NoiseType);
            noise.SetFractalType(FastNoiseLite.FractalType.PingPong);
            if (NoiseType.Equals(FastNoiseLite.NoiseType.Cellular))
            {
                noise.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
                noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            }
            noise.SetFrequency(0.001f * FrequencyMult);
            return noise.GetNoise(xInt, yInt);

        }
    }
}
