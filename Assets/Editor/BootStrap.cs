using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[InitializeOnLoad]
public static  class BootStrap
{
    static BootStrap()
    {
        EditorApplication.playModeStateChanged += LoadBootStrapScene;
    }

    private static void LoadBootStrapScene(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if(state == PlayModeStateChange.EnteredPlayMode)
        {
            if(EditorSceneManager.GetActiveScene().buildIndex != 0)
            {
                EditorSceneManager.LoadScene(0);
            }
        }
    }
}
