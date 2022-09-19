using System.Reflection;
using Timberborn.MapSystemUI;
using TimberApi.ModSystem;
using static TimberbornTerrainGenerator.Statics;
using TimberApi.ConsoleSystem;
using HarmonyLib;

namespace TimberbornTerrainGenerator
{
    public class Plugin : IModEntrypoint
    {
        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            Statics.Logger = consoleWriter;
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