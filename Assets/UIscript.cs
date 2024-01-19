using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class UIscript : MonoBehaviour
{


       public void Exit()
    {
        Debug.Log("клик");
       Application.Quit();    // закрыть приложение
    }

    public GameObject panelMenu;


    public void OpenMenu()
    {
        panelMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        panelMenu.SetActive(false);
    }

    public void RunGamebyID(int i)
    {
        if (i >= 1 && i <= SceneManager.sceneCountInBuildSettings)
        SceneManager.LoadScene(i);
        else
        Debug.LogError("ID не нашёлся. Максимальный ID - " + SceneManager.sceneCountInBuildSettings);
    }
}
