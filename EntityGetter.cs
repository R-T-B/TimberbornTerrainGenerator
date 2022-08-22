using Bindito.Core.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Timberborn.Core;
using UnityEngine;
using static TimberbornTerrainGenerator.NewMapPatch;
using static TimberbornTerrainGenerator.Utilities;

namespace TimberbornTerrainGenerator
{
    public class EntityGetter
    {
        public static List<Dictionary<string, object>> GetMines(int[,] map, int targetMines, List<Dictionary<string, object>> entitiesList)
        {
            //Place some mines!
            int minesNum = 0;
            while (minesNum < targetMines)
            {
                int bufferZone = 4; //this needs to be at least 4 to fit a mine
                int x = rand.Next(0, MapSizeX - (bufferZone + 1));
                int y = rand.Next(0, MapSizeY - (bufferZone + 1));
                int z = map[y, x];
                int testX = 0;
                int testY = 0;
                bool tryAnotherLocation = false;
                while (testY <= bufferZone)
                {
                    if ((z != map[y + testY, x + testX]) || EntityMapper[y + testY, x + testX] || RiverMapper[y + testY, x + testX])
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
                Dictionary<string, object> mineProperty = new Dictionary<string, object>();
                Dictionary<string, object> mineComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, object> mineBlockComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, int> mineBlockCoordinatesDictionary = new Dictionary<string, int>();
                Dictionary<string, bool> mineIsDryDictionary = new Dictionary<string, bool>();
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
                    EntityMapper[y + testY, x + testX] = true;
                    testX++;
                    if (testX > bufferZone)
                    {
                        testX = 0;
                        testY++;
                    }
                }
                entitiesList.Add(mineProperty);
                EntityMapper[y, x] = true; //Gotta register that entity...
                minesNum++;
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetRuins(int[,] map, int targetRuins, List<Dictionary<string, object>> entitiesList)
        {
            float[,] ruinsMap = new float[MapSizeX, MapSizeY];
            //Place some Ruins!
            int ruinsNum = 0;
            ruinsMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = GetFloatArrayMax(ruinsMap);
            float modifier = 1.00f;
            float prevalence;
            while (ruinsNum < targetRuins)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        if ((ruinsMap[yCounter, xCounter] > prevalence) && (!RiverMapper[yCounter, xCounter]) && (!EntityMapper[yCounter, xCounter]))
                        {
                            int ruinHeight = (int)Mathf.Max(Mathf.Min((ruinsMap[yCounter, xCounter] - prevalence) * 400, 8), 1);
                            int ruinYield = ruinHeight * 15;
                            int variantInt = rand.Next(1, 6);
                            int z = map[yCounter, xCounter];
                            Dictionary<string, object> ruinProperty = new Dictionary<string, object>();
                            Dictionary<string, object> ruinComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> ruinBlockComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, int> ruinBlockCoordinatesDictionary = new Dictionary<string, int>();
                            Dictionary<string, bool> ruinIsDryDictionary = new Dictionary<string, bool>();
                            Dictionary<string, object> ruinYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> ruinYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> ruinYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, string> ruinModelsDictionary = new Dictionary<string, string>();
                            string variant = "";
                            if (variantInt == 1)
                            {
                                variant = "A";
                            }
                            else if (variantInt == 2)
                            {
                                variant = "B";
                            }
                            else if (variantInt == 3)
                            {
                                variant = "C";
                            }
                            else if (variantInt == 4)
                            {
                                variant = "D";
                            }
                            else if (variantInt == 5)
                            {
                                variant = "E";
                            }
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
                            EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
        public static List<Dictionary<string, object>> GetPineTrees(int[,] map, int targetPineTrees, List<Dictionary<string, object>> entitiesList)
        {
            float[,] pineTreesMap = new float[MapSizeX, MapSizeY];
            //Place some PineTrees!
            int pineTreesNum = 0;
            pineTreesMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = GetFloatArrayMax(pineTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (pineTreesNum < targetPineTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        if ((pineTreesMap[yCounter, xCounter] > prevalence) && (!RiverMapper[yCounter, xCounter]) && (!EntityMapper[yCounter, xCounter]))
                        {
                            int z = map[yCounter, xCounter];
                            Dictionary<string, object> pineTreeProperty = new Dictionary<string, object>();
                            Dictionary<string, object> pineTreeComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> pineTreeBlockComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, int> pineTreeBlockCoordinatesDictionary = new Dictionary<string, int>();
                            Dictionary<string, bool> pineTreeIsDryDictionary = new Dictionary<string, bool>();
                            Dictionary<string, float> pineTreeGrowableDictionary = new Dictionary<string, float>();
                            Dictionary<string, float> pineTreeNaturalRandomizerDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> pineTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                            Dictionary<string, float> pineTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> pineTreeYielderCuttableYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> pineTreeYielderCuttableYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> pineTreeYielderCuttableYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, float> pineTreeGatherableYieldGrowerGrowthDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> pineTreeYielderGatherableYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> pineTreeYielderGatherableYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> pineTreeYielderGatherableYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, object> pineTreeLivingNaturalResourceDictionary = new Dictionary<string, object>();
                            Dictionary<string, bool> pineTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
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
                            pineTreeBlockCoordinatesDictionary.Add("X", xCounter);
                            pineTreeBlockCoordinatesDictionary.Add("Y", yCounter);
                            pineTreeBlockCoordinatesDictionary.Add("Z", z);
                            pineTreeYielderCuttableYieldGoodDictionary.Add("Amount", 2);
                            pineTreeYielderGatherableYieldGoodDictionary.Add("Amount", 2);
                            pineTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", pineTreeLivingNaturalResourceIsDeadDictionary);
                            pineTreeYielderGatherableYieldGoodDictionary.Add("Good", pineTreeYielderGatherableYieldGoodIdDictionary);
                            pineTreeYielderGatherableYieldDictionary.Add("Yield", pineTreeYielderGatherableYieldGoodDictionary);
                            pineTreeYielderCuttableYieldGoodDictionary.Add("Good", pineTreeYielderCuttableYieldGoodIdDictionary);
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
                            EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
        public static List<Dictionary<string, object>> GetBirchTrees(int[,] map, int targetBirchTrees, List<Dictionary<string, object>> entitiesList)
        {
            float[,] birchTreesMap = new float[MapSizeX, MapSizeY];
            //Place some BirchTrees!
            int birchTreesNum = 0;
            birchTreesMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = GetFloatArrayMax(birchTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (birchTreesNum < targetBirchTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        if ((birchTreesMap[yCounter, xCounter] > prevalence) && (!RiverMapper[yCounter, xCounter]) && (!EntityMapper[yCounter, xCounter]))
                        {
                            int z = map[yCounter, xCounter];
                            Dictionary<string, object> birchTreeProperty = new Dictionary<string, object>();
                            Dictionary<string, object> birchTreeComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> birchTreeBlockComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, int> birchTreeBlockCoordinatesDictionary = new Dictionary<string, int>();
                            Dictionary<string, bool> birchTreeIsDryDictionary = new Dictionary<string, bool>();
                            Dictionary<string, float> birchTreeGrowableDictionary = new Dictionary<string, float>();
                            Dictionary<string, float> birchTreeNaturalRandomizerDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> birchTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                            Dictionary<string, float> birchTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> birchTreeYielderCuttableYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> birchTreeYielderCuttableYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> birchTreeYielderCuttableYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, object> birchTreeLivingNaturalResourceDictionary = new Dictionary<string, object>();
                            Dictionary<string, bool> birchTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                            birchTreeIsDryDictionary.Add("IsDry", false);
                            birchTreeLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                            birchTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                            birchTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)rand.NextDouble() - 0.5f);
                            birchTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)rand.NextDouble() - 0.5f);
                            birchTreeNaturalRandomizerDictionary.Add("Rotation", (float)(rand.Next(0, 360) + rand.NextDouble()));
                            birchTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            birchTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            birchTreeGrowableDictionary.Add("GrowthProgress", 1.0f);
                            birchTreeBlockCoordinatesDictionary.Add("X", xCounter);
                            birchTreeBlockCoordinatesDictionary.Add("Y", yCounter);
                            birchTreeBlockCoordinatesDictionary.Add("Z", z);
                            birchTreeYielderCuttableYieldGoodDictionary.Add("Amount", 1);
                            birchTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", birchTreeLivingNaturalResourceIsDeadDictionary);
                            birchTreeYielderCuttableYieldGoodDictionary.Add("Good", birchTreeYielderCuttableYieldGoodIdDictionary);
                            birchTreeYielderCuttableYieldDictionary.Add("Yield", birchTreeYielderCuttableYieldGoodDictionary);
                            birchTreeCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", birchTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                            birchTreeBlockComponentsDictionary.Add("Coordinates", birchTreeBlockCoordinatesDictionary);
                            birchTreeComponentsDictionary.Add("BlockObject", birchTreeBlockComponentsDictionary);
                            birchTreeComponentsDictionary.Add("Growable", birchTreeGrowableDictionary);
                            birchTreeComponentsDictionary.Add("NaturalResourceModelRandomizer", birchTreeNaturalRandomizerDictionary);
                            birchTreeComponentsDictionary.Add("CoordinatesOffseter", birchTreeCoordinatesOffseterCoordinatesOffsetDictionary);
                            birchTreeComponentsDictionary.Add("Yielder:Cuttable", birchTreeYielderCuttableYieldDictionary);
                            birchTreeComponentsDictionary.Add("DryObject", birchTreeIsDryDictionary);
                            birchTreeComponentsDictionary.Add("LivingNaturalResource", birchTreeLivingNaturalResourceIsDeadDictionary);
                            birchTreeProperty.Add("Id", Guid.NewGuid().ToString());
                            birchTreeProperty.Add("Template", "Birch");
                            birchTreeProperty.Add("Components", birchTreeComponentsDictionary);
                            entitiesList.Add(birchTreeProperty);
                            EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
                            birchTreesNum += 1;
                            if (birchTreesNum >= targetBirchTrees)
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
        public static List<Dictionary<string, object>> GetChestnutTrees(int[,] map, int targetChestnutTrees, List<Dictionary<string, object>> entitiesList)
        {
            float[,] chestnutTreesMap = new float[MapSizeX, MapSizeY];
            //Place some ChestnutTrees!
            int chestnutTreesNum = 0;
            chestnutTreesMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = GetFloatArrayMax(chestnutTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (chestnutTreesNum < targetChestnutTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        if ((chestnutTreesMap[yCounter, xCounter] > prevalence) && (!RiverMapper[yCounter, xCounter]) && (!EntityMapper[yCounter, xCounter]))
                        {
                            int z = map[yCounter, xCounter];
                            Dictionary<string, object> chestnutTreeProperty = new Dictionary<string, object>();
                            Dictionary<string, object> chestnutTreeComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> chestnutTreeBlockComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, int> chestnutTreeBlockCoordinatesDictionary = new Dictionary<string, int>();
                            Dictionary<string, bool> chestnutTreeIsDryDictionary = new Dictionary<string, bool>();
                            Dictionary<string, float> chestnutTreeGrowableDictionary = new Dictionary<string, float>();
                            Dictionary<string, float> chestnutTreeNaturalRandomizerDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> chestnutTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                            Dictionary<string, float> chestnutTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> chestnutTreeYielderCuttableYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> chestnutTreeYielderCuttableYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> chestnutTreeYielderCuttableYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, float> chestnutTreeGatherableYieldGrowerGrowthDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> chestnutTreeYielderGatherableYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> chestnutTreeYielderGatherableYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> chestnutTreeYielderGatherableYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, object> chestnutTreeLivingNaturalResourceDictionary = new Dictionary<string, object>();
                            Dictionary<string, bool> chestnutTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                            chestnutTreeIsDryDictionary.Add("IsDry", false);
                            chestnutTreeLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                            chestnutTreeYielderGatherableYieldGoodIdDictionary.Add("Id", "Chestnut");
                            chestnutTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)rand.NextDouble());
                            chestnutTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                            chestnutTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)rand.NextDouble() - 0.5f);
                            chestnutTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)rand.NextDouble() - 0.5f);
                            chestnutTreeNaturalRandomizerDictionary.Add("Rotation", (float)(rand.Next(0, 360) + rand.NextDouble()));
                            chestnutTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            chestnutTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            chestnutTreeGrowableDictionary.Add("GrowthProgress", 1.0f);
                            chestnutTreeBlockCoordinatesDictionary.Add("X", xCounter);
                            chestnutTreeBlockCoordinatesDictionary.Add("Y", yCounter);
                            chestnutTreeBlockCoordinatesDictionary.Add("Z", z);
                            chestnutTreeYielderCuttableYieldGoodDictionary.Add("Amount", 4);
                            chestnutTreeYielderGatherableYieldGoodDictionary.Add("Amount", 3);
                            chestnutTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", chestnutTreeLivingNaturalResourceIsDeadDictionary);
                            chestnutTreeYielderGatherableYieldGoodDictionary.Add("Good", chestnutTreeYielderGatherableYieldGoodIdDictionary);
                            chestnutTreeYielderGatherableYieldDictionary.Add("Yield", chestnutTreeYielderGatherableYieldGoodDictionary);
                            chestnutTreeYielderCuttableYieldGoodDictionary.Add("Good", chestnutTreeYielderCuttableYieldGoodIdDictionary);
                            chestnutTreeYielderCuttableYieldDictionary.Add("Yield", chestnutTreeYielderCuttableYieldGoodDictionary);
                            chestnutTreeCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", chestnutTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                            chestnutTreeBlockComponentsDictionary.Add("Coordinates", chestnutTreeBlockCoordinatesDictionary);
                            chestnutTreeComponentsDictionary.Add("BlockObject", chestnutTreeBlockComponentsDictionary);
                            chestnutTreeComponentsDictionary.Add("Growable", chestnutTreeGrowableDictionary);
                            chestnutTreeComponentsDictionary.Add("NaturalResourceModelRandomizer", chestnutTreeNaturalRandomizerDictionary);
                            chestnutTreeComponentsDictionary.Add("CoordinatesOffseter", chestnutTreeCoordinatesOffseterCoordinatesOffsetDictionary);
                            chestnutTreeComponentsDictionary.Add("Yielder:Cuttable", chestnutTreeYielderCuttableYieldDictionary);
                            chestnutTreeComponentsDictionary.Add("GatherableYieldGrower", chestnutTreeGatherableYieldGrowerGrowthDictionary);
                            chestnutTreeComponentsDictionary.Add("Yielder:Gatherable", chestnutTreeYielderGatherableYieldDictionary);
                            chestnutTreeComponentsDictionary.Add("DryObject", chestnutTreeIsDryDictionary);
                            chestnutTreeComponentsDictionary.Add("LivingNaturalResource", chestnutTreeLivingNaturalResourceIsDeadDictionary);
                            chestnutTreeProperty.Add("Id", Guid.NewGuid().ToString());
                            chestnutTreeProperty.Add("Template", "ChestnutTree");
                            chestnutTreeProperty.Add("Components", chestnutTreeComponentsDictionary);
                            entitiesList.Add(chestnutTreeProperty);
                            EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
                            chestnutTreesNum += 1;
                            if (chestnutTreesNum >= targetChestnutTrees)
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
        public static List<Dictionary<string, object>> GetMapleTrees(int[,] map, int targetMapleTrees, List<Dictionary<string, object>> entitiesList)
        {
            float[,] mapleTreesMap = new float[MapSizeX, MapSizeY];
            //Place some MapleTrees!
            int mapleTreesNum = 0;
            mapleTreesMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = GetFloatArrayMax(mapleTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (mapleTreesNum < targetMapleTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        if ((mapleTreesMap[yCounter, xCounter] > prevalence) && (!RiverMapper[yCounter, xCounter]) && (!EntityMapper[yCounter, xCounter]))
                        {
                            int z = map[yCounter, xCounter];
                            Dictionary<string, object> mapleTreeProperty = new Dictionary<string, object>();
                            Dictionary<string, object> mapleTreeComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> mapleTreeBlockComponentsDictionary = new Dictionary<string, object>();
                            Dictionary<string, int> mapleTreeBlockCoordinatesDictionary = new Dictionary<string, int>();
                            Dictionary<string, bool> mapleTreeIsDryDictionary = new Dictionary<string, bool>();
                            Dictionary<string, float> mapleTreeGrowableDictionary = new Dictionary<string, float>();
                            Dictionary<string, float> mapleTreeNaturalRandomizerDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> mapleTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                            Dictionary<string, float> mapleTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> mapleTreeYielderCuttableYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> mapleTreeYielderCuttableYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> mapleTreeYielderCuttableYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, float> mapleTreeGatherableYieldGrowerGrowthDictionary = new Dictionary<string, float>();
                            Dictionary<string, object> mapleTreeYielderGatherableYieldDictionary = new Dictionary<string, object>();
                            Dictionary<string, object> mapleTreeYielderGatherableYieldGoodDictionary = new Dictionary<string, object>();
                            Dictionary<string, string> mapleTreeYielderGatherableYieldGoodIdDictionary = new Dictionary<string, string>();
                            Dictionary<string, object> mapleTreeLivingNaturalResourceDictionary = new Dictionary<string, object>();
                            Dictionary<string, bool> mapleTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                            mapleTreeIsDryDictionary.Add("IsDry", false);
                            mapleTreeLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                            mapleTreeYielderGatherableYieldGoodIdDictionary.Add("Id", "MapleSyrup");
                            mapleTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)rand.NextDouble());
                            mapleTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                            mapleTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)rand.NextDouble() - 0.5f);
                            mapleTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)rand.NextDouble() - 0.5f);
                            mapleTreeNaturalRandomizerDictionary.Add("Rotation", (float)(rand.Next(0, 360) + rand.NextDouble()));
                            mapleTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            mapleTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                            mapleTreeGrowableDictionary.Add("GrowthProgress", 1.0f);
                            mapleTreeBlockCoordinatesDictionary.Add("X", xCounter);
                            mapleTreeBlockCoordinatesDictionary.Add("Y", yCounter);
                            mapleTreeBlockCoordinatesDictionary.Add("Z", z);
                            mapleTreeYielderCuttableYieldGoodDictionary.Add("Amount", 8);
                            mapleTreeYielderGatherableYieldGoodDictionary.Add("Amount", 3);
                            mapleTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", mapleTreeLivingNaturalResourceIsDeadDictionary);
                            mapleTreeYielderGatherableYieldGoodDictionary.Add("Good", mapleTreeYielderGatherableYieldGoodIdDictionary);
                            mapleTreeYielderGatherableYieldDictionary.Add("Yield", mapleTreeYielderGatherableYieldGoodDictionary);
                            mapleTreeYielderCuttableYieldGoodDictionary.Add("Good", mapleTreeYielderCuttableYieldGoodIdDictionary);
                            mapleTreeYielderCuttableYieldDictionary.Add("Yield", mapleTreeYielderCuttableYieldGoodDictionary);
                            mapleTreeCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", mapleTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                            mapleTreeBlockComponentsDictionary.Add("Coordinates", mapleTreeBlockCoordinatesDictionary);
                            mapleTreeComponentsDictionary.Add("BlockObject", mapleTreeBlockComponentsDictionary);
                            mapleTreeComponentsDictionary.Add("Growable", mapleTreeGrowableDictionary);
                            mapleTreeComponentsDictionary.Add("NaturalResourceModelRandomizer", mapleTreeNaturalRandomizerDictionary);
                            mapleTreeComponentsDictionary.Add("CoordinatesOffseter", mapleTreeCoordinatesOffseterCoordinatesOffsetDictionary);
                            mapleTreeComponentsDictionary.Add("Yielder:Cuttable", mapleTreeYielderCuttableYieldDictionary);
                            mapleTreeComponentsDictionary.Add("GatherableYieldGrower", mapleTreeGatherableYieldGrowerGrowthDictionary);
                            mapleTreeComponentsDictionary.Add("Yielder:Gatherable", mapleTreeYielderGatherableYieldDictionary);
                            mapleTreeComponentsDictionary.Add("DryObject", mapleTreeIsDryDictionary);
                            mapleTreeComponentsDictionary.Add("LivingNaturalResource", mapleTreeLivingNaturalResourceIsDeadDictionary);
                            mapleTreeProperty.Add("Id", Guid.NewGuid().ToString());
                            mapleTreeProperty.Add("Template", "Maple");
                            mapleTreeProperty.Add("Components", mapleTreeComponentsDictionary);
                            entitiesList.Add(mapleTreeProperty);
                            EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
                            mapleTreesNum += 1;
                            if (mapleTreesNum >= targetMapleTrees)
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
        public static List<Dictionary<string, object>> GetBlueberries(int[,] map, int targetBlueberries, List<Dictionary<string, object>> entitiesList)
        {
            float[,] blueberriesNoiseMap = new float[MapSizeX, MapSizeY];
            int numBlueberries = 0;
            blueberriesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 100, 1, 0.8f, FastNoiseLite.NoiseType.Perlin); //Really speckled noise

            float maxH = GetFloatArrayMax(blueberriesNoiseMap);
            float modifier = 1f;
            float prevalence;
            while (numBlueberries < targetBlueberries)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        if (RiverMapper[yCounter, xCounter] && (blueberriesNoiseMap[yCounter, xCounter] > prevalence))
                        {
                            //Ok!  We are in the riverbed.  We are allowed to place a cluster of upto 1 blueberry nearby, but where?  First we need to find land.  Reference the global river map.
                            int walkAboutX = xCounter;
                            int walkAboutY = yCounter;
                            int walkAboutRange = -1;
                            bool triedAtLeastOnce = false;
                            bool abortPlanting = false;
                            //Walk around the immediate vicinity. Don't stop until the rivermap tells us. Start with a negative walking direction along the axis, then switch to a positive.  We know it's in one tile.
                            while (true)
                            {
                                if (triedAtLeastOnce)
                                {
                                    //We aren't THERE yet.  I think we're going in circles!  How about we try going positive instead of negative?
                                    if (walkAboutRange < 0)
                                    {
                                        walkAboutRange = Math.Abs(walkAboutRange);
                                    }
                                    else
                                    {
                                        //What do you mean we already tried going positive instead of negative!? We're going to drown, you know that right? I hate you, Charlie Brown...
                                        //back to negative, just incremented.
                                        walkAboutRange++;
                                        walkAboutRange = walkAboutRange * (-1);
                                    }
                                }
                                if (((walkAboutX + walkAboutRange) >= MapSizeX) || ((walkAboutY + walkAboutRange) >= MapSizeY) || ((walkAboutX + walkAboutRange) < 0) || ((walkAboutY + walkAboutRange) < 0))
                                {
                                    //We are too close to the border to plant blueberries, Charlie Brown.  Beyond here be dragons...
                                    abortPlanting = true;
                                    break;
                                }
                                if ((!RiverMapper[walkAboutY + walkAboutRange, walkAboutX]) && ((walkAboutY + walkAboutRange) < MapSizeY) && ((walkAboutY + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if (EntityMapper[walkAboutY + walkAboutRange, walkAboutX])
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutY += walkAboutRange;
                                    break;  //I always knew we'd make it!
                                }
                                if ((!RiverMapper[walkAboutY, walkAboutX + walkAboutRange]) && ((walkAboutX + walkAboutRange) < MapSizeX) && ((walkAboutX + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if (EntityMapper[walkAboutY, walkAboutX + walkAboutRange])
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutX += walkAboutRange;
                                    break; //I always knew we'd make it!
                                }
                                if (!RiverMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange])  //Are we there yet?
                                {
                                    if (EntityMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange])
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutY += walkAboutRange;
                                    walkAboutX += walkAboutRange;
                                    break; //I always knew we'd make it!
                                }
                                triedAtLeastOnce = true;
                            }
                            if (!abortPlanting)
                            {
                                //At this point, Charlie Brown has perished and we are going to plant some blueberries.  Let's proceed.
                                EntityMapper[walkAboutY, walkAboutX] = true;
                                int z = map[walkAboutY, walkAboutX];
                                Dictionary<string, object> blueberryProperty = new Dictionary<string, object>();
                                Dictionary<string, object> blueberryComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> blueberryBlockComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, int> blueberryBlockCoordinatesDictionary = new Dictionary<string, int>();
                                Dictionary<string, bool> blueberryIsDryDictionary = new Dictionary<string, bool>();
                                Dictionary<string, float> blueberryGrowableDictionary = new Dictionary<string, float>();
                                Dictionary<string, float> blueberryNaturalRandomizerDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> blueberryCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                                Dictionary<string, float> blueberryCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>(); ;
                                Dictionary<string, float> blueberryGatherableYieldGrowerGrowthDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> blueberryYielderGatherableYieldDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> blueberryYielderGatherableYieldGoodDictionary = new Dictionary<string, object>();
                                Dictionary<string, string> blueberryYielderGatherableYieldGoodIdDictionary = new Dictionary<string, string>();
                                Dictionary<string, object> blueberryLivingNaturalResourceDictionary = new Dictionary<string, object>();
                                Dictionary<string, bool> blueberryLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                                blueberryIsDryDictionary.Add("IsDry", false);
                                blueberryLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                                blueberryYielderGatherableYieldGoodIdDictionary.Add("Id", "Berries");
                                blueberryGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)rand.NextDouble());
                                blueberryCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)rand.NextDouble() - 0.5f);
                                blueberryCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)rand.NextDouble() - 0.5f);
                                blueberryNaturalRandomizerDictionary.Add("Rotation", (float)(rand.Next(0, 360) + rand.NextDouble()));
                                blueberryNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                                blueberryNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                                blueberryGrowableDictionary.Add("GrowthProgress", 1.0f);
                                blueberryBlockCoordinatesDictionary.Add("X", walkAboutX);
                                blueberryBlockCoordinatesDictionary.Add("Y", walkAboutY);
                                blueberryBlockCoordinatesDictionary.Add("Z", z);
                                blueberryYielderGatherableYieldGoodDictionary.Add("Amount", 3);
                                blueberryLivingNaturalResourceDictionary.Add("LivingNaturalResource", blueberryLivingNaturalResourceIsDeadDictionary);
                                blueberryYielderGatherableYieldGoodDictionary.Add("Good", blueberryYielderGatherableYieldGoodIdDictionary);
                                blueberryYielderGatherableYieldDictionary.Add("Yield", blueberryYielderGatherableYieldGoodDictionary);
                                blueberryCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", blueberryCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                                blueberryBlockComponentsDictionary.Add("Coordinates", blueberryBlockCoordinatesDictionary);
                                blueberryComponentsDictionary.Add("BlockObject", blueberryBlockComponentsDictionary);
                                blueberryComponentsDictionary.Add("Growable", blueberryGrowableDictionary);
                                blueberryComponentsDictionary.Add("NaturalResourceModelRandomizer", blueberryNaturalRandomizerDictionary);
                                blueberryComponentsDictionary.Add("CoordinatesOffseter", blueberryCoordinatesOffseterCoordinatesOffsetDictionary);
                                blueberryComponentsDictionary.Add("GatherableYieldGrower", blueberryGatherableYieldGrowerGrowthDictionary);
                                blueberryComponentsDictionary.Add("Yielder:Gatherable", blueberryYielderGatherableYieldDictionary);
                                blueberryComponentsDictionary.Add("DryObject", blueberryIsDryDictionary);
                                blueberryComponentsDictionary.Add("LivingNaturalResource", blueberryLivingNaturalResourceIsDeadDictionary);
                                blueberryProperty.Add("Id", Guid.NewGuid().ToString());
                                blueberryProperty.Add("Template", "BlueberryBush");
                                blueberryProperty.Add("Components", blueberryComponentsDictionary);
                                entitiesList.Add(blueberryProperty);
                                EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
                                numBlueberries++;
                            }
                            if (numBlueberries >= targetBlueberries)
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
        public static List<Dictionary<string, object>> GetDandelions(int[,] map, int targetDandelions, List<Dictionary<string, object>> entitiesList)
        {
            float[,] dandelionsNoiseMap = new float[MapSizeX, MapSizeY];
            int numDandelions = 0;
            dandelionsNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 100, 1, 0.8f, FastNoiseLite.NoiseType.Perlin); //Really speckled noise

            float maxH = GetFloatArrayMax(dandelionsNoiseMap);
            float modifier = 1f;
            float prevalence;
            while (numDandelions < targetDandelions)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        if (RiverMapper[yCounter, xCounter] && (dandelionsNoiseMap[yCounter, xCounter] > prevalence))
                        {
                            //Ok!  We are in the riverbed.  We are allowed to place a cluster of upto 1 dandelion nearby, but where?  First we need to find land.  Reference the global river map.
                            int walkAboutX = xCounter;
                            int walkAboutY = yCounter;
                            int walkAboutRange = -1;
                            bool triedAtLeastOnce = false;
                            bool abortPlanting = false;
                            //Walk around the immediate vicinity. Don't stop until the rivermap tells us. Start with a negative walking direction along the axis, then switch to a positive.  We know it's in one tile.
                            while (true)
                            {
                                if (triedAtLeastOnce)
                                {
                                    //We aren't THERE yet.  I think we're going in circles!  How about we try going positive instead of negative?
                                    if (walkAboutRange < 0)
                                    {
                                        walkAboutRange = Math.Abs(walkAboutRange);
                                    }
                                    else
                                    {
                                        //What do you mean we already tried going positive instead of negative!? We're going to drown, you know that right? I hate you, Charlie Brown...
                                        //back to negative, just incremented.
                                        walkAboutRange++;
                                        walkAboutRange = walkAboutRange * (-1);
                                    }
                                }
                                if (((walkAboutX + walkAboutRange) >= MapSizeX) || ((walkAboutY + walkAboutRange) >= MapSizeY) || ((walkAboutX + walkAboutRange) < 0) || ((walkAboutY + walkAboutRange) < 0))
                                {
                                    //We are too close to the border to plant dandelions, Charlie Brown.  Beyond here be dragons...
                                    abortPlanting = true;
                                    break;
                                }
                                if ((!RiverMapper[walkAboutY + walkAboutRange, walkAboutX]) && ((walkAboutY + walkAboutRange) < MapSizeY) && ((walkAboutY + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if (EntityMapper[walkAboutY + walkAboutRange, walkAboutX])
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutY += walkAboutRange;
                                    break;  //I always knew we'd make it!
                                }
                                if ((!RiverMapper[walkAboutY, walkAboutX + walkAboutRange]) && ((walkAboutX + walkAboutRange) < MapSizeX) && ((walkAboutX + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if (EntityMapper[walkAboutY, walkAboutX + walkAboutRange])
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutX += walkAboutRange;
                                    break; //I always knew we'd make it!
                                }
                                if (!RiverMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange])  //Are we there yet?
                                {
                                    if (EntityMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange])
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutY += walkAboutRange;
                                    walkAboutX += walkAboutRange;
                                    break; //I always knew we'd make it!
                                }
                                triedAtLeastOnce = true;
                            }
                            if (!abortPlanting)
                            {
                                //At this point, Charlie Brown has perished and we are going to plant some dandelions.  Let's proceed.
                                EntityMapper[walkAboutY, walkAboutX] = true;
                                int z = map[walkAboutY, walkAboutX];
                                Dictionary<string, object> dandelionProperty = new Dictionary<string, object>();
                                Dictionary<string, object> dandelionComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> dandelionBlockComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, int> dandelionBlockCoordinatesDictionary = new Dictionary<string, int>();
                                Dictionary<string, bool> dandelionIsDryDictionary = new Dictionary<string, bool>();
                                Dictionary<string, float> dandelionGrowableDictionary = new Dictionary<string, float>();
                                Dictionary<string, float> dandelionNaturalRandomizerDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> dandelionCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                                Dictionary<string, float> dandelionCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>(); ;
                                Dictionary<string, float> dandelionGatherableYieldGrowerGrowthDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> dandelionYielderGatherableYieldDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> dandelionYielderGatherableYieldGoodDictionary = new Dictionary<string, object>();
                                Dictionary<string, string> dandelionYielderGatherableYieldGoodIdDictionary = new Dictionary<string, string>();
                                Dictionary<string, object> dandelionLivingNaturalResourceDictionary = new Dictionary<string, object>();
                                Dictionary<string, bool> dandelionLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                                dandelionIsDryDictionary.Add("IsDry", false);
                                dandelionLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                                dandelionYielderGatherableYieldGoodIdDictionary.Add("Id", "Dandelion");
                                dandelionGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)rand.NextDouble());
                                dandelionCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)rand.NextDouble() - 0.5f);
                                dandelionCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)rand.NextDouble() - 0.5f);
                                dandelionNaturalRandomizerDictionary.Add("Rotation", (float)(rand.Next(0, 360) + rand.NextDouble()));
                                dandelionNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                                dandelionNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                                dandelionGrowableDictionary.Add("GrowthProgress", 1.0f);
                                dandelionBlockCoordinatesDictionary.Add("X", walkAboutX);
                                dandelionBlockCoordinatesDictionary.Add("Y", walkAboutY);
                                dandelionBlockCoordinatesDictionary.Add("Z", z);
                                dandelionYielderGatherableYieldGoodDictionary.Add("Amount", 1);
                                dandelionLivingNaturalResourceDictionary.Add("LivingNaturalResource", dandelionLivingNaturalResourceIsDeadDictionary);
                                dandelionYielderGatherableYieldGoodDictionary.Add("Good", dandelionYielderGatherableYieldGoodIdDictionary);
                                dandelionYielderGatherableYieldDictionary.Add("Yield", dandelionYielderGatherableYieldGoodDictionary);
                                dandelionCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", dandelionCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                                dandelionBlockComponentsDictionary.Add("Coordinates", dandelionBlockCoordinatesDictionary);
                                dandelionComponentsDictionary.Add("BlockObject", dandelionBlockComponentsDictionary);
                                dandelionComponentsDictionary.Add("Growable", dandelionGrowableDictionary);
                                dandelionComponentsDictionary.Add("NaturalResourceModelRandomizer", dandelionNaturalRandomizerDictionary);
                                dandelionComponentsDictionary.Add("CoordinatesOffseter", dandelionCoordinatesOffseterCoordinatesOffsetDictionary);
                                dandelionComponentsDictionary.Add("GatherableYieldGrower", dandelionGatherableYieldGrowerGrowthDictionary);
                                dandelionComponentsDictionary.Add("Yielder:Gatherable", dandelionYielderGatherableYieldDictionary);
                                dandelionComponentsDictionary.Add("DryObject", dandelionIsDryDictionary);
                                dandelionComponentsDictionary.Add("LivingNaturalResource", dandelionLivingNaturalResourceIsDeadDictionary);
                                dandelionProperty.Add("Id", Guid.NewGuid().ToString());
                                dandelionProperty.Add("Template", "Dandelion");
                                dandelionProperty.Add("Components", dandelionComponentsDictionary);
                                entitiesList.Add(dandelionProperty);
                                EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
                                numDandelions++;
                            }
                            if (numDandelions >= targetDandelions)
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
        public static List<Dictionary<string, object>> GetSlopes(int[,] map, int targetSlopes, List<Dictionary<string, object>> entitiesList)
        {
            //Lets place some slopes!
            int slopesNum = 0;
            while (slopesNum < targetSlopes)
            {
                bool tryAnotherLocation = false;
                int x = rand.Next(MapSizeX - 8);
                int y = rand.Next(MapSizeY - 8);
                int z = 0; //we'll figure this out later.
                int testValue = 0;
                //First off, where?  Lets wander and find a potential spot.
                int startingZ = map[y, x];
                bool HorizontalOrVertical = false;
                if (rand.Next(1) == 0)
                {
                    HorizontalOrVertical = true;
                }
                if (!HorizontalOrVertical)
                {
                    while (startingZ == map[y, x + testValue])
                    {
                        if (x + testValue >= (MapSizeX - 1)) //We've walked off the map...
                        {
                            tryAnotherLocation = true;
                            break;
                        }
                        testValue += 1;
                    }
                }
                else
                {
                    while (startingZ == map[y + testValue, x])
                    {
                        if (y + testValue >= (MapSizeY - 1)) //We've walked off the map...
                        {
                            tryAnotherLocation = true;
                            break;
                        }
                        testValue += 1;
                    }
                }
                if (!HorizontalOrVertical)
                {
                    x += testValue;
                }
                else
                {
                    y += testValue;
                }
                int endingZ = map[y, x];
                if ((startingZ > endingZ && ((startingZ - endingZ) > 1)) || (startingZ < endingZ && ((endingZ - startingZ) > 1))) //Slope is impossibly steep
                {
                    tryAnotherLocation = true;
                }
                if (EntityMapper[y,x]) //Something is already placed here...
                {
                    tryAnotherLocation = true;
                }
                if (tryAnotherLocation) //Something went wrong, bail out and try again...
                {
                    continue;
                }
                //If we got this far, we can make a slope!  Hooray!  We want to start from the low ground, and go up
                if (startingZ > endingZ) //Most simple scenario
                {
                    z = endingZ;
                }
                else if (startingZ < endingZ) //Uh oh, we're going uphill.  We need to back off a step
                {
                    if (!HorizontalOrVertical)
                    {
                        x--;
                    }
                    else
                    {
                        y--;
                    }
                }
                //Okay, the coords are established!  We are ready!  First what way does the slope spin?
                string slopeDirection = "NOROTATION"; ; //this is kept only if it doesn't need rotation data.
                if ((!HorizontalOrVertical) && (startingZ > endingZ))
                {
                    slopeDirection = "Cw90";
                }
                else if ((!HorizontalOrVertical) && (startingZ < endingZ))
                {
                    slopeDirection = "Cw270";
                }
                else if (HorizontalOrVertical && (startingZ > endingZ))
                {
                    slopeDirection = "NOROTATION"; //Dummy Entry to keep the logical flow, no rotation needed.
                }
                else if (HorizontalOrVertical && (startingZ < endingZ))
                {
                    slopeDirection = "Cw180";
                }
                //Write the slope!
                Dictionary<string, object> slopeProperty = new Dictionary<string, object>();
                Dictionary<string, object> slopeComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, object> slopeBlockComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, int> slopeBlockCoordinatesDictionary = new Dictionary<string, int>();
                Dictionary<string, object> slopeBlockOrientationDictionary = new Dictionary<string, object>();
                Dictionary<string, string> slopeBlockOrientationValueDictionary = new Dictionary<string, string>();
                Dictionary<string, bool> slopeBlockComponentsConstructibleFinishedDictionary = new Dictionary<string, bool>();
                Dictionary<string, float> slopeBlockComponentsConstructionSiteBuildTimeDictionary = new Dictionary<string, float>();
                slopeBlockComponentsConstructionSiteBuildTimeDictionary.Add("BuildTimeProgressInHoursKey", 1f);
                slopeBlockCoordinatesDictionary.Add("X", x);
                slopeBlockCoordinatesDictionary.Add("Y", y);
                slopeBlockCoordinatesDictionary.Add("Z", z);
                slopeBlockComponentsDictionary.Add("Coordinates", slopeBlockCoordinatesDictionary);
                slopeBlockComponentsConstructibleFinishedDictionary.Add("Finished", true);
                if (!slopeDirection.Equals("NOROTATION"))
                {
                    slopeBlockOrientationDictionary.Add("Value", slopeDirection);
                    slopeBlockComponentsDictionary.Add("Orientation", slopeBlockOrientationDictionary);
                }
                slopeComponentsDictionary.Add("BlockObject", slopeBlockComponentsDictionary);
                slopeComponentsDictionary.Add("Constructible", slopeBlockComponentsConstructibleFinishedDictionary);
                slopeComponentsDictionary.Add("ConstructionSite", slopeBlockComponentsConstructionSiteBuildTimeDictionary);
                slopeProperty.Add("Components", slopeComponentsDictionary);
                slopeProperty.Add("Id", Guid.NewGuid().ToString());
                slopeProperty.Add("Template", "Slope");
                slopeProperty.Add("Components", slopeComponentsDictionary);
                entitiesList.Add(slopeProperty);
                EntityMapper[y, x] = true; //Gotta register that entity...
                slopesNum++;
            }
            return entitiesList;
        }
    }
}
