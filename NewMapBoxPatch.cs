using System;
using HarmonyLib;
using Timberborn.CoreUI;
using Timberborn.MapEditorSceneLoading;
using Timberborn.MapSystemUI;
using TimberbornAPI;
using TimberbornAPI.UIBuilderSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(NewMapBox), "GetPanel")]
    public static class NewMapBoxPatch {
        
        private static void Postfix(NewMapBox __instance, VisualElement __result) 
        {
            var builder = TimberAPI.DependencyContainer.GetInstance<UIBuilder>();

            var wrapper = new VisualElement 
            {
                style = 
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1,
                    justifyContent = Justify.Center,
                    paddingTop = new StyleLength(8f)
                }
            };

            var generate = builder.Presets().Buttons().Button(name: "generate", text: "Generate random map");
            var settings = builder.Presets().Buttons().Button(name: "settings", text: "Random map settings");

            wrapper.Add(generate);
            wrapper.Add(settings);
            
            var root = __result.Q<VisualElement>("NewMapPanel");
            root.Add(wrapper);
            root.style.height = 240f;

            generate.clicked += () => GenerateRandomMap(__instance);
            settings.clicked += ShowSettingsBox;
            
        }

        private static void GenerateRandomMap(NewMapBox newMapBox)
        {
            newMapBox._sizeYField.text = newMapBox._sizeXField.text;
            var randomMapSettingsBox = TimberAPI.DependencyContainer.GetInstance<RandomMapSettingsBox>();
            randomMapSettingsBox.LoadINISettings();
            newMapBox.StartNewMap();
        }

        private static void ShowSettingsBox() 
        {
            var panelStack = TimberAPI.DependencyContainer.GetInstance<PanelStack>();
            var randomMapSettingsBox = TimberAPI.DependencyContainer.GetInstance<RandomMapSettingsBox>();
            panelStack.HideAndPush(randomMapSettingsBox);
        }
    }
}