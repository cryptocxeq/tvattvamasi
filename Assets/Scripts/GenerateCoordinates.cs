using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCoordinates : MonoBehaviour
{
    int delay = 5;
    float genTimer = 0f;
    int xmin = (int)(0 + (Screen.width * 0.10));
    int xmax = (int)(Screen.width * 0.90);
    int ymin = (int)(0 + (Screen.height * 0.10));
    int ymax = (int)(Screen.height * 0.90);
    float percentile = 0.25f;
    int xco;
    int yco;
    Vector3 cv;
    int gameLength = 10;

    int maxCo;
    int minCo;
    bool createBubbles = true;
    int upc = 0;
    public static bool showNext = false;
    List<int[]> coords = new List<int[]>();

    public TextManager texm;

    public GameObject myPrefab;

    void Start()
    {
        GetRandomSetCoordinates();
    }

    void GetRandomSetCoordinates()
    {
        int minCo = gameLength;
        int maxCo = gameLength;
        int numberCo = UnityEngine.Random.Range(minCo, maxCo);
        int i = 1;
        int attempts = 0;
        int distance = 0;
        bool checkLast = true;

        bool passable = true;
        xco = UnityEngine.Random.Range(xmin, xmax);
        yco = UnityEngine.Random.Range(ymin, ymax);
        coords.Add(new int[] { xco, yco, 0 });
        float minDistance = (xmax * ymax * percentile) / (xmax + ymax); //needs to be improved (function to check find min distance between balls based on screensize)
        float lastDistance = minDistance / 2;
        while (i <= numberCo)
        {
            attempts++;
            passable = true;
            xco = UnityEngine.Random.Range(xmin, xmax);
            yco = UnityEngine.Random.Range(ymin, ymax);
            if (checkLast == true)
            {
                distance = (int)System.Math.Sqrt(((coords[coords.Count - 1][0] - xco) * (coords[coords.Count - 1][0] - xco)) + ((coords[coords.Count - 1][1] - yco) * (coords[coords.Count - 1][1] - yco)));
                if (distance < lastDistance)
                {
                    passable = false;
                }
            }
            for (int n = 0; n < coords.Count; n++)
            {
                distance = (int)System.Math.Sqrt(((coords[n][0] - xco) * (coords[n][0] - xco)) + ((coords[n][1] - yco) * (coords[n][1] - yco)));
                if (distance < minDistance)
                {
                    passable = false;
                }


            }

            if (passable == true) {
                i++;
                coords.Add(new int[] { xco, yco, 0 });
            }

            if (i > 10)
            {
                break;
            }

        }

        Debug.Log("attmepts:" + attempts);

        for (int n = 0; n < coords.Count; n++)
        {


        }
    }

    void Update()
    {
        Debug.Log(delay - genTimer);
        if (texm.introState == 7 || texm.introState == 8)
        {
            float dt = Time.deltaTime;
            
            if (showNext == true)
            {
                genTimer += dt;
                if ((genTimer > delay) & (upc < coords.Count) & (showNext == true))
                {
                    showNext = false;
                    cv = new Vector3(coords[upc][0], coords[upc][1], 1);
                    cv = Camera.main.ScreenToWorldPoint(cv);
                    if (createBubbles)
                    {
                        Instantiate(myPrefab, cv, Quaternion.identity);
                        //mask.ScheduleFullMaskRefresh();
                    }
                    genTimer = 0f;
                    upc++;
                }
            }
            
        }
       
    }
}