using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSaveEditor
{
    private static double lastSaveTime;
    private static readonly double saveInterval = 300; // seconds (every 5 minutes)

    static AutoSaveEditor()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        // Don't autosave while playing or compiling
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            return;

        if (EditorApplication.timeSinceStartup - lastSaveTime > saveInterval)
        {
            SaveCurrentScene();
            lastSaveTime = EditorApplication.timeSinceStartup;
        }
    }

    private static void SaveCurrentScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.isDirty) // Only save if something changed
        {
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("💾 Auto-saved scene at " + System.DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
