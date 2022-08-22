using BepInEx;
using HarmonyLib;
using static TimberbornTerrainGenerator.Statics;

namespace TimberbornTerrainGenerator
{
    [BepInPlugin("TimberbornTerrainGenerator", "TimberbornTerrainGenerator", PluginVersion)]
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
            new Harmony("TimberbornTerrainGenerator").PatchAll();
        }
    }

}
