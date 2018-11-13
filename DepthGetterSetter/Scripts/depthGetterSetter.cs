using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using System;

public class depthGetterSetter : MonoBehaviour
{

    public enum DisplayFrameType
    {
        Infrared,
        Color,
        Depth
    }

    //unity variables
    private const int width = 512;
    private const int height = 424;
    private float[,] depthMap = new float[512, 424];

    //private int foo = 0;
    //private float[,] tempMap = null;
    
    //instansiate TerrainGenerator
    private TerrainGenerator terrainGenerator;

    //create variable to handle the actual sensor hardware
    private KinectSensor kinectSensor = null;

    //depth Frame variables
    private MultiSourceFrameReader frameReader = null;
    private ushort[] depthFrameData = null;


    // Use this for initialization
    void Start()
    {
        terrainGenerator = GetComponent<TerrainGenerator>();

        //gives variable kinectSensor the actual hardware
        this.kinectSensor = KinectSensor.GetDefault();

        //Setup depth data stuff for incoming frames
        //Depth frame source
        FrameDescription depthFrameDescription =
            this.kinectSensor.DepthFrameSource.FrameDescription;

        //open reader for multiple types of frames
        this.frameReader = this.kinectSensor.OpenMultiSourceFrameReader
            (FrameSourceTypes.Depth | FrameSourceTypes.Color);

        //open reader for multi source frames
        this.frameReader.MultiSourceFrameArrived +=
            this.Reader_MultiSourceFrameArrived;

        //create space for pixel generation
        this.depthFrameData = new ushort[depthFrameDescription.Width *
            depthFrameDescription.Height];

        //starts the kinect sensor
        this.kinectSensor.Open();
        Debug.Log("Sensor initialised");

    }
       
    private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
    {
        //bool depthFrameProcessed = false;
        //gets reference to multi frame object
        var reference = e.FrameReference.AcquireFrame();

        //getting the depthframe from the multiframe object
        using (DepthFrame depthFrame = reference.DepthFrameReference.AcquireFrame())
        {
            if (depthFrame != null)
            {
                FrameDescription depthFrameDescription = depthFrame.FrameDescription;

                //gets depthFrame data and outputs to array of ushort ints
                //depthFrameData(y*frameHeight + x)
                depthFrame.CopyFrameDataToArray(depthFrameData);


                //to prevent frames being rendered whilst people are moving sand around, find the sum of the heights,
                //and if it is greater than the highest possible collective height of all the sand there is obviously something else in the way?
                
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        depthMap[x, y] = (float)depthFrameData[x + y * depthFrameDescription.Height];
                    }
                }

                /*
                int count = 0;
                foreach (ushort depth in depthFrameData)
                {
                    count++;
                }
                Debug.Log(count);
                //217088
                */

                //when representing the camera space as a 1D list, is depthFrameDate like this:
                //[0][1][2][3][4]
                //[5][6][7][8][9]
                //i think this ^^^^^^^

                //or, like this:
                //[0][3][6]
                //[1][4][7]
                //[2][5][8]

                terrainGenerator.UpdateTerrain(depthMap, width, height);

                //combining frames makes no difference to accuracy that I can tell
                //I dont think tempMap is even saved, is it altering the global value? Same with foo
                /*
                if (foo == 3)
                {
                    terrainGenerator.UpdateTerrain(tempMap, width, height);
                    foo = 0;
                }
                else
                {
                    foo++;
                    tempMap = terrainGenerator.Combine(tempMap, depthMap, width, height);
                }
                */


                //probably will be useful in the future
                //ushort minDepth = frame.DepthMinReliableDistance;
                //ushort maxDepth = frame.DepthMaxReliableDistance;
            }
        }

        //can put other type of frame handlers in here if needed

    }
}
