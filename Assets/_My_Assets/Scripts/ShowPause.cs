using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPause : MonoBehaviour
{
    public GameObject pauseGameObject;

    public void ShowPauseMenu()
    {
        pauseGameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) pauseGameObject.SetActive(true);
    }
}
