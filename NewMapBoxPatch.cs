using HarmonyLib;
using Timberborn.CoreUI;
using Timberborn.MapRepositorySystemUI;
using UnityEngine.UIElements;
using TimberApi.DependencyContainerSystem;
using TimberApi.UIBuilderSystem;
using TimberApi.UIPresets.Builders;
using TimberApi.UIBuilderSystem.ElementBuilders;
using System;
using TimberApi.UIPresets.Buttons;
using TimberApi.UIPresets.Labels;
using Bindito.Core;

namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(NewMapBox), "GetPanel")]
    public static class NewMapBoxPatch {
        private static LocalizableButton settings = null;
        private static void Postfix(NewMapBox __instance, VisualElement __result) 
        {
            var builder = DependencyContainer.GetInstance<UIBuilder>();
            var root = __result;
            if (!root.Contains(settings))
            {
                settings = builder.Create<MenuButton>().Build();
                settings.text = "Random Map";
                settings.clicked += ShowSettingsBox;
                root.Add(settings);
            }
        }

        private static void ShowSettingsBox() 
        {
            var panelStack = DependencyContainer.GetInstance<PanelStack>();
            var randomMapSettingsBox = DependencyContainer.GetInstance<RandomMapSettingsBox>();
            panelStack.HideAndPush(randomMapSettingsBox);
        }
    }
}