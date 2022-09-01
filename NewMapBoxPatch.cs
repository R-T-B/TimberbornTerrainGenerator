using System;
using HarmonyLib;
using Timberborn.Core;
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
            var settings = builder.Presets().Buttons().Button(null, text: "Random map");
            wrapper.Add(settings);
            
            var root = __result.Q<VisualElement>("NewMapPanel");
            var createButton = __result.Q<LocalizableButton>("StartButton");
            createButton.text = "Create New Empty Map";
            root.Add(wrapper);
            root.style.height = 240f;
            settings.clicked += ShowSettingsBox;
            
        }

        private static void ShowSettingsBox() 
        {
            var panelStack = TimberAPI.DependencyContainer.GetInstance<PanelStack>();
            var randomMapSettingsBox = TimberAPI.DependencyContainer.GetInstance<RandomMapSettingsBox>();
            panelStack.HideAndPush(randomMapSettingsBox);
        }
    }
}