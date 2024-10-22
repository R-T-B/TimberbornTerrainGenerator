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
        static LocalizableButton settings = null;
        private static void Postfix(NewMapBox __instance, VisualElement __result) 
        {
            var builder = DependencyContainer.GetInstance<UIBuilder>();

            var root = __result;
            if (settings == null)
            {
                settings = builder.Create<MenuButton>().Build();
                settings.text = "Random Map";
                root.Add(settings);
            }
            var createButton = __result.Q<LocalizableButton>("StartButton");
            createButton.text = "Create New Empty Map";
            root.style.height = 240f;
            settings.clicked += ShowSettingsBox;
        }

        private static void ShowSettingsBox() 
        {
            var panelStack = DependencyContainer.GetInstance<PanelStack>();
            var randomMapSettingsBox = DependencyContainer.GetInstance<RandomMapSettingsBox>();
            panelStack.HideAndPush(randomMapSettingsBox);
        }
    }
}