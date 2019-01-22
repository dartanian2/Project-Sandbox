using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour {

    public GameObject ui;
    public Camera terrainCamera;
    public depthGetterSetter kinectInterface;

    public void ShowUI()
    {
        terrainCamera.enabled = false;
        ui.SetActive(true);
    }

    public void ShowTerrain()
    {
        terrainCamera.enabled = true;
        ui.SetActive(false);
        kinectInterface.Setup();
    }
}
