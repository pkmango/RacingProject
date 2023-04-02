using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader; // Компонент для загрузки другой сцены
    public GameObject mainCanvas;
    public GameObject biomeSelectionCavas;



    void Start()
    {
        
    }

    public void ShowBiomeSelection()
    {
        mainCanvas.SetActive(false);
        biomeSelectionCavas.SetActive(true);
    }

    public void ShowMain()
    {
        mainCanvas.SetActive(true);
        biomeSelectionCavas.SetActive(false);
    }

    public void ShowTargetCanvas(GameObject target, GameObject current)
    {
        target.SetActive(true);
        current.SetActive(false);
    }

    public void Options()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
