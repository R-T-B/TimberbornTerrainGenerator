using BepInEx;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using TimberbornTerrainGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.FactionSystem;
using Timberborn.NeedSpecifications;
using Timberborn.Persistence;

namespace TimberbornTerrainGenerator
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Timberborn.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Statics.Logger = Logger;
            Patcher.DoPatching();
        }

	}
    public class Patcher
    {
        public static void DoPatching()
        {
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }

}
