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
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapEditorSceneLoading;
using UnityEngine;
using UnityEngine.UIElements;

namespace TimberbornTerrainGenerator
{
	[HarmonyPatch(typeof(MapEditorSceneLoader))]
	[HarmonyPatch("StartNewMap")]
	class NewMapPatch
	{
        //Overrides starting new map with generating some hardcoded map into "newMap.json" file and then loading it
        public static bool Prefix(Vector2Int mapSize, MapEditorSceneLoader __instance)
		{
            bool launched = false;
			Statics.Logger.LogInfo("Creating new randomised map");
            if (File.Exists(Statics.PluginPath + "/dist/" + "newMap.json"))
            {
                File.Delete(Statics.PluginPath + "/dist/" + "newMap.json");
            }
            if (!launched)
            {
                if ((Application.platform == RuntimePlatform.WindowsPlayer))
                {
                    new Process
                    {
                        StartInfo =
            {
                UseShellExecute = false,
                WorkingDirectory = Statics.PluginPath + "/dist/",
                FileName = Statics.PluginPath + "/dist/terrainGenerator.exe",
                Arguments = mapSize.x.ToString() + " " + mapSize.y.ToString(),
                CreateNoWindow = true
            }
                    }.Start();
                }
                else
                {
                    new Process
                    {
                        StartInfo =
            {
                UseShellExecute = false,
                WorkingDirectory = Statics.PluginPath + "/dist/",
                FileName = "python3",
                Arguments = "terrainGenerator.py" + " " + mapSize.x.ToString() + " " + mapSize.y.ToString(),
                CreateNoWindow = true
            }
                    }.Start();
                }
                launched = true;
            }
            while (!File.Exists(Statics.PluginPath + "/dist/" + "newMap.json"))
            {
                Thread.Sleep(500);
            }
            Statics.Logger.LogInfo("Loading randomised map");
            MapFileReference mapFileReference = MapFileReference.FromDisk(Statics.PluginPath + "/dist/" + "newMap");
            __instance.LoadMap(mapFileReference);
			Statics.Logger.LogInfo("Finished loading");
            return false;
		}
    }
    /*[HarmonyPatch(typeof(NewMapBox))]
    [HarmonyPatch("GetPanel")]
    class GetPanelPatch
    {
        public static VisualElement Prefix(NewMapBox __instance)
        {
            VisualElement visualElement = this._visualElementLoader.LoadVisualElement("Options/NewMapBox");
            visualElement.Q("StartButton", null).clicked += this.StartNewMap;
            visualElement.Q("CancelButton", null).clicked += this.OnUICancelled;
            this._sizeXField = visualElement.Q("SizeXField", null);
            this._sizeYField = visualElement.Q("SizeYField", null);
            this._sizeXField.value = NewMapBox.DefaultMapSize.x.ToString();
            this._sizeYField.value = NewMapBox.DefaultMapSize.y.ToString();
            return visualElement;
            __instance.
        }
    }*/
    [BepInPlugin("org.bepinex.plugins.timberbornterraingenerator", "TimberbornTerrainGenerator", "0.3.2")]
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
