using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TimberbornTerrainGenerator
{
    public class MapFileTools
    {
        public static void SaveTerrainMap(int[,] map, int x, int y, List<Dictionary<string, object>> jsonEntities)
        {
            string heightMap = "";
            string scalarMap = "";
            string flowMap = "";
            GenerateArrayStrings(map, x, out heightMap, out scalarMap, out flowMap);
            string fileName = Statics.PluginPath + "/newMap.json";
            MapFileFormat mapFile = new MapFileFormat();
            mapFile.GameVersion = "0.2.4.1-9edd51d-xsw";
            mapFile.Timestamp = "2022-08-12 18:46:19";
            Dictionary<string, object> mapSizeDictLevel1;
            //Now we start building our world header
            mapSizeDictLevel1 = new Dictionary<string, object>();
            Dictionary<string, int> mapSizeVector2 = new Dictionary<string, int>();
            mapSizeVector2.Add("X", x);
            mapSizeVector2.Add("Y", y);
            mapSizeDictLevel1.Add("Size", mapSizeVector2);
            mapFile.Singletons.Add("MapSize", mapSizeDictLevel1);
            //Now TerrainMap Heights
            mapSizeDictLevel1 = new Dictionary<string, object>();
            Dictionary<string, object> arrayMapDict;
            arrayMapDict = new Dictionary<string, object>();
            arrayMapDict.Add("Array", heightMap.ToString());
            mapSizeDictLevel1.Add("Heights", arrayMapDict);
            mapFile.Singletons.Add("TerrainMap", mapSizeDictLevel1);
            //Now we work on SoilMoistureSimulator
            mapSizeDictLevel1 = new Dictionary<string, object>();
            arrayMapDict = new Dictionary<string, object>();
            arrayMapDict.Add("Array", scalarMap.ToString());
            mapSizeDictLevel1.Add("MoistureLevels", arrayMapDict);
            mapFile.Singletons.Add("SoilMoistureSimulator", mapSizeDictLevel1);
            //Now we work on CameraStateRestorer
            mapSizeDictLevel1 = new Dictionary<string, object>();
            arrayMapDict = new Dictionary<string, object>();
            Dictionary<string, double> cameraVector3 = new Dictionary<string, double>();
            cameraVector3.Add("X", 0.0f);
            cameraVector3.Add("Y", 0.0f);
            cameraVector3.Add("Z", 0.0f);
            arrayMapDict.Add("Target", cameraVector3);
            arrayMapDict.Add("ZoomLevel", 0.0f);
            arrayMapDict.Add("HorizontalAngle", 30.0f);
            arrayMapDict.Add("VerticalAngle", 70.0f);
            mapSizeDictLevel1.Add("SavedCameraState", arrayMapDict);
            mapFile.Singletons.Add("CameraStateRestorer", mapSizeDictLevel1);
            //Now we work on WaterMap WaterDepths
            mapSizeDictLevel1 = new Dictionary<string, object>();
            arrayMapDict = new Dictionary<string, object>();
            arrayMapDict.Add("Array", scalarMap.ToString());
            mapSizeDictLevel1.Add("WaterDepths", arrayMapDict);
            arrayMapDict = new Dictionary<string, object>();
            arrayMapDict.Add("Array", flowMap.ToString());
            mapSizeDictLevel1.Add("Outflows", arrayMapDict);
            mapFile.Singletons.Add("WaterMap", mapSizeDictLevel1);
            mapFile.Entities = jsonEntities;
            //Now we work on Entities, with a new variable in the class to manage them.
            mapSizeDictLevel1 = new Dictionary<string, object>();
            // serialize JSON to a string and then write string to a file

            File.WriteAllText(Statics.PluginPath + "/newMap.json", Newtonsoft.Json.JsonConvert.SerializeObject(mapFile));
            return;
        }
        public static void GenerateArrayStrings(int[,] map, int mapSize, out string heightMap, out string scalarMap, out string flowMap)
        {
            heightMap = "";
            scalarMap = "";
            flowMap = "";
            int x = 0;
            int y = 0;
            while (y < mapSize)
            {
                while (x < mapSize)
                {
                    heightMap = heightMap + map[x, y].ToString() + " ";
                    scalarMap = scalarMap + "0 ";
                    flowMap = flowMap + "0:0:0:0 ";
                    y++;
                    if (y == mapSize)
                    {
                        x++;
                        if (x == mapSize)
                        {
                            continue;
                        }
                        y = 0;
                    }
                }
            }
            heightMap = heightMap.Remove(heightMap.Length - 1);
            scalarMap = scalarMap.Remove(scalarMap.Length - 1);
            flowMap = flowMap.Remove(flowMap.Length - 1);
            return;
        }
    }
    public class MapFileFormat
    {
        public string GameVersion;
        public string Timestamp;

        public Dictionary<string, Dictionary<string, object>> Singletons = new Dictionary<string, Dictionary<string, object>>();
        public List<Dictionary<string, object>> Entities = new List<Dictionary<string, object>>();
    }
}
