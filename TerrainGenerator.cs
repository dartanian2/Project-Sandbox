using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TerrainGenerator : MonoBehaviour
{
    /// <summary>
    /// Max value depth should be, this value minus MinDistance is equivalent to 0 because inversion, is used to scale
    /// </summary>
    private const float MaxDistance = 3000f;

    /// <summary>
    /// Min value depth should be, is equivalent to 1f because inversion
    /// </summary>
    private const float MinDistance = 500f;

    /// <summary>
    /// Max value terrain mesh can be
    /// </summary>
    private const int depth = 600;

    //3725f is max value that you want to measure
    //official max is 4500f but I dont think so
    //private float divisor = 3725f;

    /// <summary>
    /// Checks if frame depths have been saved
    /// </summary>
    private bool hasSaved = true;

    /// <summary>
    /// Configures and sets heights of terrain according to depth values
    /// </summary>
    public void UpdateTerrain(float[,] depthMap, int width, int height)
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData tD = terrain.terrainData;
        tD.heightmapResolution = width + 1;
        tD.size = new Vector3(width, depth, height);

        Debug.Log("Actual Value: " + depthMap[211, 255]);

        depthMap = Clean(depthMap, width, height);
        Debug.Log("Multiplier: " + depthMap[211, 255]);
        tD.SetHeights(0, 0, depthMap);

        //Saves frame if not saved already
        if (hasSaved)
        {
            string depths = "";
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    depths += "_*_" + (depthMap[x,y]);
                }
                depths += Environment.NewLine;
            }

            SaveFile(depths);
            hasSaved = false;
        }
    }

    /*
    //combine 3 frames into one, hopefully more accurate than just one
    public float[,] Combine(float[,] tempMap, float[,] depthMap, int width, int height)
    {
        if (tempMap == null)
        {
            return depthMap;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tempMap[x, y] = (tempMap[x, y] + depthMap[x, y]) / 2;
            }
        }
        return tempMap;
    }
    */

    //if depth is zero, it should be ignored, so copy neighbour?

    /// <summary>
    /// Gets float value, minus MinDistance, divides by MaxDistance, scales and rounds to 2sf, inverts, averages with neighbours
    /// </summary>
    private float[,] Clean(float[,] depthMap, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (depthMap[x, y] > MaxDistance || depthMap[x, y] == 0)
                {
                    //Erroneous, ignore or copy adjacent

                    //Copy
                    if (x > 0)
                    {
                        depthMap[x, y] = depthMap[x - 1, y];
                    } else
                    {
                        depthMap[x, y] = 0f;
                    }

                    //Average values with neighbours
                    //depthMap = Compare4x(depthMap, x, y);
                    //depthMap = Compare9x(depthMap, x, y);
                }
                else if (depthMap[x, y] < MinDistance && depthMap[x, y] != 0)
                {
                    //Erroneous, ignore or copy adjacent
                    depthMap[x, y] = 1f;

                    //Average values with neighbours
                    //depthMap = Compare4x(depthMap, x, y);
                    //depthMap = Compare9x(depthMap, x, y);
                }
                else
                {
                    //Move value "closer" to the camera, then divide by altered scale value
                    float preRounding = (depthMap[x, y] - MinDistance) / (MaxDistance - MinDistance);

                    //Round float to 2 sf, then invert
                    depthMap[x, y] = 1f - (Mathf.Round(preRounding * 100f) / 100f);

                    //Average values with neighbours
                    //depthMap = Compare4x(depthMap, x, y);
                    depthMap = Compare9x(depthMap, x, y);
                }
            }
        }
        return depthMap;
    }

    //Averageing functions:
    //Has to point up left so that we are not comparing to values that have not been processed yet (Any value right or down of pointer has not been processed yet)

    //Should try to assign lower weight to values further away from pointer, make new function to do this
    //Use terrainData.GetSteepness to get rid of stupidly steep values?? (I have tried this and failed, still seems suitable however, try again?)


    //Shouldn't all (heightIndex + 1) be (heightIndex - 1) ??

    /// <summary>
    /// Smaller box blur filter
    /// </summary>
    private float[,] Compare4x(float[,] depthMap, int widthIndex, int heightIndex)
    {
        //ensures values on the edge are not compared, avoids accessing outside of array range
        if (widthIndex > 0 && heightIndex > 0)
        {
            //compare block of four pointing top left from indexed cell
            // [1][2]
            // [3][x]
            //like this

            float one = depthMap[widthIndex - 1, heightIndex - 1];
            float two = depthMap[widthIndex, heightIndex - 1];
            float three = depthMap[widthIndex - 1, heightIndex];
            float x = depthMap[widthIndex, heightIndex];

            float ave = (one + two + three + x) / 4;

            depthMap[widthIndex - 1, heightIndex - 1] = ave;
            depthMap[widthIndex, heightIndex - 1] = ave;
            depthMap[widthIndex - 1, heightIndex] = ave;
            depthMap[widthIndex, heightIndex] = ave;

            return depthMap;
        }
        else
        {
            return depthMap;
        }
    }

    //Does not work, why is this? (Is it because "heightIndex + 1" should be "heightIndex - 1"?)
    /// <summary>
    /// Box blur filter
    /// </summary>
    private float[,] Compare9x(float[,] depthMap, int widthIndex, int heightIndex)
    {
        //ensures values on the edge are not compared, avoids accessing outside of array range
        if (widthIndex > 1 && heightIndex > 1)
        {
            //compare block of nine pointing top left from indexed cell
            // [1][2][3]
            // [4][5][6]
            // [7][8][x]
            //like this

            float one = depthMap[widthIndex - 2, heightIndex - 2];
            float two = depthMap[widthIndex - 1, heightIndex - 2];
            float three = depthMap[widthIndex, heightIndex - 2];
            float four = depthMap[widthIndex - 2, heightIndex - 1];
            float five = depthMap[widthIndex - 1, heightIndex - 1];
            float six = depthMap[widthIndex, heightIndex - 1];
            float seven = depthMap[widthIndex - 2, heightIndex];
            float eight = depthMap[widthIndex - 1, heightIndex];
            float x = depthMap[widthIndex, heightIndex];

            float ave = (one + two + three + four + five + six + seven + eight + x) / 9;


            depthMap[widthIndex - 2, heightIndex - 2] = ave;
            depthMap[widthIndex - 1, heightIndex - 2] = ave;
            depthMap[widthIndex, heightIndex - 2] = ave;
            depthMap[widthIndex - 2, heightIndex - 1] = ave;
            depthMap[widthIndex - 1, heightIndex - 1] = ave;
            depthMap[widthIndex, heightIndex - 1] = ave;
            depthMap[widthIndex - 2, heightIndex] = ave;
            depthMap[widthIndex - 1, heightIndex] = ave;
            depthMap[widthIndex, heightIndex] = ave;

            return depthMap;
        }
        else
        {
            return depthMap;
        }
    }

    /// <summary>
    /// Saves cleaned frame depth values into Desktop/saveCleaned.txt
    /// </summary>
    private void SaveFile(string depths)
    {
        string destination = "C:/Users/Theo Levison/Desktop/saveCleaned.txt";

        using (StreamWriter streamWriter = File.CreateText(destination))
        {
            streamWriter.Write(depths);
        }
        Debug.Log("save successful");
    }
}