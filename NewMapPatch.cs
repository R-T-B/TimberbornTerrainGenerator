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
using static FastNoiseLite;

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
        //BEGIN EXTERNAL LOADABLE INI SETTINGS
        public static int TerrainMinHeight = 10;
        public static int TerrainMaxHeight = 20;
        public static FastNoiseLite.NoiseType TerrainNoiseType = FastNoiseLite.NoiseType.Perlin;
        public static float TerrainAmplitude = 2.5f;
        public static int TerrainFrequencyMult = 10;
        public static int RiverNodes = 3;
        public static float RiverWindiness = 0.4125f;
        public static int RiverWidth = 4;
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
        //END EXTERNAL LOADABLE INI SETTINGS
        public static bool Prefix(Vector2Int mapSize, MapEditorSceneLoader __instance)
        {
            //Try load .ini settings
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
            floatMapCombiner.Add(GenerateBaseLayerNoise(mapSizeX, mapSizeY, TerrainAmplitude, seed));
            floatMapCombiner.Add(GenerateSlopeMap(mapSizeX, mapSizeY, 0.8f));
            float[,] finalFloatMap = GenerateFinalRiverSlopeMap(Utilities.ReturnMeanedMap(floatMapCombiner), out jsonEntities, mapSizeX, mapSizeY, RiverNodes, RiverWindiness, RiverWidth, RiverDepth);
            int[,] normalizedMap = new int[mapSizeX, mapSizeY];
            normalizedMap = ConvertMap(finalFloatMap, TerrainMinHeight, TerrainMaxHeight);
            jsonEntities = PlaceEntities(normalizedMap, jsonEntities);
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
        public static List<Dictionary<String, System.Object>> PlaceEntities(int[,] map, List<Dictionary<String, System.Object>> entitiesList)
        {
            float mapScaler = (mapSizeX * mapSizeX) / 65536f;
            float mineMapScaler = mapScaler;
            if ((mineMapScaler < 0.255) && (!(mineMapScaler < 0.22))) //This deals with float imprecision cheating 128x128 players out of their own default mine.
            {
                mineMapScaler = 0.255f;
            }
            int minesQuantity = (int)Math.Round(MaxMineCount * mineMapScaler);
            int scaledRuinsCount = (int)Math.Round(RuinCount * mapScaler);
            int scaledPineTreeCount = (int)Math.Round(PineTreeCount * mapScaler);
            int scaledBirchTreeCount = (int)Math.Round(BirchTreeCount * mapScaler);
            int scaledChestnutTreeCount = (int)Math.Round(ChestnutTreeCount * mapScaler);
            int scaledMapleTreeCount = (int)Math.Round(MapleTreeCount * mapScaler);
            int scaledBlueberryBushCount = (int)Math.Round(BlueberryBushCount * mapScaler);
            int scaledDandelionBushCount = (int)Math.Round(DandelionBushCount * mapScaler);
            int scaledSlopeCount = (int)Math.Round(SlopeCount * mapScaler);


            if (minesQuantity < MinMineCount)
            {
                minesQuantity = MinMineCount;
            }
            //Place all the stuff!
            rand = new System.Random(seed);
            entitiesList = GetMines(map, minesQuantity, entitiesList);
            rand = new System.Random(seed + 25);
            noise.SetSeed(seed + 25);
            entitiesList = GetRuins(map, scaledRuinsCount, entitiesList);
            rand = new System.Random(seed + 50);
            noise.SetSeed(seed + 50);
            entitiesList = GetPineTrees(map, scaledPineTreeCount, entitiesList);
            rand = new System.Random(seed + 75);
            noise.SetSeed(seed + 75);
            entitiesList = GetBirchTrees(map, scaledBirchTreeCount, entitiesList);
            rand = new System.Random(seed + 100);
            noise.SetSeed(seed + 100);
            entitiesList = GetChestnutTrees(map, scaledChestnutTreeCount, entitiesList);
            rand = new System.Random(seed + 125);
            noise.SetSeed(seed + 125);
            entitiesList = GetMapleTrees(map, scaledMapleTreeCount, entitiesList);
            rand = new System.Random(seed + 150);
            noise.SetSeed(seed + 150);
            entitiesList = GetBlueberries(map, scaledBlueberryBushCount, entitiesList);
            rand = new System.Random(seed + 175);
            noise.SetSeed(seed + 175);
            entitiesList = GetDandelions(map, scaledDandelionBushCount, entitiesList);
            rand = new System.Random(seed + 200);
            noise.SetSeed(seed + 200);
            entitiesList = GetSlopes(map, scaledSlopeCount, entitiesList);
                

            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetMines(int[,] map, int targetMines, List<Dictionary<String, System.Object>> entitiesList)
        {
            Boolean[,] mineMap = new bool[mapSizeX, mapSizeY];
            //Place some mines!
            int minesNum = 0;
            while (minesNum < targetMines)
            {
                int bufferZone = 4; //this needs to be at least 4 to fit a mine
                int x = rand.Next(0, mapSizeX - (bufferZone + 1));
                int y = rand.Next(0, mapSizeY - (bufferZone + 1));
                int z = map[y, x];
                int testX = 0;
                int testY = 0;
                Boolean tryAnotherLocation = false;
                while (testY <= bufferZone)
                {
                    if ((z != map[y + testY, x + testX]) || mineMap[y + testY, x + testX])
                    {
                        tryAnotherLocation = true;
                    }
                    testX++;
                    if (testX > bufferZone)
                    {
                        testX = 0;
                        testY++;
                    }
                }
                testX = 0;
                testY = 0;
                if (tryAnotherLocation)
                {
                    tryAnotherLocation = false;
                    continue;
                }
                Dictionary<String, System.Object> mineProperty = new Dictionary<String, System.Object>();
                Dictionary<String, System.Object> mineComponentsDictionary = new Dictionary<String, System.Object>();
                Dictionary<String, System.Object> mineBlockComponentsDictionary = new Dictionary<String, System.Object>();
                Dictionary<String, int> mineBlockCoordinatesDictionary = new Dictionary<String, int>();
                Dictionary<String, bool> mineIsDryDictionary = new Dictionary<String, bool>();
                mineBlockCoordinatesDictionary.Add("X", x);
                mineBlockCoordinatesDictionary.Add("Y", y);
                mineBlockCoordinatesDictionary.Add("Z", z);
                mineBlockComponentsDictionary.Add("Coordinates", mineBlockCoordinatesDictionary);
                mineComponentsDictionary.Add("BlockObject", mineBlockComponentsDictionary);
                mineIsDryDictionary.Add("IsDry", true);
                mineComponentsDictionary.Add("DryObject", mineIsDryDictionary);
                mineProperty.Add("Id", Guid.NewGuid().ToString());
                mineProperty.Add("Template", "UndergroundRuins");
                mineProperty.Add("Components", mineComponentsDictionary);
                testX = 0;
                testY = 0;
                while (testY <= bufferZone)
                {
                    mineMap[y + testY, x + testX] = true;
                    testX++;
                    if (testX > bufferZone)
                    {
                        testX = 0;
                        testY++;
                    }
                }
                entitiesList.Add(mineProperty);
                minesNum++;
            }
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetRuins(int[,] map, int targetRuins, List<Dictionary<String, System.Object>> entitiesList)
        {
            float[,] ruinsMap = new float[mapSizeX, mapSizeY];
            //Place some Ruins!
            int ruinsNum = 0;
            ruinsMap = GenerateNoiseMap(mapSizeX, mapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = Utilities.GetFloatArrayMax(ruinsMap);
            float modifier = 1.00f;
            float prevalence;
            while (ruinsNum < targetRuins)
            {
                ruinsNum = 0;
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < mapSizeX)
                {
                    while (yCounter < mapSizeY)
                    {
                        if (ruinsMap[yCounter, xCounter] > prevalence)
                        {
                            int ruinHeight = (int)Mathf.Max(Mathf.Min((ruinsMap[yCounter,xCounter] - prevalence) * 400, 8), 1);
                            int ruinYield = ruinHeight * 15;
                            int variantInt = rand.Next(1,6);
                            int z = map[yCounter, xCounter];
                            Dictionary<String, System.Object> ruinProperty = new Dictionary<String, System.Object>();
                            Dictionary<String, System.Object> ruinComponentsDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, System.Object> ruinBlockComponentsDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, int> ruinBlockCoordinatesDictionary = new Dictionary<String, int>();
                            Dictionary<String, bool> ruinIsDryDictionary = new Dictionary<String, bool>();
                            Dictionary<String, System.Object> ruinYieldDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, System.Object> ruinYieldGoodDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, String> ruinYieldGoodIdDictionary = new Dictionary<String, String>();
                            Dictionary<String, String> ruinModelsDictionary = new Dictionary<String, String>();
                            string variant = "";
                            if (variantInt == 1)
                                variant = "A";
                            else if (variantInt == 2)
                                variant = "B";
                            else if (variantInt == 3)
                                variant = "C";
                            else if (variantInt == 4)
                                variant = "D";
                            else if (variantInt == 5)
                                variant = "E";

                            ruinIsDryDictionary.Add("IsDry", true);
                            ruinModelsDictionary.Add("VariantId", variant);
                            ruinYieldGoodIdDictionary.Add("Id", "ScrapMetal");
                            ruinYieldGoodDictionary.Add("Good", ruinYieldGoodIdDictionary);
                            ruinYieldGoodDictionary.Add("Amount", (int)ruinYield);
                            ruinYieldDictionary.Add("Yield", ruinYieldGoodDictionary);
                            ruinBlockCoordinatesDictionary.Add("X", xCounter);
                            ruinBlockCoordinatesDictionary.Add("Y", yCounter);
                            ruinBlockCoordinatesDictionary.Add("Z", z);
                            ruinBlockComponentsDictionary.Add("Coordinates", ruinBlockCoordinatesDictionary);
                            ruinComponentsDictionary.Add("BlockObject", ruinBlockComponentsDictionary);
                            ruinComponentsDictionary.Add("Yielder:Ruin", ruinYieldDictionary);
                            ruinComponentsDictionary.Add("RuinModels", ruinModelsDictionary);
                            ruinComponentsDictionary.Add("DryObject", ruinIsDryDictionary);
                            ruinProperty.Add("Id", Guid.NewGuid().ToString());
                            ruinProperty.Add("Template", "RuinColumnH" + ruinHeight.ToString());
                            ruinProperty.Add("Components", ruinComponentsDictionary);
                            entitiesList.Add(ruinProperty);
                            ruinsNum += 1;
                            if (ruinsNum >= targetRuins)
                            {
                                return entitiesList;
                            }
                        }
                        yCounter++;
                    }
                    yCounter = 0;
                    xCounter++;
                }
            }
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetPineTrees(int[,] map, int targetPineTrees, List<Dictionary<String, System.Object>> entitiesList)
        {
            float[,] pineTreesMap = new float[mapSizeX, mapSizeY];
            //Place some PineTrees!
            int pineTreesNum = 0;
            pineTreesMap = GenerateNoiseMap(mapSizeX, mapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = Utilities.GetFloatArrayMax(pineTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (pineTreesNum < targetPineTrees)
            {
                pineTreesNum = 0;
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < mapSizeX)
                {
                    while (yCounter < mapSizeY)
                    {
                        if (pineTreesMap[yCounter, xCounter] > prevalence)
                        {
                            int z = map[yCounter, xCounter];
                            Dictionary<String, System.Object> pineTreeProperty = new Dictionary<String, System.Object>();
                            Dictionary<String, System.Object> pineTreeComponentsDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, System.Object> pineTreeBlockComponentsDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, int> pineTreeBlockCoordinatesDictionary = new Dictionary<String, int>();
                            Dictionary<String, bool> pineTreeIsDryDictionary = new Dictionary<String, bool>();
                            Dictionary<String, float> pineTreeGrowableDictionary = new Dictionary<String, float>();
                            Dictionary<String, float> pineTreeNaturalRandomizerDictionary = new Dictionary<String, float>();
                            Dictionary<String, System.Object> pineTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, float> pineTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<String, float>();
                            Dictionary<String, System.Object> pineTreeYielderCuttableYieldDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, System.Object> pineTreeYielderCuttableYieldGoodDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, String> pineTreeYielderCuttableYieldGoodIdDictionary = new Dictionary<String, String>();
                            Dictionary<String, float> pineTreeGatherableYieldGrowerGrowthDictionary = new Dictionary<String, float>();
                            Dictionary<String, System.Object> pineTreeYielderGatherableYieldDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, System.Object> pineTreeYielderGatherableYieldGoodDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, String> pineTreeYielderGatherableYieldGoodIdDictionary = new Dictionary<String, String>();
                            Dictionary<String, System.Object> pineTreeLivingNaturalResourceDictionary = new Dictionary<String, System.Object>();
                            Dictionary<String, bool> pineTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<String, bool>();

                            pineTreeIsDryDictionary.Add("IsDry", false);
                            pineTreeLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                            pineTreeYielderGatherableYieldGoodIdDictionary.Add("Id", "PineResin");
                            pineTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)rand.NextDouble());
                            pineTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                            pineTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)rand.NextDouble() - 0.5f);
                            pineTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)rand.NextDouble() - 0.5f);
                            pineTreeNaturalRandomizerDictionary.Add("Rotation", (float)(rand.Next(0, 360) + rand.NextDouble()));
                            pineTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            pineTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            pineTreeGrowableDictionary.Add("GrowthProgress", 1.0f);
                            pineTreeBlockCoordinatesDictionary.Add("X",xCounter);
                            pineTreeBlockCoordinatesDictionary.Add("Y",yCounter);
                            pineTreeBlockCoordinatesDictionary.Add("Z", z);
                            pineTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", pineTreeLivingNaturalResourceIsDeadDictionary);
                            pineTreeYielderGatherableYieldGoodDictionary.Add("Good", pineTreeYielderCuttableYieldGoodIdDictionary);
                            pineTreeYielderGatherableYieldGoodDictionary.Add("Amount", 2);
                            pineTreeYielderGatherableYieldDictionary.Add("Yield", pineTreeYielderGatherableYieldGoodDictionary);
                            pineTreeYielderCuttableYieldGoodDictionary.Add("Good", pineTreeYielderCuttableYieldGoodIdDictionary);
                            pineTreeYielderCuttableYieldGoodDictionary.Add("Amount", 2);
                            pineTreeYielderCuttableYieldDictionary.Add("Yield", pineTreeYielderCuttableYieldGoodDictionary);
                            pineTreeCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", pineTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                            pineTreeBlockComponentsDictionary.Add("Coordinates", pineTreeBlockCoordinatesDictionary);
                            pineTreeComponentsDictionary.Add("BlockObject", pineTreeBlockComponentsDictionary);
                            pineTreeComponentsDictionary.Add("Growable", pineTreeGrowableDictionary);
                            pineTreeComponentsDictionary.Add("NaturalResourceModelRandomizer", pineTreeNaturalRandomizerDictionary);
                            pineTreeComponentsDictionary.Add("CoordinatesOffseter", pineTreeCoordinatesOffseterCoordinatesOffsetDictionary);
                            pineTreeComponentsDictionary.Add("Yielder:Cuttable", pineTreeYielderCuttableYieldDictionary);
                            pineTreeComponentsDictionary.Add("GatherableYieldGrower", pineTreeGatherableYieldGrowerGrowthDictionary);
                            pineTreeComponentsDictionary.Add("Yielder:Gatherable", pineTreeYielderGatherableYieldDictionary);
                            pineTreeComponentsDictionary.Add("DryObject", pineTreeIsDryDictionary);
                            pineTreeComponentsDictionary.Add("LivingNaturalResource", pineTreeLivingNaturalResourceIsDeadDictionary);
                            pineTreeProperty.Add("Id", Guid.NewGuid().ToString());
                            pineTreeProperty.Add("Template", "Pine");
                            pineTreeProperty.Add("Components", pineTreeComponentsDictionary);
                            entitiesList.Add(pineTreeProperty);
                            pineTreesNum += 1;
                            if (pineTreesNum >= targetPineTrees)
                            {
                                return entitiesList;
                            }
                        }
                        yCounter++;
                    }
                    yCounter = 0;
                    xCounter++;
                }
            }
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetBirchTrees(int[,] map, int count, List<Dictionary<String, System.Object>> entitiesList)
        {
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetChestnutTrees(int[,] map, int count, List<Dictionary<String, System.Object>> entitiesList)
        {
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetMapleTrees(int[,] map, int count, List<Dictionary<String, System.Object>> entitiesList)
        {
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetBlueberries(int[,] map, int count, List<Dictionary<String, System.Object>> entitiesList)
        {
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetDandelions(int[,] map, int count, List<Dictionary<String, System.Object>> entitiesList)
        {
            return entitiesList;
        }
        private static List<Dictionary<String, System.Object>> GetSlopes(int[,] map, int count, List<Dictionary<String, System.Object>> entitiesList)
        {
            return entitiesList;
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
                riverSourceProperty.Add("Template", "WaterSource");
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
        private static float[,] GenerateBaseLayerNoise(int xSize, int ySize, float hills, int seed)
        {
            
            return GenerateNoiseMap(xSize, ySize, -1, 1, 0.2f * hills, TerrainNoiseType);
        }
        private static float[,] GenerateNoiseMap(int xSize, int ySize, int freqMult, int cubicScale, float amplitude, FastNoiseLite.NoiseType noiseType)
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
        private static float GenerateRawNoise(int x, int y, FastNoiseLite.NoiseType NoiseType, int FrequencyMult)
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
