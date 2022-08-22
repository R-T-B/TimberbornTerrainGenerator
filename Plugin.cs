using BepInEx;
using HarmonyLib;
using System.Reflection;
using Timberborn.MapSystemUI;
using static TimberbornTerrainGenerator.Statics;

namespace TimberbornTerrainGenerator
{
    [BepInPlugin("TimberbornTerrainGenerator", "TimberbornTerrainGenerator", PluginVersion)]
    [BepInProcess("Timberborn.exe")]
    //[BepInDependency("com.timberapi.timberapi")]
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
            var maxsize = typeof(NewMapBox).GetField("MaxMapSize", BindingFlags.Static | BindingFlags.NonPublic);
            var minsize = typeof(NewMapBox).GetField("MinMapSize", BindingFlags.Static | BindingFlags.NonPublic);
            maxsize.SetValue(null, (int)int.MaxValue);
            minsize.SetValue(null, (int)int.MinValue);
            new Harmony("TimberbornTerrainGenerator").PatchAll();
        }
    }

}