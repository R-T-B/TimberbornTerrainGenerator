using BepInEx;
using HarmonyLib;
using System.Reflection;
using Timberborn.MapSystemUI;
using TimberbornAPI;
using TimberbornAPI.Common;
using static TimberbornTerrainGenerator.Statics;

namespace TimberbornTerrainGenerator
{
    [BepInPlugin("TimberbornTerrainGenerator", "TimberbornTerrainGenerator", PluginVersion)]
    [BepInDependency("com.timberapi.timberapi")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            TimberAPI.DependencyRegistry.AddConfigurator(new TimberbornTerrainGeneratorConfigurator(), 
                SceneEntryPoint.MainMenu);
            TimberAPI.DependencyRegistry.AddConfigurator(new TimberbornTerrainGeneratorConfigurator(),
                SceneEntryPoint.MapEditor);
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
            maxsize.SetValue(null, 384);
            minsize.SetValue(null, 32);
            new Harmony("TimberbornTerrainGenerator").PatchAll();
        }
    }
}