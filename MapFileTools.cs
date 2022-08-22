using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static TimberbornTerrainGenerator.Statics;

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
            string fileName = PluginPath + "/newMap.json";
            MapFileFormat mapFile = new MapFileFormat();
            mapFile.GameVersion = "0.2.4.1-9edd51d-xsw";
            mapFile.Timestamp = "2022-08-12 18:46:19";
            //Lets declare this dictionary mess.
            Dictionary<string, object> mapSizeDictLevel1 = new Dictionary<string, object>();
            Dictionary<string, int> mapSizeVector2 = new Dictionary<string, int>();
            Dictionary<string, object> TerrainMapDictLevel1 = new Dictionary<string, object>();
            Dictionary<string, string> heightArrayMapDict = new Dictionary<string, string>();
            Dictionary<string, object> soilMoistureDictLevel1 = new Dictionary<string, object>();
            Dictionary<string, string> moistureArrayMapDict = new Dictionary<string, string>();
            Dictionary<string, object> cameraDictLevel1 = new Dictionary<string, object>();
            Dictionary<string, object> cameraArrayMapDict = new Dictionary<string, object>();
            Dictionary<string, double> cameraVector3 = new Dictionary<string, double>();
            Dictionary<string, object> waterDepthsDictLevel1 = new Dictionary<string, object>();
            Dictionary<string, string> waterDepthsArrayDict = new Dictionary<string, string>();
            Dictionary<string, string> waterOutflowsDict = new Dictionary<string, string>();
            //Now we start building our world header
            mapSizeVector2.Add("X", x);
            mapSizeVector2.Add("Y", y);
            mapSizeDictLevel1.Add("Size", mapSizeVector2);
            //Now TerrainMap Heights
            heightArrayMapDict.Add("Array", heightMap);
            TerrainMapDictLevel1.Add("Heights", heightArrayMapDict);
            //Now we work on SoilMoistureSimulator
            moistureArrayMapDict.Add("Array", scalarMap);
            soilMoistureDictLevel1.Add("MoistureLevels", moistureArrayMapDict);

            //Now we work on CameraStateRestorer

            cameraVector3.Add("X", 0.0f);
            cameraVector3.Add("Y", 0.0f);
            cameraVector3.Add("Z", 0.0f);
            cameraArrayMapDict.Add("Target", cameraVector3);
            cameraArrayMapDict.Add("ZoomLevel", 0.0f);
            cameraArrayMapDict.Add("HorizontalAngle", 30.0f);
            cameraArrayMapDict.Add("VerticalAngle", 70.0f);
            cameraDictLevel1.Add("SavedCameraState", cameraArrayMapDict);

            //Now we work on WaterMap WaterDepths
            waterDepthsArrayDict.Add("Array", scalarMap);
            waterDepthsDictLevel1.Add("WaterDepths", waterDepthsArrayDict);
            waterOutflowsDict.Add("Array", flowMap);
            waterDepthsDictLevel1.Add("Outflows", waterOutflowsDict);

            mapFile.Singletons.Add("MapSize", mapSizeDictLevel1);
            mapFile.Singletons.Add("TerrainMap", TerrainMapDictLevel1);
            mapFile.Singletons.Add("SoilMoistureSimulator", soilMoistureDictLevel1);
            mapFile.Singletons.Add("CameraStateRestorer", cameraDictLevel1);
            mapFile.Singletons.Add("WaterMap", waterDepthsDictLevel1);
            mapFile.Entities = jsonEntities;
            //Now we work on Entities, with a new variable in the class to manage them.
            // serialize JSON to a string and then write string to a file

            File.WriteAllText(PluginPath + "/newMap.json", Newtonsoft.Json.JsonConvert.SerializeObject(mapFile));
            //cleanup
            mapSizeDictLevel1 = null;
            mapSizeVector2 = null;
            TerrainMapDictLevel1 = null;
            heightArrayMapDict = null;
            soilMoistureDictLevel1 = null;
            moistureArrayMapDict = null;
            cameraDictLevel1 = null;
            cameraArrayMapDict = null;
            cameraVector3 = null;
            waterDepthsDictLevel1 = null;
            waterDepthsArrayDict = null;
            waterOutflowsDict = null;
            mapFile.Entities = null;
            mapFile.Singletons = null;
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
