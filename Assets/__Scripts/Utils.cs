using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utils : MonoBehaviour
{
     /// * Brezier Curves *
     /// ---------------------------------------------------------------------
     /// <summary>
     /// While most Brezier curves are 3 to 4 points, it is possible to have
     /// any numbers of points using this recursive function
     /// </summary>
     /// <param name="u"> The amount of interpolation [0...1] </param>
     /// <param name="points"> An array of Vector3s to interpolate </param>
     
    static public Vector3 Brezier(float u, params Vector3[] points)
    {
        // Setup array and list
        Vector3[,] vArr = new Vector3[points.Length, points.Length];
        List<Vector3> vList = new List<Vector3>();

        // File the last row of vArr with the elements of vList
        int r = points.Length - 1;
        for(int c = 0; c < points.Length; c++)
        {
            vArr[r,c] = vList[c];
        }

        // Iterate over all remaining rows and interpolate points at each one
        for(r--; r >= 0; r--)
        {
            for (int c = 0; c <= r; c++)
            {
                vArr[r, c] = Vector3.LerpUnclamped(vArr[r + 1, c], vArr[r + 1, c + 1], u);
            }
        }

        // vArr[0,0] holds the final interpolated value
        return vArr[0, 0];
    }

}