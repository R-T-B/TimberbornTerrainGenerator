using System;
using System.Collections.Generic;
using System.Text;
using Timberborn.Core;
using UnityEngine;

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
                int x = NewMapPatch.rand.Next(0, NewMapPatch.mapSizeX - (bufferZone + 1));
                int y = NewMapPatch.rand.Next(0, NewMapPatch.mapSizeY - (bufferZone + 1));
                int z = map[y, x];
                int testX = 0;
                int testY = 0;
                bool tryAnotherLocation = false;
                while (testY <= bufferZone)
                {
                    if ((z != map[y + testY, x + testX]) || NewMapPatch.EntityMapper[y + testY, x + testX] || NewMapPatch.RiverMapper[y + testY, x + testX])
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
                    NewMapPatch.EntityMapper[y + testY, x + testX] = true;
                    testX++;
                    if (testX > bufferZone)
                    {
                        testX = 0;
                        testY++;
                    }
                }
                entitiesList.Add(mineProperty);
                NewMapPatch.EntityMapper[y, x] = true; //Gotta register that entity...
                minesNum++;
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetRuins(int[,] map, int targetRuins, List<Dictionary<string, object>> entitiesList)
        {
            float[,] ruinsMap = new float[NewMapPatch.mapSizeX, NewMapPatch.mapSizeY];
            //Place some Ruins!
            int ruinsNum = 0;
            ruinsMap = NewMapPatch.GenerateNoiseMap(NewMapPatch.mapSizeX, NewMapPatch.mapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = Utilities.GetFloatArrayMax(ruinsMap);
            float modifier = 1.00f;
            float prevalence;
            while (ruinsNum < targetRuins)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < NewMapPatch.mapSizeX)
                {
                    while (yCounter < NewMapPatch.mapSizeY)
                    {
                        if ((ruinsMap[yCounter, xCounter] > prevalence) && (!NewMapPatch.RiverMapper[yCounter, xCounter]) && (!NewMapPatch.EntityMapper[yCounter, xCounter]))
                        {
                            int ruinHeight = (int)Mathf.Max(Mathf.Min((ruinsMap[yCounter, xCounter] - prevalence) * 400, 8), 1);
                            int ruinYield = ruinHeight * 15;
                            int variantInt = NewMapPatch.rand.Next(1, 6);
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
                            NewMapPatch.EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
            float[,] pineTreesMap = new float[NewMapPatch.mapSizeX, NewMapPatch.mapSizeY];
            //Place some PineTrees!
            int pineTreesNum = 0;
            pineTreesMap = NewMapPatch.GenerateNoiseMap(NewMapPatch.mapSizeX, NewMapPatch.mapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = Utilities.GetFloatArrayMax(pineTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (pineTreesNum < targetPineTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < NewMapPatch.mapSizeX)
                {
                    while (yCounter < NewMapPatch.mapSizeY)
                    {
                        if ((pineTreesMap[yCounter, xCounter] > prevalence) && (!NewMapPatch.RiverMapper[yCounter, xCounter]) && (!NewMapPatch.EntityMapper[yCounter, xCounter]))
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
                            pineTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)NewMapPatch.rand.NextDouble());
                            pineTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                            pineTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            pineTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            pineTreeNaturalRandomizerDictionary.Add("Rotation", (float)(NewMapPatch.rand.Next(0, 360) + NewMapPatch.rand.NextDouble()));
                            pineTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                            pineTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
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
                            NewMapPatch.EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
            float[,] birchTreesMap = new float[NewMapPatch.mapSizeX, NewMapPatch.mapSizeY];
            //Place some BirchTrees!
            int birchTreesNum = 0;
            birchTreesMap = NewMapPatch.GenerateNoiseMap(NewMapPatch.mapSizeX, NewMapPatch.mapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = Utilities.GetFloatArrayMax(birchTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (birchTreesNum < targetBirchTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < NewMapPatch.mapSizeX)
                {
                    while (yCounter < NewMapPatch.mapSizeY)
                    {
                        if ((birchTreesMap[yCounter, xCounter] > prevalence) && (!NewMapPatch.RiverMapper[yCounter, xCounter]) && (!NewMapPatch.EntityMapper[yCounter, xCounter]))
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
                            birchTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            birchTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            birchTreeNaturalRandomizerDictionary.Add("Rotation", (float)(NewMapPatch.rand.Next(0, 360) + NewMapPatch.rand.NextDouble()));
                            birchTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                            birchTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
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
                            NewMapPatch.EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
            float[,] chestnutTreesMap = new float[NewMapPatch.mapSizeX, NewMapPatch.mapSizeY];
            //Place some ChestnutTrees!
            int chestnutTreesNum = 0;
            chestnutTreesMap = NewMapPatch.GenerateNoiseMap(NewMapPatch.mapSizeX, NewMapPatch.mapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = Utilities.GetFloatArrayMax(chestnutTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (chestnutTreesNum < targetChestnutTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < NewMapPatch.mapSizeX)
                {
                    while (yCounter < NewMapPatch.mapSizeY)
                    {
                        if ((chestnutTreesMap[yCounter, xCounter] > prevalence) && (!NewMapPatch.RiverMapper[yCounter, xCounter]) && (!NewMapPatch.EntityMapper[yCounter, xCounter]))
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
                            chestnutTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)NewMapPatch.rand.NextDouble());
                            chestnutTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                            chestnutTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            chestnutTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            chestnutTreeNaturalRandomizerDictionary.Add("Rotation", (float)(NewMapPatch.rand.Next(0, 360) + NewMapPatch.rand.NextDouble()));
                            chestnutTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                            chestnutTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
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
                            NewMapPatch.EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
            float[,] mapleTreesMap = new float[NewMapPatch.mapSizeX, NewMapPatch.mapSizeY];
            //Place some MapleTrees!
            int mapleTreesNum = 0;
            mapleTreesMap = NewMapPatch.GenerateNoiseMap(NewMapPatch.mapSizeX, NewMapPatch.mapSizeY, 25, 1, 0.8f, FastNoiseLite.NoiseType.Perlin);

            float maxH = Utilities.GetFloatArrayMax(mapleTreesMap);
            float modifier = 1.00f;
            float prevalence;
            while (mapleTreesNum < targetMapleTrees)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < NewMapPatch.mapSizeX)
                {
                    while (yCounter < NewMapPatch.mapSizeY)
                    {
                        if ((mapleTreesMap[yCounter, xCounter] > prevalence) && (!NewMapPatch.RiverMapper[yCounter, xCounter]) && (!NewMapPatch.EntityMapper[yCounter, xCounter]))
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
                            mapleTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)NewMapPatch.rand.NextDouble());
                            mapleTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                            mapleTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            mapleTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                            mapleTreeNaturalRandomizerDictionary.Add("Rotation", (float)(NewMapPatch.rand.Next(0, 360) + NewMapPatch.rand.NextDouble()));
                            mapleTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                            mapleTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
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
                            NewMapPatch.EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
            float[,] blueberriesNoiseMap = new float[NewMapPatch.mapSizeX, NewMapPatch.mapSizeY];
            int numBlueberries = 0;
            blueberriesNoiseMap = NewMapPatch.GenerateNoiseMap(NewMapPatch.mapSizeX, NewMapPatch.mapSizeY, 100, 1, 0.8f, FastNoiseLite.NoiseType.Perlin); //Really speckled noise

            float maxH = Utilities.GetFloatArrayMax(blueberriesNoiseMap);
            float modifier = 1f;
            float prevalence;
            while (numBlueberries < targetBlueberries)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < NewMapPatch.mapSizeX)
                {
                    while (yCounter < NewMapPatch.mapSizeY)
                    {
                        if (NewMapPatch.RiverMapper[yCounter, xCounter] && (blueberriesNoiseMap[yCounter, xCounter] > prevalence))
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
                                if (((walkAboutX + walkAboutRange) >= NewMapPatch.mapSizeX) || ((walkAboutY + walkAboutRange) >= NewMapPatch.mapSizeY) || ((walkAboutX + walkAboutRange) < 0) || ((walkAboutY + walkAboutRange) < 0))
                                {
                                    //We are too close to the border to plant blueberries, Charlie Brown.  Beyond here be dragons...
                                    abortPlanting = true;
                                    break;
                                }
                                if ((!NewMapPatch.RiverMapper[walkAboutY + walkAboutRange, walkAboutX]) && ((walkAboutY + walkAboutRange) < NewMapPatch.mapSizeY) && ((walkAboutY + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if ((NewMapPatch.EntityMapper[walkAboutY + walkAboutRange, walkAboutX]))
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutY += walkAboutRange;
                                    break;  //I always knew we'd make it!
                                }
                                if ((!NewMapPatch.RiverMapper[walkAboutY, walkAboutX + walkAboutRange]) && ((walkAboutX + walkAboutRange) < NewMapPatch.mapSizeX) && ((walkAboutX + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if ((NewMapPatch.EntityMapper[walkAboutY, walkAboutX + walkAboutRange]))
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutX += walkAboutRange;
                                    break; //I always knew we'd make it!
                                }
                                if ((!NewMapPatch.RiverMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange]))  //Are we there yet?
                                {
                                    if ((NewMapPatch.EntityMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange]))
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
                                NewMapPatch.EntityMapper[walkAboutY, walkAboutX] = true;
                                int z = map[walkAboutY, walkAboutX];
                                Dictionary<string, object> blueberryProperty = new Dictionary<string, object>();
                                Dictionary<string, object> blueberryTreeComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> blueberryTreeBlockComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, int> blueberryTreeBlockCoordinatesDictionary = new Dictionary<string, int>();
                                Dictionary<string, bool> blueberryTreeIsDryDictionary = new Dictionary<string, bool>();
                                Dictionary<string, float> blueberryTreeGrowableDictionary = new Dictionary<string, float>();
                                Dictionary<string, float> blueberryTreeNaturalRandomizerDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> blueberryTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                                Dictionary<string, float> blueberryTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>(); ;
                                Dictionary<string, float> blueberryTreeGatherableYieldGrowerGrowthDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> blueberryTreeYielderGatherableYieldDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> blueberryTreeYielderGatherableYieldGoodDictionary = new Dictionary<string, object>();
                                Dictionary<string, string> blueberryTreeYielderGatherableYieldGoodIdDictionary = new Dictionary<string, string>();
                                Dictionary<string, object> blueberryTreeLivingNaturalResourceDictionary = new Dictionary<string, object>();
                                Dictionary<string, bool> blueberryTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                                blueberryTreeIsDryDictionary.Add("IsDry", false);
                                blueberryTreeLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                                blueberryTreeYielderGatherableYieldGoodIdDictionary.Add("Id", "Berries");
                                blueberryTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)NewMapPatch.rand.NextDouble());
                                blueberryTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                                blueberryTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                                blueberryTreeNaturalRandomizerDictionary.Add("Rotation", (float)(NewMapPatch.rand.Next(0, 360) + NewMapPatch.rand.NextDouble()));
                                blueberryTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                                blueberryTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                                blueberryTreeGrowableDictionary.Add("GrowthProgress", 1.0f);
                                blueberryTreeBlockCoordinatesDictionary.Add("X", walkAboutX);
                                blueberryTreeBlockCoordinatesDictionary.Add("Y", walkAboutY);
                                blueberryTreeBlockCoordinatesDictionary.Add("Z", z);
                                blueberryTreeYielderGatherableYieldGoodDictionary.Add("Amount", 3);
                                blueberryTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", blueberryTreeLivingNaturalResourceIsDeadDictionary);
                                blueberryTreeYielderGatherableYieldGoodDictionary.Add("Good", blueberryTreeYielderGatherableYieldGoodIdDictionary);
                                blueberryTreeYielderGatherableYieldDictionary.Add("Yield", blueberryTreeYielderGatherableYieldGoodDictionary);
                                blueberryTreeCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", blueberryTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                                blueberryTreeBlockComponentsDictionary.Add("Coordinates", blueberryTreeBlockCoordinatesDictionary);
                                blueberryTreeComponentsDictionary.Add("BlockObject", blueberryTreeBlockComponentsDictionary);
                                blueberryTreeComponentsDictionary.Add("Growable", blueberryTreeGrowableDictionary);
                                blueberryTreeComponentsDictionary.Add("NaturalResourceModelRandomizer", blueberryTreeNaturalRandomizerDictionary);
                                blueberryTreeComponentsDictionary.Add("CoordinatesOffseter", blueberryTreeCoordinatesOffseterCoordinatesOffsetDictionary);
                                blueberryTreeComponentsDictionary.Add("GatherableYieldGrower", blueberryTreeGatherableYieldGrowerGrowthDictionary);
                                blueberryTreeComponentsDictionary.Add("Yielder:Gatherable", blueberryTreeYielderGatherableYieldDictionary);
                                blueberryTreeComponentsDictionary.Add("DryObject", blueberryTreeIsDryDictionary);
                                blueberryTreeComponentsDictionary.Add("LivingNaturalResource", blueberryTreeLivingNaturalResourceIsDeadDictionary);
                                blueberryProperty.Add("Id", Guid.NewGuid().ToString());
                                blueberryProperty.Add("Template", "BlueberryBush");
                                blueberryProperty.Add("Components", blueberryTreeComponentsDictionary);
                                entitiesList.Add(blueberryProperty);
                                NewMapPatch.EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
            float[,] dandelionsNoiseMap = new float[NewMapPatch.mapSizeX, NewMapPatch.mapSizeY];
            int numDandelions = 0;
            dandelionsNoiseMap = NewMapPatch.GenerateNoiseMap(NewMapPatch.mapSizeX, NewMapPatch.mapSizeY, 100, 1, 0.8f, FastNoiseLite.NoiseType.Perlin); //Really speckled noise

            float maxH = Utilities.GetFloatArrayMax(dandelionsNoiseMap);
            float modifier = 1f;
            float prevalence;
            while (numDandelions < targetDandelions)
            {
                modifier -= 0.01f;
                prevalence = maxH * modifier;
                int xCounter = 0;
                int yCounter = 0;
                while (xCounter < NewMapPatch.mapSizeX)
                {
                    while (yCounter < NewMapPatch.mapSizeY)
                    {
                        if (NewMapPatch.RiverMapper[yCounter, xCounter] && (dandelionsNoiseMap[yCounter, xCounter] > prevalence))
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
                                if (((walkAboutX + walkAboutRange) >= NewMapPatch.mapSizeX) || ((walkAboutY + walkAboutRange) >= NewMapPatch.mapSizeY) || ((walkAboutX + walkAboutRange) < 0) || ((walkAboutY + walkAboutRange) < 0))
                                {
                                    //We are too close to the border to plant dandelions, Charlie Brown.  Beyond here be dragons...
                                    abortPlanting = true;
                                    break;
                                }
                                if ((!NewMapPatch.RiverMapper[walkAboutY + walkAboutRange, walkAboutX]) && ((walkAboutY + walkAboutRange) < NewMapPatch.mapSizeY) && ((walkAboutY + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if ((NewMapPatch.EntityMapper[walkAboutY + walkAboutRange, walkAboutX]))
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutY += walkAboutRange;
                                    break;  //I always knew we'd make it!
                                }
                                if ((!NewMapPatch.RiverMapper[walkAboutY, walkAboutX + walkAboutRange]) && ((walkAboutX + walkAboutRange) < NewMapPatch.mapSizeX) && ((walkAboutX + walkAboutRange) >= 0))  //Are we there yet?
                                {
                                    if ((NewMapPatch.EntityMapper[walkAboutY, walkAboutX + walkAboutRange]))
                                    {
                                        //We've been here before you dummy. We have to keep looking!
                                        triedAtLeastOnce = true;
                                        continue;
                                    }
                                    walkAboutX += walkAboutRange;
                                    break; //I always knew we'd make it!
                                }
                                if ((!NewMapPatch.RiverMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange]))  //Are we there yet?
                                {
                                    if ((NewMapPatch.EntityMapper[walkAboutY + walkAboutRange, walkAboutX + walkAboutRange]))
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
                                NewMapPatch.EntityMapper[walkAboutY, walkAboutX] = true;
                                int z = map[walkAboutY, walkAboutX];
                                Dictionary<string, object> dandelionProperty = new Dictionary<string, object>();
                                Dictionary<string, object> dandelionTreeComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> dandelionTreeBlockComponentsDictionary = new Dictionary<string, object>();
                                Dictionary<string, int> dandelionTreeBlockCoordinatesDictionary = new Dictionary<string, int>();
                                Dictionary<string, bool> dandelionTreeIsDryDictionary = new Dictionary<string, bool>();
                                Dictionary<string, float> dandelionTreeGrowableDictionary = new Dictionary<string, float>();
                                Dictionary<string, float> dandelionTreeNaturalRandomizerDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> dandelionTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                                Dictionary<string, float> dandelionTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>(); ;
                                Dictionary<string, float> dandelionTreeGatherableYieldGrowerGrowthDictionary = new Dictionary<string, float>();
                                Dictionary<string, object> dandelionTreeYielderGatherableYieldDictionary = new Dictionary<string, object>();
                                Dictionary<string, object> dandelionTreeYielderGatherableYieldGoodDictionary = new Dictionary<string, object>();
                                Dictionary<string, string> dandelionTreeYielderGatherableYieldGoodIdDictionary = new Dictionary<string, string>();
                                Dictionary<string, object> dandelionTreeLivingNaturalResourceDictionary = new Dictionary<string, object>();
                                Dictionary<string, bool> dandelionTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                                dandelionTreeIsDryDictionary.Add("IsDry", false);
                                dandelionTreeLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                                dandelionTreeYielderGatherableYieldGoodIdDictionary.Add("Id", "Dandelion");
                                dandelionTreeGatherableYieldGrowerGrowthDictionary.Add("GrowthProgress", (float)NewMapPatch.rand.NextDouble());
                                dandelionTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                                dandelionTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)NewMapPatch.rand.NextDouble() - 0.5f);
                                dandelionTreeNaturalRandomizerDictionary.Add("Rotation", (float)(NewMapPatch.rand.Next(0, 360) + NewMapPatch.rand.NextDouble()));
                                dandelionTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                                dandelionTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + NewMapPatch.rand.Next(1, 30) / 100.0));
                                dandelionTreeGrowableDictionary.Add("GrowthProgress", 1.0f);
                                dandelionTreeBlockCoordinatesDictionary.Add("X", walkAboutX);
                                dandelionTreeBlockCoordinatesDictionary.Add("Y", walkAboutY);
                                dandelionTreeBlockCoordinatesDictionary.Add("Z", z);
                                dandelionTreeYielderGatherableYieldGoodDictionary.Add("Amount", 1);
                                dandelionTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", dandelionTreeLivingNaturalResourceIsDeadDictionary);
                                dandelionTreeYielderGatherableYieldGoodDictionary.Add("Good", dandelionTreeYielderGatherableYieldGoodIdDictionary);
                                dandelionTreeYielderGatherableYieldDictionary.Add("Yield", dandelionTreeYielderGatherableYieldGoodDictionary);
                                dandelionTreeCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", dandelionTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                                dandelionTreeBlockComponentsDictionary.Add("Coordinates", dandelionTreeBlockCoordinatesDictionary);
                                dandelionTreeComponentsDictionary.Add("BlockObject", dandelionTreeBlockComponentsDictionary);
                                dandelionTreeComponentsDictionary.Add("Growable", dandelionTreeGrowableDictionary);
                                dandelionTreeComponentsDictionary.Add("NaturalResourceModelRandomizer", dandelionTreeNaturalRandomizerDictionary);
                                dandelionTreeComponentsDictionary.Add("CoordinatesOffseter", dandelionTreeCoordinatesOffseterCoordinatesOffsetDictionary);
                                dandelionTreeComponentsDictionary.Add("GatherableYieldGrower", dandelionTreeGatherableYieldGrowerGrowthDictionary);
                                dandelionTreeComponentsDictionary.Add("Yielder:Gatherable", dandelionTreeYielderGatherableYieldDictionary);
                                dandelionTreeComponentsDictionary.Add("DryObject", dandelionTreeIsDryDictionary);
                                dandelionTreeComponentsDictionary.Add("LivingNaturalResource", dandelionTreeLivingNaturalResourceIsDeadDictionary);
                                dandelionProperty.Add("Id", Guid.NewGuid().ToString());
                                dandelionProperty.Add("Template", "Dandelion");
                                dandelionProperty.Add("Components", dandelionTreeComponentsDictionary);
                                entitiesList.Add(dandelionProperty);
                                NewMapPatch.EntityMapper[yCounter, xCounter] = true; //Gotta register that entity...
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
        public static List<Dictionary<string, object>> GetSlopes(int[,] map, int count, List<Dictionary<string, object>> entitiesList)
        {
            return entitiesList;
        }
    }
}
