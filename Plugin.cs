using BepInEx;
using HarmonyLib;

namespace TimberbornTerrainGenerator
{
    [BepInPlugin("TimberbornTerrainGenerator", "TimberbornTerrainGenerator", "0.4.0")]
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
