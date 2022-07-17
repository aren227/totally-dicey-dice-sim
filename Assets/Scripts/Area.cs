using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    BoxCollider boxCollider;

    void Awake() {
        boxCollider = GetComponent<BoxCollider>();
    }

    public bool HasEntered() {
        Bounds bounds = boxCollider.bounds;
        if (Physics.CheckBox(bounds.center, bounds.extents, Quaternion.identity, LayerMask.GetMask("Structure"))) {
            return true;
        }
        return false;
    }
}
