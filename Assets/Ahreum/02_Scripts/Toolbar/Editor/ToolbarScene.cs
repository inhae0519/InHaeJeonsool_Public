#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityToolbarExtender;

[InitializeOnLoad]
public class ToolbarScene
{
    private const string SCENES_FILE_PATH = "/Scenes";

    static ToolbarScene()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    private static void OnToolbarGUI(IMGUIEvent evt)
    {
        Debug.Log($"OnToolbarGUI {evt.target}");
    }

    private static void OnToolbarGUI()
    {
        var content = new GUIContent(SceneManager.GetActiveScene().name);
        
        var size = EditorStyles.toolbarDropDown.CalcSize(content);

        var filePath =
            $"{Application.dataPath}{SCENES_FILE_PATH}"; //Path.Combine(Application.dataPath, SCENES_FILE_PATH);

        if (!EditorGUILayout.DropdownButton(content, FocusType.Keyboard,EditorStyles.toolbarDropDown, GUILayout.Width(size.x + 5f))) return;

        var menu = new GenericMenu();
        MakeSceneMenus(filePath, menu);
        menu.ShowAsContext();
    }

    private static void MakeSceneMenus(string path, GenericMenu menu, string addPath = "")
    {
        string[] scenes = { };
        try
        {
            scenes = Directory.GetFileSystemEntries(path);
        }
        catch
        {
            // ignored
        }

        foreach (var scene in scenes)
        {
            var dotIndex = scene.LastIndexOf('.');
            if (dotIndex == -1) continue;

            var substring = scene.Substring(dotIndex);

            if (substring == ".meta") continue;

            var extension = Path.GetFileNameWithoutExtension(scene);
            
            if (substring == ".unity")
            {
                var assetsIndex = scene.IndexOf("Assets");

                if (assetsIndex == -1) continue;

                menu.AddItem(new GUIContent($"{addPath}{extension}"), false, () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(scene.Substring(assetsIndex));
                });
            }
            else
            {
                if (addPath == "")
                    MakeSceneMenus(scene, menu, extension + "/");
                else
                    MakeSceneMenus(scene, menu, addPath + extension + "/");
            }
        }
    }
}
#endif