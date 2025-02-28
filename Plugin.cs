using System.Reflection;
using static TimberbornTerrainGenerator.Statics;
using HarmonyLib;
using Timberborn.ModManagerScene;
using Timberborn.MapRepositorySystemUI;

namespace TimberbornTerrainGenerator
{
    public class Plugin : IModStarter
    {
        public void StartMod(IModEnvironment modEnvironment)
        {
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