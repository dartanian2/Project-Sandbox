using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainActualChanger : MonoBehaviour {

    // Use this for initialization
    public int depth = 20;
    public int width = 256;
    public int height = 256;
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData TD = terrain.terrainData;
        TD = terrainBoi(terrain.terrainData);
       
        Debug.Log(TD.GetHeight(0, 0));
    }

    TerrainData terrainBoi(TerrainData TD)
    {
        TD.heightmapResolution = width + 1;
        TD.size = new Vector3(width, height, height);
        TD.SetHeights(0, 0, genBoi());
        return TD;
    }

    float[,] genBoi()
    {
        float[,] heightMap = new float[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //heightMap[x, y] = generateHeight(x, y);
                heightMap[x, y] = 0.1f;
            }
        }
        return heightMap;
    }

    float generateHeight(int x, int y)
    {
        float xCoord = (float)x / width * (float)20f;
        float yCoord = (float)y / height * (float)20f;
        return Mathf.PerlinNoise(xCoord,yCoord);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
