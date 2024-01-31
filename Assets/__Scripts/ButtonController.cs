using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonController : MonoBehaviour
{
    public void LoadLevel(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

#if UNITY_STANDALONE_WIN
        Application.Quit();
#endif

    }
}

