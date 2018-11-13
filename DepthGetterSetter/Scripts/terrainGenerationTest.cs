using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainGenerationTest : MonoBehaviour {

    
    

	// Use this for initialization
	void Start () {
        GameObject terrainObject = new GameObject("Terrain");
        Terrain terrain = terrainObject.AddComponent<Terrain>();
        //Terrain terrain = terrainObject.GetComponent<Terrain>();
        TerrainData TD = terrain.terrainData;
        TD.size.Set(256f, 256f, 256f);// = new Vector3(256f,256f,256f);
        float[,] HeightMap = new float[TD.heightmapWidth, TD.heightmapHeight];
        for (int x = 0; x < TD.heightmapWidth; x++)
        {
            for (int y = 0; y < TD.heightmapHeight; y++)
            {
                HeightMap[x, y] = 20;
            }
        }
        
        TD.SetHeights(0, 0, HeightMap);
        Debug.Log(TD.GetHeight(0, 0));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
