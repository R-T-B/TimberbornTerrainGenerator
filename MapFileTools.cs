﻿using System;
using System.Collections.Generic;
using System.IO;
using static TimberbornTerrainGenerator.Statics;
using static TimberbornTerrainGenerator.RandomMapSettingsBox;
using System.Text;
using System.Linq;
using UnityEngine;
using System.IO.Compression;

namespace TimberbornTerrainGenerator
{
    public class MapFileTools
    {
        public static string fileName = PluginPath + "/newMap.timber";
        public static void SaveTerrainMap(int[,] map, int x, int y, List<Dictionary<string, object>> jsonEntities)
        {
            string heightMap = "";
            string scalarMap = "";
            string flowMap = "";
            GenerateArrayStrings(map, out heightMap, out scalarMap, out flowMap);
            MapFileFormat mapFile = new MapFileFormat();
            mapFile.GameVersion = Application.version;
            mapFile.Timestamp = DateTime.UtcNow.ToString().Replace("/","-"); //"2022-08-12 18:46:19"; style note, hence the replace for the slashes.  Time is UTC.
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
            //Now we start building our world header, inverted of course...
            mapSizeVector2.Add("X", y);
            mapSizeVector2.Add("Y", x);
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
            //zip & cleanup
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            using (ZipArchive zip = ZipFile.Open(fileName, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(PluginPath + "/newMap.json", "world.json");
            }
            File.Delete(PluginPath + "/newMap.json");
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
        public static void GenerateArrayStrings(int[,] map, out string heightMap, out string scalarMap, out string flowMap)
        {
            var stringBuilder = new StringBuilder();
            heightMap = "";
            scalarMap = string.Concat(Enumerable.Repeat("0 ", map.Length));
            flowMap = string.Concat(Enumerable.Repeat("0:0:0:0 ", map.Length)); ;
            int x = 0;
            int y = 0;
            while (y < MapSizeY)
            {
                while (x < MapSizeX)
                {
                    stringBuilder.Append(map[x, y].ToString() + " ");
                    y++;
                    if (y == MapSizeY)
                    {
                        x++;
                        if (x == MapSizeX)
                        {
                            continue;
                        }
                        y = 0;
                    }
                }
            }
            heightMap = stringBuilder.ToString();
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
