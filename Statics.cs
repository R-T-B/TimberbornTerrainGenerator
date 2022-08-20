using System;
using System.IO;
using System.Reflection;

namespace TimberbornTerrainGenerator
{
    public static class Statics
	{
		public static BepInEx.Logging.ManualLogSource Logger;
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
    }
}
