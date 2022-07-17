using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Structure structure;
    Part part;

    void Awake() {
        structure = FindObjectOfType<Structure>();
        part = GetComponent<Part>();
    }

    public int GetNumber() {
        int maxDot = 0;
        float max = float.NegativeInfinity;
        for (int i = 0; i < 6; i++) {
            Side side = (Side) i;
            float d = Vector3.Dot(transform.rotation * side.GetVector(), Vector3.up);
            if (max < d) {
                max = d;
                maxDot = i;
            }
        }

        Side globalSide = part.ToGlobal((Side) maxDot);

        Part adjPart = structure.GetPartAt(Vector3Int.RoundToInt(transform.localPosition) + globalSide.GetVector());
        if (adjPart != null && adjPart.IsOpaque(globalSide.GetOpposite())) {
            return 0;
        }

        int[] nums = new int[] { 6, 1, 5, 2, 4, 3 };
        return nums[maxDot];
    }
}
