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
        public const string PluginVersion = "1.8.2";
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
                string legacyConfigPath = PluginPath;
                string configPath = Timberborn.Core.UserDataFolder.Folder + "/Maps/TimberbornTerrainGeneratorProfiles";
                try
                {
                    while (!Directory.Exists(legacyConfigPath + "/config"))
                    {
                        legacyConfigPath = legacyConfigPath + "/../";
                    }
                    legacyConfigPath = legacyConfigPath + "/config/TimberbornTerrainGenerator";
                    if (Directory.Exists(legacyConfigPath) && (!Directory.Exists(configPath)))
                    {
                        DirectoryInfo legacyDir = new DirectoryInfo(legacyConfigPath);
                        Directory.CreateDirectory(configPath);
                        DirectoryInfo configDir = new DirectoryInfo(configPath);
                        foreach (FileInfo file in legacyDir.GetFiles())
                        {
                            string targetFilePath = Path.Combine(configPath, file.Name);
                            file.MoveTo(targetFilePath);
                        }
                        Directory.Delete(legacyConfigPath, true);
                    }
                }
                catch
                {
                    Debug.LogError("Unable to find a config folder!"); //Fail?
                    return "";
                }
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }
                return configPath;
            }
        }
    }
}
