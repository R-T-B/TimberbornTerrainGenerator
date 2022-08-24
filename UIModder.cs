using HarmonyLib;
using Timberborn.MapSystemUI;
using TimberbornAPI.UIBuilderSystem;
using TimberbornAPI;
using UnityEngine.UIElements;
using UnityEngine;

namespace TimberbornTerrainGenerator
{
    [HarmonyPatch]
    [HarmonyPatch(typeof(NewMapBox), "GetPanel")]
    public static class UIModder
    {
        /*private static void Postfix(NewMapBox __instance, VisualElement __result)
        {
            UIBuilder builder = TimberAPI.DependencyContainer.GetInstance<UIBuilder>();
            Vector2 size = new Vector2(0, 0);//we need the origin artwork.
            //__result.SetSize(size);
            // Adds a new button to the stockpile inventory fragment
            __result.Add(builder.Presets().Buttons().ButtonGame(
                "preview.stockpile.inventory.button",
                builder: buttonBuilder => buttonBuilder
                    .SetWidth(new Length(100, Length.Unit.Percent))
                    .SetHeight(new Length(25, Length.Unit.Pixel))
                    .BuildAndInitialize())
            );
            return;
        }*/
    }
}