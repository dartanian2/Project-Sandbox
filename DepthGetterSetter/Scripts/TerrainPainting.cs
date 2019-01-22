using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPainting : MonoBehaviour {


    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public int startingHeight;
    }

    public Transform WaterBall;
    public SplatHeights[] splatHeights;
    private float[,,] splatmapData;
    private readonly int seaLevel = 5;
    private readonly int maxHeight = 600;

    // Use this for initialization

    public void UpdatePaint()
    {
        Time.timeScale = 4;
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        splatmapData = new float[terrainData.alphamapWidth,
                                 terrainData.alphamapHeight,
                                 terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);//y and x are switched because splat map is inverted for some reason thanks unity
                float[] splat = new float[splatHeights.Length];

                //decides splat values
                for (int i = 0; i < splatHeights.Length; i++)
                {
                    if (i == splatHeights.Length - 1 && terrainHeight >= splatHeights[i].startingHeight)
                    {
                        splat[i] = 1;
                    }
                    else if (terrainHeight >= splatHeights[i].startingHeight && terrainHeight <= splatHeights[i + 1].startingHeight)
                    {
                        splat[i] = 1;
                    }
                    splatmapData[x, y, i] = splat[i];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData);
        Debug.Log("Painted");
    }

    //Change to globas interrupt or add to queue
    /*
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("You pressed space to create a river");
            //RiverGeneration();
            BallGeneration();
            Debug.Log("-------------COMPLETE---------------");
        }
    }
    */

    private void BallGeneration()
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        int x = Random.Range(0, terrainData.alphamapWidth);
        int y = Random.Range(0, terrainData.alphamapHeight);
        int foo = 0;
        int i = 0;
        /*
        while (true) {
            if (i == 100)
            {
                Debug.Log("tick");
                //Instantiate(WaterBall, new Vector3(x, terrainData.GetHeight(y, x) + foo, y), Quaternion.identity);
                foo += 5;
                i = 0;
            }
            else
            {
                i++;
            }
        }
        */

        for (int k = 0; k < 500; k++)
        {
            Instantiate(WaterBall, new Vector3(x, terrainData.GetHeight(y, x) + foo, y), Quaternion.identity);
            foo += 5;
        }
    }

    private void RiverGeneration()
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        int x = Random.Range(0, terrainData.alphamapWidth);
        int y = Random.Range(0, terrainData.alphamapHeight);

        while (true)
        {
            //To prevent out of bounds checking
            if (x > 1 && x < terrainData.alphamapWidth - 1 && y > 1 && y < terrainData.alphamapHeight - 1)
            {
                float temp;
                int xTemp = x;
                int yTemp = y;
                bool lakeCheck = true;

                //check if has reached sea
                if (terrainData.GetHeight(y, x) * maxHeight < seaLevel)
                {
                    break;
                }

                //finds the lowest point in the surrounding square
                for (int a = -1; a < 2; a++)
                {
                    for (int b = -1; b < 2; b++)
                    {
                        if (terrainData.GetHeight(y + a, x + b) < terrainData.GetHeight(y, x))
                        {
                            temp = terrainData.GetHeight(y + a, x + b);
                            xTemp = x + b;
                            yTemp = y + a;
                            lakeCheck = false;
                        }
                    }
                }

                x = xTemp;
                y = yTemp;

                //This finds correct path, now impose stuff like meanders etc ontop of the line

                //create lake if terrain is flat, else continue river
                if (lakeCheck)
                {
                    LakeGeneration(x, y);
                    break;
                }
                else
                {
                    //gives thickness to river
                    splatmapData[x - 1, y, 0] = 1;
                    splatmapData[x, y, 0] = 1;
                    splatmapData[x + 1, y, 0] = 1;
                }

                terrainData.SetAlphamaps(0, 0, splatmapData);
                //set alpha map here

                //animate with projector method???

            }
            else
            {
                break;
            }
        }
    }

    private void LakeGeneration(int x, int y)
    {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        Debug.Log("\n---Lake generation commencing---");
        float lakeCenter = terrainData.GetHeight(y, x);
        int foo = 0;

        while (true)
        {
            if (x > 1 && x < terrainData.alphamapWidth - 1 && y > 1 && y < terrainData.alphamapHeight - 1)
            {
                /*
                 *      ||
                 *    []||[]
                 *  [][]||[][]
                 * =====||=====
                 *  [][]||[][]
                 *    []||[]
                 *      ||
                 * 
                 *
                 * 
                 */
                terrainData.GetHeight(y + foo, x);
                terrainData.GetHeight(y - foo, x);
                terrainData.GetHeight(y, x + foo);
                terrainData.GetHeight(y, x - foo);
                terrainData.GetHeight(y + foo, x - foo);
                terrainData.GetHeight(y + foo, x + foo);
                terrainData.GetHeight(y - foo, x - foo);
                terrainData.GetHeight(y - foo, x + foo);

                splatmapData[x + foo, y, 0] = 1;
                splatmapData[x - foo, y, 0] = 1;
                splatmapData[x, y + foo, 0] = 1;
                splatmapData[x, y - foo, 0] = 1;
                splatmapData[x + foo, y - foo, 0] = 1;
                splatmapData[x + foo, y + foo, 0] = 1;
                splatmapData[x - foo, y - foo, 0] = 1;
                splatmapData[x - foo, y + foo, 0] = 1;

                foo++;


                //screw this
                //find the boundary
                //use search to fill in the boundary
            }
            else
            {
                break;
            }
            //splatmapData[x, y, 0] = 1;
            terrainData.SetAlphamaps(0, 0, splatmapData);
        }
    }
}
