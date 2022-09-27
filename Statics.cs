using System;
using System.IO;
using System.Reflection;
using TimberApi.ConsoleSystem;

namespace TimberbornTerrainGenerator
{
    public static class Statics
	{
		public static IConsoleWriter Logger;
        private static string pluginPath;
        public const string PluginVersion = "1.4.1";
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
