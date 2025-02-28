using System;
using System.Collections.Generic;
using UnityEngine;
using static TimberbornTerrainGenerator.TerrainGen;
using static TimberbornTerrainGenerator.Utilities;
using static TimberbornTerrainGenerator.RandomMapSettingsBox;

namespace TimberbornTerrainGenerator
{
    //This class is a readability mess I have yet to fix.  Caveat Emptor.
    public class EntityManager
    {
        public static bool[,] EntityMapper = new bool[32, 32];
        public static List<Dictionary<string, object>> PlaceEntities(int[,] map, List<Dictionary<string, object>> entitiesList)
        {
            float mapScaler = MapSizeX * MapSizeY / 65536f;
            float mineMapScaler = mapScaler;
            int scaledSlopeCount = (int)Math.Round(SlopeCount * mapScaler);
            if ((mineMapScaler < 0.255) && (!(mineMapScaler < 0.23))) //This deals with float imprecision cheating 128x128 players out of their one default mine/badwater.
            {
                mineMapScaler = 0.255f;
            }
            int scaledMinesCount = (int)Math.Round(MaxMineCount * mineMapScaler);
            int scaledBadWaterCount = (int)Math.Round(BadWaterCount * mineMapScaler);
            int scaledRuinsCount = (int)Math.Round(RuinCount * mapScaler);
            int scaledPineTreeCount = (int)Math.Round(PineTreeCount * mapScaler);
            int scaledBirchTreeCount = (int)Math.Round(BirchTreeCount * mapScaler);
            int scaledOakTreeCount = (int)Math.Round(OakTreeCount * mapScaler);
            int scaledBlueberriesCount = (int)Math.Round(BlueberryBushCount * mapScaler);
            if (scaledMinesCount < MinMineCount)
            {
                scaledMinesCount = MinMineCount;
            }
            //Setup the counter variables:
            int ruinsNum = 0;
            int pineTreesNum = 0;
            int birchTreesNum = 0;
            int oakTreesNum = 0;
            int blueberriesNum = 0;
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
            float[,] oakTreesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 25, 0.8f, FastNoiseLite.NoiseType.Perlin);
            noise.SetSeed(Seed + 100);
            float[,] blueberriesNoiseMap = GenerateNoiseMap(MapSizeX, MapSizeY, 100, 0.8f, FastNoiseLite.NoiseType.Perlin); //Really speckled noise
            noise.SetSeed(Seed + 125);
            //Setup the other misc variables
            float ruinsMaxH = GetFloatArrayMax(ruinsNoiseMap);
            float pineTreesMaxH = GetFloatArrayMax(pineTreesNoiseMap);
            float birchTreesMaxH = GetFloatArrayMax(birchTreesNoiseMap);
            float oakTreesMaxH = GetFloatArrayMax(oakTreesNoiseMap);
            float blueberriesMaxH = GetFloatArrayMax(blueberriesNoiseMap);

            float modifier = 1.00f;
            float ruinsPrevalence;
            float pineTreesPrevalence;
            float birchTreesPrevalence;
            float oakTreesPrevalence;
            float blueberriesPrevalence;

            //First place slopes and mines, they are their own thing as they don't need an expensive map loop.
            entitiesList = GetSlopes(map, scaledSlopeCount, entitiesList);
            entitiesList = GetMines(map, scaledMinesCount, entitiesList);
            entitiesList = GetBadwaters(map, scaledBadWaterCount, entitiesList);

            //Lets begin our massive "other" entity loop, now consolidated into one thing.  We keep going until done!
            while ((ruinsNum < scaledRuinsCount) || (pineTreesNum < scaledPineTreeCount) || (birchTreesNum < scaledBirchTreeCount) || (oakTreesNum < scaledOakTreeCount) || (blueberriesNum < scaledBlueberriesCount))
            {
                modifier -= 0.01f;
                ruinsPrevalence = ruinsMaxH * modifier;
                pineTreesPrevalence = pineTreesMaxH * modifier;
                birchTreesPrevalence = birchTreesMaxH * modifier;
                oakTreesPrevalence = oakTreesMaxH * modifier;
                blueberriesPrevalence = blueberriesMaxH * modifier;
                //Begin map loop
                while (xCounter < MapSizeX)
                {
                    while (yCounter < MapSizeY)
                    {
                        //NOTE: Order of placement priority in the event of a conflict is as follows: Slopes (already placed), Mines (already placed), Blueberries,Ruins,Pines,Birches,Oaks.  We may want to sort this by quantity someday.
                        //We need to count if we spawned something or not, so we track how many entities there are and compare later.
                        int entitiesListCounter = entitiesList.Count;
                        if (!(blueberriesNum >= scaledBlueberriesCount))
                        {
                            entitiesList = EntityManager.GetBlueberries(map, entitiesList, xCounter, yCounter, blueberriesNoiseMap, blueberriesPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                blueberriesNum++;
                            }
                        }
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
                        if (!(oakTreesNum >= scaledOakTreeCount))
                        {
                            entitiesList = EntityManager.GetOakTrees(map, entitiesList, xCounter, yCounter, oakTreesNoiseMap, oakTreesPrevalence);
                            if (entitiesListCounter < entitiesList.Count) //The count changes, we obviously spawned something!
                            {
                                entitiesListCounter = entitiesList.Count;
                                oakTreesNum++;
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
        public static List<Dictionary<string, object>> GetBadwaters(int[,] map, int targetBadwater, List<Dictionary<string, object>> entitiesList)
        {
            //Place some badwaters!
            int badWaterNum = 0;
            int attemptedTimes = 0;
            int abortTimeframe = ((MapSizeX + MapSizeY) / 2) * 4;
            while ((badWaterNum < targetBadwater) && (attemptedTimes < abortTimeframe))
            {
                int bufferZone = 2; //this needs to be at least 2x2 to fit a badwater source
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
                Dictionary<string, object> badWaterProperty = new Dictionary<string, object>();
                Dictionary<string, object> badWaterComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, object> waterSourceComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, object> badWaterBlockComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, int> badWaterBlockCoordinatesDictionary = new Dictionary<string, int>();
                badWaterBlockCoordinatesDictionary.Add("X", y);
                badWaterBlockCoordinatesDictionary.Add("Y", x);
                badWaterBlockCoordinatesDictionary.Add("Z", z);
                waterSourceComponentsDictionary.Add("SpecifiedStrength", 1.5f);
                waterSourceComponentsDictionary.Add("CurrentStrength", 1.5f);
                badWaterComponentsDictionary.Add("WaterSource", waterSourceComponentsDictionary);
                badWaterBlockComponentsDictionary.Add("Coordinates", badWaterBlockCoordinatesDictionary);
                badWaterComponentsDictionary.Add("BlockObject", badWaterBlockComponentsDictionary);
                badWaterProperty.Add("Id", Guid.NewGuid().ToString());
                badWaterProperty.Add("Template", "BadwaterSource");
                badWaterProperty.Add("Components", badWaterComponentsDictionary);
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
                entitiesList.Add(badWaterProperty);
                EntityMapper[x, y] = true; //Gotta register that entity...
                badWaterNum++;
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
        public static List<Dictionary<string, object>> GetOakTrees(int[,] map, List<Dictionary<string, object>> entitiesList, int xCounter, int yCounter, float[,] oakTreesMap, float prevalence)
        {
            if ((oakTreesMap[xCounter, yCounter] > prevalence) && (!RiverMapper[xCounter, yCounter]) && (!EntityMapper[xCounter, yCounter]))
            {
                int z = map[xCounter, yCounter];
                Dictionary<string, object> oakTreeProperty = new Dictionary<string, object>();
                Dictionary<string, object> oakTreeComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, object> oakTreeBlockComponentsDictionary = new Dictionary<string, object>();
                Dictionary<string, int> oakTreeBlockCoordinatesDictionary = new Dictionary<string, int>();
                Dictionary<string, bool> oakTreeIsDryDictionary = new Dictionary<string, bool>();
                Dictionary<string, float> oakTreeGrowableDictionary = new Dictionary<string, float>();
                Dictionary<string, float> oakTreeNaturalRandomizerDictionary = new Dictionary<string, float>();
                Dictionary<string, object> oakTreeCoordinatesOffseterCoordinatesOffsetDictionary = new Dictionary<string, object>();
                Dictionary<string, float> oakTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary = new Dictionary<string, float>();
                Dictionary<string, object> oakTreeYielderCuttableYieldDictionary = new Dictionary<string, object>();
                Dictionary<string, object> oakTreeYielderCuttableYieldGoodDictionary = new Dictionary<string, object>();
                Dictionary<string, string> oakTreeYielderCuttableYieldGoodIdDictionary = new Dictionary<string, string>();
                Dictionary<string, object> oakTreeLivingNaturalResourceDictionary = new Dictionary<string, object>();
                Dictionary<string, bool> oakTreeLivingNaturalResourceIsDeadDictionary = new Dictionary<string, bool>();
                oakTreeIsDryDictionary.Add("IsDry", false);
                oakTreeLivingNaturalResourceIsDeadDictionary.Add("IsDead", false);
                oakTreeYielderCuttableYieldGoodIdDictionary.Add("Id", "Log");
                oakTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("X", (float)rand.NextDouble() - 0.5f);
                oakTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary.Add("Y", (float)rand.NextDouble() - 0.5f);
                oakTreeNaturalRandomizerDictionary.Add("Rotation", (float)(rand.Next(0, 360) + rand.NextDouble()));
                oakTreeNaturalRandomizerDictionary.Add("DiameterScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                oakTreeNaturalRandomizerDictionary.Add("HeightScale", (float)(0.85 + rand.Next(1, 30) / 100.0));
                oakTreeGrowableDictionary.Add("GrowthProgress", 1.0f);
                oakTreeBlockCoordinatesDictionary.Add("X", yCounter);
                oakTreeBlockCoordinatesDictionary.Add("Y", xCounter);
                oakTreeBlockCoordinatesDictionary.Add("Z", z);
                oakTreeYielderCuttableYieldGoodDictionary.Add("Amount", 8);
                oakTreeLivingNaturalResourceDictionary.Add("LivingNaturalResource", oakTreeLivingNaturalResourceIsDeadDictionary);
                oakTreeYielderCuttableYieldGoodDictionary.Add("Good", oakTreeYielderCuttableYieldGoodIdDictionary);
                oakTreeYielderCuttableYieldDictionary.Add("Yield", oakTreeYielderCuttableYieldGoodDictionary);
                oakTreeCoordinatesOffseterCoordinatesOffsetDictionary.Add("CoordinatesOffset", oakTreeCoordinatesOffseterCoordinatesOffsetCoordinatesDictionary);
                oakTreeBlockComponentsDictionary.Add("Coordinates", oakTreeBlockCoordinatesDictionary);
                oakTreeComponentsDictionary.Add("BlockObject", oakTreeBlockComponentsDictionary);
                oakTreeComponentsDictionary.Add("Growable", oakTreeGrowableDictionary);
                oakTreeComponentsDictionary.Add("NaturalResourceModelRandomizer", oakTreeNaturalRandomizerDictionary);
                oakTreeComponentsDictionary.Add("CoordinatesOffseter", oakTreeCoordinatesOffseterCoordinatesOffsetDictionary);
                oakTreeComponentsDictionary.Add("Yielder:Cuttable", oakTreeYielderCuttableYieldDictionary);
                oakTreeComponentsDictionary.Add("DryObject", oakTreeIsDryDictionary);
                oakTreeComponentsDictionary.Add("LivingNaturalResource", oakTreeLivingNaturalResourceIsDeadDictionary);
                oakTreeProperty.Add("Id", Guid.NewGuid().ToString());
                oakTreeProperty.Add("Template", "Oak");
                oakTreeProperty.Add("Components", oakTreeComponentsDictionary);
                entitiesList.Add(oakTreeProperty);
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
                    if (((walkAboutX + walkAboutRange) >= MapSizeX) || ((walkAboutY + walkAboutRange) >= MapSizeY) || ((walkAboutX + walkAboutRange) < 0) || ((walkAboutY + walkAboutRange) < 0))
                    {
                        //We are too close to the border to plant blueberries, Charlie Brown.  Beyond here be dragons...
                        abortPlanting = true;
                        break;
                    }
                    if ((!RiverMapper[walkAboutX + walkAboutRange, walkAboutY]) && ((walkAboutX + walkAboutRange) < MapSizeX) && ((walkAboutX + walkAboutRange) >= 0))  //Are we there yet?
                    {
                        if (EntityMapper[walkAboutX + walkAboutRange, walkAboutY])
                        {
                            //We've been here before you dummy. We have to keep looking!
                            triedAtLeastOnce = true;
                            continue;
                        }
                        walkAboutX += walkAboutRange;
                        break;  //I always knew we'd make it!
                    }
                    if ((!RiverMapper[walkAboutX, walkAboutY + walkAboutRange]) && ((walkAboutY + walkAboutRange) < MapSizeY) && ((walkAboutY + walkAboutRange) >= 0))  //Are we there yet?
                    {
                        if (EntityMapper[walkAboutX,walkAboutY + walkAboutRange])
                        {
                            //We've been here before you dummy. We have to keep looking!
                            triedAtLeastOnce = true;
                            continue;
                        }
                        walkAboutY += walkAboutRange;
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
    }
}
