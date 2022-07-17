using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Structure structure;

    public int GetNumber() {
        float[] dot = new float[6];
        dot[0] = Vector3.Dot(transform.rotation * Vector3.down, Vector3.up);
        dot[1] = Vector3.Dot(transform.rotation * Vector3.right, Vector3.up);
        dot[2] = Vector3.Dot(transform.rotation * Vector3.back, Vector3.up);
        dot[3] = Vector3.Dot(transform.rotation * Vector3.forward, Vector3.up);
        dot[4] = Vector3.Dot(transform.rotation * Vector3.left, Vector3.up);
        dot[5] = Vector3.Dot(transform.rotation * Vector3.up, Vector3.up);

        int maxDot = 0;
        float max = float.NegativeInfinity;
        for (int i = 0; i < 6; i++) {
            if (max < dot[i]) {
                max = dot[i];
                maxDot = i;
            }
        }

        return maxDot+1;
    }
}
