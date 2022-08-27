using System;
using System.Collections.Generic;
using UnityEngine;
using static TimberbornTerrainGenerator.TerrainGen;
using static TimberbornTerrainGenerator.Utilities;
using static TimberbornTerrainGenerator.SettingsUI;

namespace TimberbornTerrainGenerator
{
    public class EntityManager
    {
        public static bool[,] EntityMapper = new bool[32, 32];
        public static List<Dictionary<string, object>> PlaceEntities(int[,] map, List<Dictionary<string, object>> entitiesList)
        {
            float mapScaler = MapSizeX * MapSizeY / 65536f;
            float mineMapScaler = mapScaler;
            int scaledSlopeCount = (int)Math.Round(SlopeCount * mapScaler);
            if ((mineMapScaler < 0.255) && (!(mineMapScaler < 0.23))) //This deals with float imprecision cheating 128x128 players out of their one default mine.
            {
                mineMapScaler = 0.255f;
            }
            int scaledMinesCount = (int)Math.Round(MaxMineCount * mineMapScaler);
            int scaledRuinsCount = (int)Math.Round(RuinCount * mapScaler);
            int scaledPineTreeCount = (int)Math.Round(PineTreeCount * mapScaler);
            int scaledBirchTreeCount = (int)Math.Round(BirchTreeCount * mapScaler);
            int scaledChestnutTreeCount = (int)Math.Round(ChestnutTreeCount * mapScaler);
            int scaledMapleTreeCount = (int)Math.Round(MapleTreeCount * mapScaler);
            int scaledBlueberriesCount = (int)Math.Round(BlueberryBushCount * mapScaler);
            int scaledDandelionsCount = (int)Math.Round(DandelionBushCount * mapScaler);


            if (scaledMinesCount < MinMineCount)
            {
                scaledMinesCount = MinMineCount;
            }
            //Setup the counter variables:
            int ruinsNum = 0;
            int pineTreesNum = 0;
            int birchTreesNum = 0;
            int chestnutTreesNum = 0;
            int mapleTreesNum = 0;
            int blueberriesNum = 0;
            int dandelionsNum = 0;
            int xCounter = 0;
            int yCounter = 0;
            //Setup the noise maps with their own independent random data
            noise.SetSeed(Seed);
            float[,] ruinsNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 0.8f, FastNoiseLite.NoiseType.Perlin);
            noise.SetSeed(Seed + 25);
            float[,] pineTreesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 0.8f, FastNoiseLite.NoiseType.Perlin);
            noise.SetSeed(Seed + 50);
            float[,] birchTreesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 0.8f, FastNoiseLite.NoiseType.Perlin);
            noise.SetSeed(Seed + 75);
            float[,] chestnutTreesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 0.8f, FastNoiseLite.NoiseType.Perlin);
            noise.SetSeed(Seed + 100);
            float[,] mapleTreesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 0.8f, FastNoiseLite.NoiseType.Perlin);
            noise.SetSeed(Seed + 125);
            float[,] blueberriesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 100, 0.8f, FastNoiseLite.NoiseType.Perlin); //Really speckled noise
            noise.SetSeed(Seed + 150);
            float[,] dandelionsNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 100, 0.8f, FastNoiseLite.NoiseType.Perlin); //Really speckled noise
            //Setup the other misc variables
            float ruinsMaxH = GetFloatArrayMax(ruinsNoiseMap);
            float pineTreesMaxH = GetFloatArrayMax(pineTreesNoiseMap);
            float birchTreesMaxH = GetFloatArrayMax(birchTreesNoiseMap);
            float chestnutTreesMaxH = GetFloatArrayMax(chestnutTreesNoiseMap);
            float mapleTreesMaxH = GetFloatArrayMax(mapleTreesNoiseMap);
            float blueberriesMaxH = GetFloatArrayMax(blueberriesNoiseMap);
            float dandelionsMaxH = GetFloatArrayMax(dandelionsNoiseMap);

            float modifier = 1.00f;
            float ruinsPrevalence;
            float pineTreesPrevalence;
            float birchTreesPrevalence;
            float chestnutTreesPrevalence;
            float mapleTreesPrevalence;
            float blueberriesPrevalence;
            float dandelionsPrevalence;

            //First place slopes and mines, they are their own thing as they don't need an expensive map loop.
            entitiesList = GetSlopes(map, scaledSlopeCount, entitiesList);
            entitiesList = GetMines(map, scaledMinesCount, entitiesList);

            //Lets begin our massive "other" entity loop, now consolidated into one thing.  We keep going until done!
            while ((ruinsNum < scaledRuinsCount) || (pineTreesNum < scaledPineTreeCount) || (birchTreesNum < scaledBirchTreeCount) || (chestnutTreesNum < scaledChestnutTreeCount) || (mapleTreesNum < scaledMapleTreeCount) || (blueberriesNum < scaledBlueberriesCount) || (dandelionsNum < scaledDandelionsCount))
            {
                modifier -= 0.01f;
                ruinsPrevalence = ruinsMaxH * modifier;
                pineTreesPrevalence = pineTreesMaxH * modifier;
                birchTreesPrevalence = birchTreesMaxH * modifier;
                chestnutTreesPrevalence = chestnutTreesMaxH * modifier;
                mapleTreesPrevalence = mapleTreesMaxH * modifier;
                blueberriesPrevalence = blueberriesMaxH * modifier;
                dandelionsPrevalence = dandelionsMaxH * modifier;
                //Begin map loop
                while (xCounter < MapSizeX - 1)
                {
                    while (yCounter < MapSizeY - 1)
                    {
                        //NOTE: Order of placement priority in the event of a conflict is as follows: Slopes (already placed), Mines (already placed),Ruins, Pines, Birches, Chestnuts,Maples, Blueberries, and Dandelions.  We may want to sort this by quantity someday.
                        yCounter++;
                        //We need to count if we spawned something or not, so we track how many entities there are and compare later.
                        int entitiesListCounter = entitiesList.Count;
                        if (!(ruinsNum >= scaledRuinsCount))
                        {
                            entitiesList = EntityManager.GetRuins(map, entitiesList, xCounter, yCounter, ruinsNoiseMap, ruinsPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                ruinsNum++;
                            }
                        }
                        if (!(pineTreesNum >= scaledPineTreeCount))
                        {
                            entitiesList = EntityManager.GetPineTrees(map, entitiesList, xCounter, yCounter, pineTreesNoiseMap, pineTreesPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                pineTreesNum++;
                            }
                        }
                        if (!(birchTreesNum >= scaledBirchTreeCount))
                        {
                            entitiesList = EntityManager.GetBirchTrees(map, entitiesList, xCounter, yCounter, birchTreesNoiseMap, birchTreesPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                birchTreesNum++;
                            }
                        }
                        if (!(chestnutTreesNum >= scaledChestnutTreeCount))
                        {
                            entitiesList = EntityManager.GetChestnutTrees(map, entitiesList, xCounter, yCounter, chestnutTreesNoiseMap, chestnutTreesPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                chestnutTreesNum++;
                            }
                        }
                        if (!(mapleTreesNum >= scaledMapleTreeCount))
                        {
                            entitiesList = EntityManager.GetMapleTrees(map, entitiesList, xCounter, yCounter, mapleTreesNoiseMap, mapleTreesPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                mapleTreesNum++;
                            }
                        }
                        if (!(blueberriesNum >= scaledBlueberriesCount))
                        {
                            entitiesList = EntityManager.GetBlueberries(map, entitiesList, xCounter, yCounter, blueberriesNoiseMap, blueberriesPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                blueberriesNum++;
                            }
                        }
                        if (!(dandelionsNum >= scaledDandelionsCount))
                        {
                            entitiesList = EntityManager.GetDandelions(map, entitiesList, xCounter, yCounter, dandelionsNoiseMap, dandelionsPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                dandelionsNum++;
                            }
                        }
                        yCounter++;
                    }
                    yCounter = 0;
                    xCounter++;
                }
                xCounter = 0; //reset
                yCounter = 0; //reset
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetSlopes(int[,] map, int targetSlopes, List<Dictionary<string, object>> entitiesList)
        {
            //Lets place some slopes!
            int slopesNum = 0;
            int attemptedTimes = 0;
            int abortTimeframe = ((MapSizeX + MapSizeY) / 2) * 4;
            while ((slopesNum < targetSlopes) && (attemptedTimes < abortTimeframe))  //If we couldn't produce in over 4 random walks of the average of the map dimension, it ain't happening.  Abort.
            {
                bool tryAnotherLocation = false;
                int x = rand.Next(MapSizeX - 8);
                int y = rand.Next(MapSizeY - 8);
                int z = 0; //we'll figure this out later.
                int testValue = 0;
                //First off, where?  Lets wander and find a potential spot.
                int startingZ = map[x, y];
                bool HorizontalOrVertical = false;
                bool whichWay = false;
                if (rand.Next(0, 2) == 0)
                {
                    HorizontalOrVertical = true;
                }
                if (rand.Next(0, 2) == 0)
                {
                    whichWay = true;
                }
                if (!HorizontalOrVertical)
                {
                    while (startingZ == map[x + testValue, y])
                    {
                        if (x + testValue >= (MapSizeX - 1)) //We've walked too close to map edge...
                        {
                            attemptedTimes++;
                            tryAnotherLocation = true;
                            break;
                        }
                        if (x + testValue <= 1) //We've walked too close to map edge...
                        {
                            attemptedTimes++;
                            tryAnotherLocation = true;
                            break;
                        }
                        if (whichWay)
                        {
                            testValue += 1;
                        }
                        else
                        {
                            testValue -= 1;
                        }
                    }
                }
                else
                {
                    while (startingZ == map[x, y + testValue])
                    {
                        if (y + testValue >= (MapSizeY - 1)) //We've walked too close to map edge...
                        {
                            attemptedTimes++;
                            tryAnotherLocation = true;
                            break;
                        }
                        if (y + testValue <= 1) //We've walked too close to map edge...
                        {
                            attemptedTimes++;
                            tryAnotherLocation = true;
                            break;
                        }
                        if (whichWay)
                        {
                            testValue += 1;
                        }
                        else
                        {
                            testValue -= 1;
                        }
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
                if (x >= (MapSizeX - 2)) //We've walked too close to map edge...
                {
                    attemptedTimes++;
                    tryAnotherLocation = true;
                    continue;
                }
                if (x <= 0) //We've walked too close to map edge...
                {
                    attemptedTimes++;
                    tryAnotherLocation = true;
                    continue;
                }
                if (y >= (MapSizeY - 2)) //We've walked too close to map edge...
                {
                    attemptedTimes++;
                    tryAnotherLocation = true;
                    continue;
                }
                if (y <= 0) //We've walked too close to map edge...
                {
                    attemptedTimes++;
                    tryAnotherLocation = true;
                    continue;
                }
                int endingZ = map[x, y];
                if ((startingZ > endingZ && ((startingZ - endingZ) > 1)) || (startingZ < endingZ && ((endingZ - startingZ) > 1))) //Slope is impossibly steep
                {
                    attemptedTimes++;
                    tryAnotherLocation = true;
                    continue;
                }
                if ((!HorizontalOrVertical) && (map[x - 1, y] != startingZ && map[x + 1, y] != startingZ))
                {
                    attemptedTimes++;
                    tryAnotherLocation = true;
                    continue;
                }
                else if ((HorizontalOrVertical) && (map[x, y - 1] != startingZ && map[x, y + 1] != startingZ))
                {
                    attemptedTimes++;
                    tryAnotherLocation = true;
                    continue;
                }
                if (EntityMapper[x, y]) //Something is already placed here...
                {
                    tryAnotherLocation = true;
                }
                if (RiverMapper[x, y]) //Not in the river...
                {
                    tryAnotherLocation = true;
                }
                if (tryAnotherLocation) //Something went wrong, bail out and try again...
                {
                    attemptedTimes++;
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
                        if (whichWay)
                        {
                            x--;
                        }
                        else
                        {
                            x++;
                        }
                    }
                    else
                    {
                        if (whichWay)
                        {
                            y--;
                        }
                        else
                        {
                            y++;
                        }
                    }
                }
                //Okay, the coords are established!  We are ready!  First what way does the slope spin?
                string slopeDirection = "NOROTATION"; ; //this is kept only if it doesn't need rotation data.
                if (whichWay) //There are two rotation logics that we need...  They are inverts of each other.
                {
                    if ((HorizontalOrVertical) && (startingZ > endingZ))
                    {
                        slopeDirection = "Cw90";
                    }
                    else if ((HorizontalOrVertical) && (startingZ < endingZ))
                    {
                        slopeDirection = "Cw270";
                    }
                    else if (!HorizontalOrVertical && (startingZ > endingZ))
                    {
                        slopeDirection = "NOROTATION"; //Dummy Entry to keep the logical flow, no rotation needed.
                    }
                    else if (!HorizontalOrVertical && (startingZ < endingZ))
                    {
                        slopeDirection = "Cw180";
                    }
                }
                else
                {
                    if ((HorizontalOrVertical) && (startingZ > endingZ))
                    {
                        slopeDirection = "Cw270";
                    }
                    else if ((HorizontalOrVertical) && (startingZ < endingZ))
                    {
                        slopeDirection = "Cw90";
                    }
                    else if (!HorizontalOrVertical && (startingZ > endingZ))
                    {
                        slopeDirection = "Cw180";
                    }
                    else if (!HorizontalOrVertical && (startingZ < endingZ))
                    {
                        slopeDirection = "NOROTATION"; //Dummy Entry to keep the logical flow, no rotation needed.
                    }
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
                slopeBlockCoordinatesDictionary.Add("X", y);
                slopeBlockCoordinatesDictionary.Add("Y", x);
                slopeBlockCoordinatesDictionary.Add("Z", map[x, y]);
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
                slopeProperty.Add("Id", Guid.NewGuid().ToString());
                slopeProperty.Add("Template", "Slope");
                slopeProperty.Add("Components", slopeComponentsDictionary);
                entitiesList.Add(slopeProperty);
                EntityMapper[x, y] = true; //Gotta register that entity...
                attemptedTimes++;
                slopesNum++;
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetMines(int[,] map, int targetMines, List<Dictionary<string, object>> entitiesList)
        {
            //Place some mines!
            int minesNum = 0;
            int attemptedTimes = 0;
            int abortTimeframe = ((MapSizeX + MapSizeY) / 2) * 4;
            while ((minesNum < targetMines) && (attemptedTimes < abortTimeframe))
            {
                int bufferZone = 4; //this needs to be at least 4 to fit a mine
                int x = rand.Next(0, MapSizeX - (bufferZone + 1));
                int y = rand.Next(0, MapSizeY - (bufferZone + 1));
                int z = map[x, y];
                int testX = 0;
                int testY = 0;
                bool tryAnotherLocation = false;
                while (testY <= bufferZone)
                {
                    if ((z != map[x + testX,y + testY]) || EntityMapper[x + testX, y + testY] || RiverMapper[x + testX, y + testY])
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
                    attemptedTimes++;
                    continue;
                }
                Dictionary<string, object> mineProperty = new Dictionary<string, object>();
                Dictionary<string, object> mineComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, object> mineBlockComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, int> mineBlockCoordinatesDictionary = new Dictionary<string, int>();
                Dictionary<string, bool> mineIsDryDictionary = new Dictionary<string, bool>();
                mineBlockCoordinatesDictionary.Add("X", y);
                mineBlockCoordinatesDictionary.Add("Y", x);
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
                    EntityMapper[x + testX, y + testY] = true;
                    testX++;
                    if (testX > bufferZone)
                    {
                        testX = 0;
                        testY++;
                    }
                }
                entitiesList.Add(mineProperty);
                EntityMapper[x, y] = true; //Gotta register that entity...
                minesNum++;
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetRuins(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] ruinsMap, float prevalence)
        {
            if ((ruinsMap[xCounter, yCounter] > prevalence) && (!RiverMapper[xCounter, yCounter]) && (!EntityMapper[xCounter, yCounter]))
            {
                int ruinHeight = (int)Mathf.Max(Mathf.Min((ruinsMap[xCounter, yCounter] - prevalence) * 400, 8), 1);
                int ruinYield = ruinHeight * 15;
                int variantInt = rand.Next(1, 6);
                int z = map[xCounter, yCounter];
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
                ruinBlockCoordinatesDictionary.Add("X", yCounter);
                ruinBlockCoordinatesDictionary.Add("Y", xCounter);
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
                EntityMapper[xCounter, yCounter] = true; //Gotta register that entity...
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetPineTrees(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] pineTreesMap, float prevalence)
        {
            if ((pineTreesMap[xCounter, yCounter] > prevalence) && (!RiverMapper[xCounter, yCounter]) && (!EntityMapper[xCounter, yCounter]))
            {
                int z = map[xCounter, yCounter];
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
                pineTreeBlockCoordinatesDictionary.Add("X", yCounter);
                pineTreeBlockCoordinatesDictionary.Add("Y", xCounter);
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
                EntityMapper[xCounter, yCounter] = true; //Gotta register that entity...
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetBirchTrees(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] birchTreesMap, float prevalence)
        {
            if ((birchTreesMap[xCounter, yCounter] > prevalence) && (!RiverMapper[xCounter, yCounter]) && (!EntityMapper[xCounter, yCounter]))
            {
                int z = map[xCounter, yCounter];
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
                birchTreeBlockCoordinatesDictionary.Add("X", yCounter);
                birchTreeBlockCoordinatesDictionary.Add("Y", xCounter);
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
                EntityMapper[xCounter, yCounter] = true; //Gotta register that entity...
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetChestnutTrees(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] chestnutTreesMap, float prevalence)
        {
            if ((chestnutTreesMap[xCounter, yCounter] > prevalence) && (!RiverMapper[xCounter, yCounter]) && (!EntityMapper[xCounter, yCounter]))
            {
                int z = map[xCounter, yCounter];
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
                chestnutTreeBlockCoordinatesDictionary.Add("X", yCounter);
                chestnutTreeBlockCoordinatesDictionary.Add("Y", xCounter);
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
                EntityMapper[xCounter, yCounter] = true; //Gotta register that entity...
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetMapleTrees(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] mapleTreesMap, float prevalence)
        {
            if ((mapleTreesMap[xCounter, yCounter] > prevalence) && (!RiverMapper[xCounter, yCounter]) && (!EntityMapper[xCounter, yCounter]))
            {
                int z = map[xCounter, yCounter];
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
                mapleTreeBlockCoordinatesDictionary.Add("X", yCounter);
                mapleTreeBlockCoordinatesDictionary.Add("Y", xCounter);
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
                EntityMapper[xCounter, yCounter] = true; //Gotta register that entity...
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetBlueberries(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] blueberriesNoiseMap, float prevalence)
        {
            if (RiverMapper[xCounter, yCounter] && (blueberriesNoiseMap[xCounter, yCounter] > prevalence))
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
                    if (((walkAboutX + walkAboutRange) >= MapSizeY) || ((walkAboutY + walkAboutRange) >= MapSizeX) || ((walkAboutX + walkAboutRange) < 0) || ((walkAboutY + walkAboutRange) < 0))
                    {
                        //We are too close to the border to plant blueberries, Charlie Brown.  Beyond here be dragons...
                        abortPlanting = true;
                        break;
                    }
                    if ((!RiverMapper[walkAboutX + walkAboutRange, walkAboutY]) && ((walkAboutY + walkAboutRange) < MapSizeX) && ((walkAboutY + walkAboutRange) >= 0))  //Are we there yet?
                    {
                        if (EntityMapper[walkAboutX + walkAboutRange, walkAboutY])
                        {
                            //We've been here before you dummy. We have to keep looking!
                            triedAtLeastOnce = true;
                            continue;
                        }
                        walkAboutY += walkAboutRange;
                        break;  //I always knew we'd make it!
                    }
                    if ((!RiverMapper[walkAboutX + walkAboutRange,walkAboutY]) && ((walkAboutX + walkAboutRange) < MapSizeY) && ((walkAboutX + walkAboutRange) >= 0))  //Are we there yet?
                    {
                        if (EntityMapper[walkAboutX + walkAboutRange,walkAboutY])
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
                    EntityMapper[walkAboutX, walkAboutY] = true;
                    int z = map[walkAboutX, walkAboutY];
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
                    blueberryBlockCoordinatesDictionary.Add("X", walkAboutY);
                    blueberryBlockCoordinatesDictionary.Add("Y", walkAboutX);
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
                    EntityMapper[walkAboutX, walkAboutY] = true; //Gotta register that entity...
                }
            }
            return entitiesList;
        }
        public static List<Dictionary<string, object>> GetDandelions(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] dandelionsNoiseMap, float prevalence)
        {
            if (RiverMapper[xCounter, yCounter] && (dandelionsNoiseMap[xCounter, yCounter] > prevalence))
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
                    if ((!RiverMapper[walkAboutX + walkAboutRange, walkAboutY]) && ((walkAboutY + walkAboutRange) < MapSizeY) && ((walkAboutY + walkAboutRange) >= 0))  //Are we there yet?
                    {
                        if (EntityMapper[walkAboutX + walkAboutRange, walkAboutY])
                        {
                            //We've been here before you dummy. We have to keep looking!
                            triedAtLeastOnce = true;
                            continue;
                        }
                        walkAboutY += walkAboutRange;
                        break;  //I always knew we'd make it!
                    }
                    if ((!RiverMapper[walkAboutX + walkAboutRange,walkAboutY]) && ((walkAboutX + walkAboutRange) < MapSizeX) && ((walkAboutX + walkAboutRange) >= 0))  //Are we there yet?
                    {
                        if (EntityMapper[walkAboutX + walkAboutRange,walkAboutY])
                        {
                            //We've been here before you dummy. We have to keep looking!
                            triedAtLeastOnce = true;
                            continue;
                        }
                        walkAboutX += walkAboutRange;
                        break; //I always knew we'd make it!
                    }
                    if (!RiverMapper[walkAboutX + walkAboutRange, walkAboutY + walkAboutRange])  //Are we there yet?
                    {
                        if (EntityMapper[walkAboutX + walkAboutRange, walkAboutY + walkAboutRange])
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
                    EntityMapper[walkAboutX, walkAboutY] = true;
                    int z = map[walkAboutX, walkAboutY];
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
                    dandelionBlockCoordinatesDictionary.Add("X", walkAboutY);
                    dandelionBlockCoordinatesDictionary.Add("Y", walkAboutX);
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
                    EntityMapper[walkAboutX, walkAboutY] = true; //Gotta register that entity...
                }
            }
            return entitiesList;
        }
    }
}
