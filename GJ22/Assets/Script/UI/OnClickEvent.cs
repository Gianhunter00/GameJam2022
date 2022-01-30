using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnClickEvent : MonoBehaviour
{
    public void OnTutorial()
    {
        SceneManager.LoadScene(1);
    }

    public void OnLevel1()
    {
        SceneManager.LoadScene(2);
    }
    public void OnQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
}
