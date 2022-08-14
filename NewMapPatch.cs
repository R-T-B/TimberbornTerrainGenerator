using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timberborn.Core;
using Timberborn.EntitySystem;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapSystem;
using Timberborn.MapSystemUI;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace TimberbornTerrainGenerator
{
	[HarmonyPatch(typeof(MapEditorSceneLoader))]
	[HarmonyPatch("StartNewMap")]
	class NewMapPatch
	{
        private static string pluginPath;
        public static string PluginPath
        {
            get
            {
                if (ReferenceEquals(null, pluginPath))
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    pluginPath = Uri.UnescapeDataString(uri.Path);
                    pluginPath = Path.GetDirectoryName(pluginPath);
                }
                return pluginPath;
            }
        }
        //Overrides starting new map with generating some hardcoded map into "tmp.json" file and then loading it
        public static bool Prefix(Vector2Int mapSize, MapEditorSceneLoader __instance)
		{
            bool launched = false;
			Statics.Logger.LogInfo("Creating new randomised map");
            if (File.Exists(PluginPath + "/dist/" + "newMap.json"))
            {
                File.Delete(PluginPath + "/dist/" + "newMap.json");
            }
            if (!launched)
            {
                new Process
                {
                    StartInfo =
            {
                UseShellExecute = false,
                WorkingDirectory = PluginPath + "/dist/",
                FileName = PluginPath + "/dist/terrainGenerator.exe",
                Arguments = mapSize.x.ToString() + " " + mapSize.y.ToString(),
                CreateNoWindow = true
            }
                }.Start();
                launched = true;
            }
            while (!File.Exists(PluginPath + "/dist/" + "newMap.json"))
            {
                Thread.Sleep(500);
            }
            Statics.Logger.LogInfo("Loading randomised map");
            MapFileReference mapFileReference = MapFileReference.FromDisk(PluginPath + "/dist/" + "newMap");
            __instance.LoadMap(mapFileReference);
			Statics.Logger.LogInfo("Finished loading");
            return false;
		}
    }
    [BepInPlugin("org.bepinex.plugins.timberbornterraingenerator", "TimberbornTerrainGenerator", "0.1.4")]
    public class TimberbornTerrainGeneratorPlugin : BaseUnityPlugin
    {
        public void Awake()
        {
            var size = typeof(NewMapBox).GetField("MaxMapSize", BindingFlags.Static | BindingFlags.NonPublic);
            size.SetValue(null, (int)Int32.MaxValue);
        }
    }
}
