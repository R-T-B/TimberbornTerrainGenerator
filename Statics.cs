using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Timberborn.MapRepositorySystem;

namespace TimberbornTerrainGenerator
{
    public static class Statics
	{
        private static string pluginPath = null;
        public const string PluginVersion = "1.9.0";
        public static string PluginPath
        {
            get
            {
                if (ReferenceEquals(null, pluginPath))
                {
                    string codeBase = Path.Combine(Timberborn.PlatformUtilities.UserDataFolder.Folder, "Mods/TimerbornTerrainGenerator/");
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
                string configPath = Path.Combine(MapRepository.UserMapsDirectory, "TimberbornTerrainGeneratorProfiles");
                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }
                return configPath;
            }
        }
    }
}
