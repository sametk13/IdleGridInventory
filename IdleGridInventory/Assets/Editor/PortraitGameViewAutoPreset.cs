#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class PortraitGameViewAutoPreset
{
    private const int Width = 1284;
    private const int Height = 2778;
    private const string PresetName = "1284x2778";

    static PortraitGameViewAutoPreset()
    {
        // Delay call to ensure Unity has initialized the GameView sizes.
        EditorApplication.delayCall += EnsurePresetAndSelect;
    }
     
    private static void EnsurePresetAndSelect()
    {
        try
        {
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singletonType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singletonType.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
            object sizesInstance = instanceProp.GetValue(null);

            var currentGroupProp = sizesType.GetProperty("currentGroupType", BindingFlags.Public | BindingFlags.Instance);
            var getGroupMethod = sizesType.GetMethod("GetGroup", BindingFlags.Public | BindingFlags.Instance);

            int groupType = (int)currentGroupProp.GetValue(sizesInstance);
            object group = getGroupMethod.Invoke(sizesInstance, new object[] { groupType });

            var groupTypeObj = group.GetType();
            var getDisplayTextsMethod = groupTypeObj.GetMethod("GetDisplayTexts");
            var getBuiltinCountMethod = groupTypeObj.GetMethod("GetBuiltinCount");
            var addCustomSizeMethod = groupTypeObj.GetMethod("AddCustomSize");
            var getCustomCountMethod = groupTypeObj.GetMethod("GetCustomCount");
            var getTotalCountMethod = groupTypeObj.GetMethod("GetTotalCount");

            string[] displayTexts = (string[])getDisplayTextsMethod.Invoke(group, null);

            int targetIndex = FindPresetIndex(displayTexts, PresetName);
            if (targetIndex < 0)
            {
                // Create new custom size
                var gameViewSizeType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
                var gameViewSizeTypeEnum = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");

                object fixedResolutionEnum = Enum.Parse(gameViewSizeTypeEnum, "FixedResolution");
                object newSize = Activator.CreateInstance(gameViewSizeType, fixedResolutionEnum, Width, Height, PresetName);

                addCustomSizeMethod.Invoke(group, new object[] { newSize });

                // Re-fetch display texts to find index
                displayTexts = (string[])getDisplayTextsMethod.Invoke(group, null);
                targetIndex = FindPresetIndex(displayTexts, PresetName);
            }

            if (targetIndex >= 0)
            {
                SetGameViewSizeIndex(targetIndex);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[PortraitGameViewAutoPreset] Could not set GameView preset. Set it manually to 1080x1920. Error: {e.Message}");
        }
    }

    private static int FindPresetIndex(string[] displayTexts, string name)
    {
        if (displayTexts == null) return -1;

        for (int i = 0; i < displayTexts.Length; i++)
        {
            if (displayTexts[i] != null && displayTexts[i].Contains(name, StringComparison.OrdinalIgnoreCase))
                return i;
        }

        return -1;
    }

    private static void SetGameViewSizeIndex(int index)
    {
        var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var window = EditorWindow.GetWindow(gameViewType);

        var selectedSizeIndexProp = gameViewType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (selectedSizeIndexProp != null)
        {
            selectedSizeIndexProp.SetValue(window, index);
            window.Repaint();
            return;
        }

        var selectedSizeIndexField = gameViewType.GetField("m_SelectedSizeIndex", BindingFlags.Instance | BindingFlags.NonPublic);
        if (selectedSizeIndexField != null)
        {
            selectedSizeIndexField.SetValue(window, index);
            window.Repaint();
        }
    }
}
#endif
