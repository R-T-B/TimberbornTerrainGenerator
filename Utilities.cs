using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SocialPlatforms;
using UnityEngine;
using static TimberbornTerrainGenerator.NewMapPatch;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace TimberbornTerrainGenerator
{
    public class Utilities
    {
        public static int ReturnScaledIntFromFloat(float value)
        {
            int finalResult = TerrainMaxHeight - TerrainMinHeight; // I mean why not set a safe default?  Should never happen, but never say never...
            int range = (TerrainMaxHeight - TerrainMinHeight) / 2; //We divide by two because to make TerrainMinHeight the absolute map floor, we need to prepare for the possibility that there may be negative values needing z space.  An even split makes sense.
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
                    finalResult = intValue;
                }
                else
                {
                    finalResult = intValue;
                }
            }
            else
            {
                value = 1 - Mathf.Abs(value);
                value = value * range + TerrainMinHeight;
                int intValue = (int)Math.Round(value);
                if (intValue < TerrainMinHeight) //If we clip...  assign the lowest possible value
                {
                    finalResult = TerrainMinHeight;
                }
                else
                {
                    finalResult = intValue;
                }
            }
            return finalResult;
        }
        public static float[,] ReturnAdditiveMap(List<float[,]> MList)
        {
            float[,] result;
            int xSize;
            int ySize;
            try
            {
                xSize = MList.FirstOrDefault().GetLength(0);
                ySize = MList.FirstOrDefault().GetLength(1);
                result = new float[xSize, ySize];
            }
            catch
            {
                result = new float[MapSizeX, MapSizeY];
                xSize = MapSizeX;
                ySize = MapSizeY;

            }
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter < xSize)
            {
                while (yCounter < ySize)
                {
                    float resultingAdditiveValue = 0;
                    foreach (float[,] map in MList)
                    {
                        resultingAdditiveValue += map[xCounter, yCounter];
                    }
                    if (resultingAdditiveValue > 1)
                    {
                        result[xCounter, yCounter] = 1;
                    }
                    else if (resultingAdditiveValue < (-1))
                    {
                        result[xCounter, yCounter] = -1;
                    }
                    else
                    {
                        result[xCounter, yCounter] = resultingAdditiveValue;
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float[,] ReturnMeanedMap(List<float[,]> MList, bool excludeZeroes)
        {
            float[,] result;
            int xSize;
            int ySize;
            try
            {
                xSize = MList.FirstOrDefault().GetLength(0);
                ySize = MList.FirstOrDefault().GetLength(1);
                result = new float[xSize, ySize];
            }
            catch
            {
                result = new float[MapSizeX, MapSizeY];
                xSize = MapSizeX;
                ySize = MapSizeY;

            }
            result = new float[xSize, ySize];
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter < xSize)
            {
                while (yCounter < ySize)
                {
                    float resultingMeanedValue = 0;
                    int divisor = 0;
                    foreach (float[,] map in MList)
                    {
                        resultingMeanedValue += map[xCounter, yCounter];
                        if (!excludeZeroes)
                        {
                            divisor++;
                        }
                        else
                        {
                            if (map[xCounter, yCounter] != 0)
                            {
                                divisor++;
                            }
                        }
                    }
                    resultingMeanedValue = resultingMeanedValue / divisor;
                    if (resultingMeanedValue > 1)
                    {
                        result[xCounter, yCounter] = 1;
                    }
                    else if (resultingMeanedValue < (-1))
                    {
                        result[xCounter, yCounter] = -1;
                    }
                    else
                    {
                        result[xCounter, yCounter] = resultingMeanedValue;
                    }

                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float[,] ReturnMaximizedMap(List<float[,]> MList)
        {
            float[,] result;
            int xSize;
            int ySize;
            try
            {
                xSize = MList.FirstOrDefault().GetLength(0);
                ySize = MList.FirstOrDefault().GetLength(1);
                result = new float[xSize, ySize];
            }
            catch
            {
                result = new float[MapSizeX, MapSizeY];
                xSize = MapSizeX;
                ySize = MapSizeY;

            }
            result = new float[xSize, ySize];
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter < xSize)
            {
                while (yCounter < ySize)
                {
                    float resultingMaximalValue = float.MinValue;
                    foreach (float[,] map in MList)
                    {
                        if (map[xCounter, yCounter] > resultingMaximalValue)
                        {
                            resultingMaximalValue = map[xCounter, yCounter];
                        }
                    }
                    if (resultingMaximalValue < 1)
                    {
                        result[xCounter, yCounter] = resultingMaximalValue;
                    }
                    else
                    {
                        result[xCounter, yCounter] = 1;
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float[,] ReturnExtremedMap(List<float[,]> MList)
        {
            float[,] result;
            int xSize;
            int ySize;
            try
            {
                xSize = MList.FirstOrDefault().GetLength(0);
                ySize = MList.FirstOrDefault().GetLength(1);
                result = new float[xSize, ySize];
            }
            catch
            {
                result = new float[MapSizeX, MapSizeY];
                xSize = MapSizeX;
                ySize = MapSizeY;

            }
            result = new float[xSize, ySize];
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter < xSize)
            {
                while (yCounter < ySize)
                {
                    float resultingValue = 0;
                    bool resultingValueIsNegative = false;
                    foreach (float[,] map in MList)
                    {
                        if (Math.Abs(map[xCounter, yCounter]) > resultingValue)
                        {
                            if (map[xCounter, yCounter] < 0)
                            {
                                resultingValueIsNegative = true;
                            }
                            else
                            {
                                resultingValueIsNegative = false;
                            }
                            resultingValue = Math.Abs(map[xCounter, yCounter]);
                        }
                    }
                    if (resultingValueIsNegative)
                    {
                        if (resultingValue > 1)
                        {
                            result[xCounter, yCounter] = resultingValue * (-1);
                        }
                        else
                        {
                            result[xCounter, yCounter] = -1;
                        }
                    }
                    else
                    {
                        if (resultingValue < 1)
                        {
                            result[xCounter, yCounter] = resultingValue;
                        }
                        else
                        {
                            result[xCounter, yCounter] = 1;
                        }
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float[,] ReturnMinimizedMap(List<float[,]> MList)
        {
            float[,] result;
            int xSize;
            int ySize;
            try
            {
                xSize = MList.FirstOrDefault().GetLength(0);
                ySize = MList.FirstOrDefault().GetLength(1);
                result = new float[xSize, ySize];
            }
            catch
            {
                result = new float[MapSizeX, MapSizeY];
                xSize = MapSizeX;
                ySize = MapSizeY;

            }
            result = new float[xSize, ySize];
            int xCounter = 0;
            int yCounter = 0;
            while (xCounter < xSize)
            {
                while (yCounter < ySize)
                {
                    float resultingMinimalValue = float.MaxValue;
                    foreach (float[,] map in MList)
                    {
                        if (map[xCounter, yCounter] < resultingMinimalValue)
                        {
                            resultingMinimalValue = map[xCounter, yCounter];
                        }
                    }
                    if (resultingMinimalValue > (-1))
                    {
                        result[xCounter, yCounter] = resultingMinimalValue;
                    }
                    else
                    {
                        result[xCounter, yCounter] = -1;
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float GetFloatArrayMax(float[,] floatArray)
        {
            int xCounter = 0;
            int yCounter = 0;
            int sizeX = floatArray.GetLength(0);
            int sizeY = floatArray.GetLength(1);
            float result = 0.0f;
            while (xCounter < sizeX)
            {
                while (yCounter < sizeY)
                {
                    if (floatArray[xCounter,yCounter] > result)
                    {
                        result = floatArray[xCounter,yCounter];
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float GetFloatArrayMin(float[,] floatArray)
        {
            int xCounter = 0;
            int yCounter = 0;
            int sizeX = floatArray.GetLength(0);
            int sizeY = floatArray.GetLength(1);
            float result = float.MaxValue;
            while (xCounter < sizeX)
            {
                while (yCounter < sizeY)
                {
                    if (floatArray[xCounter, yCounter] < result)
                    {
                        result = floatArray[xCounter, yCounter];
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float GetIntArrayMax(int[,] floatArray)
        {
            int xCounter = 0;
            int yCounter = 0;
            int sizeX = floatArray.GetLength(0);
            int sizeY = floatArray.GetLength(1);
            float result = 0.0f;
            while (xCounter < sizeX)
            {
                while (yCounter < sizeY)
                {
                    if (floatArray[xCounter, yCounter] > result)
                    {
                        result = floatArray[xCounter, yCounter];
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
        public static float GetIntArrayMin(int[,] floatArray)
        {
            int xCounter = 0;
            int yCounter = 0;
            int sizeX = floatArray.GetLength(0);
            int sizeY = floatArray.GetLength(1);
            float result = float.MaxValue;
            while (xCounter < sizeX)
            {
                while (yCounter < sizeY)
                {
                    if (floatArray[xCounter, yCounter] < result)
                    {
                        result = floatArray[xCounter, yCounter];
                    }
                    yCounter++;
                }
                yCounter = 0;
                xCounter++;
            }
            return result;
        }
    }
}
