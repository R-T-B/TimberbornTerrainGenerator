using System;
using System.IO;
using System.Reflection;
using TimberApi.ConsoleSystem;
using UnityEngine;

namespace TimberbornTerrainGenerator
{
    public static class Statics
	{
		public static IConsoleWriter Logger;
        private static string pluginPath;
        public const string PluginVersion = "1.6.3";
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
        public static string ConfigPath
        {
            get
            {
                string configPath = PluginPath;
                try
                {
                    while (!Directory.Exists(configPath + "/config"))
                    {
                        configPath = configPath + "/../";
                    }
                }
                catch
                {
                    Debug.LogError("Unable to find a config folder!"); //Fail?
                    return "";
                }
                configPath = configPath + "/config/TimberbornTerrainGenerator";
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }
                return configPath;
            }
        }
    }
}
