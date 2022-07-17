using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabRegistry : MonoBehaviour
{
    public static PrefabRegistry Instance;

    public List<GameObject> partObjects;

    public GameObject gizmos;

    public GameObject arrowUi;
    public GameObject diceUi;
    public GameObject itemUi;

    public Dictionary<string, Part> parts { get; private set; }

    public Material availMat;
    public Material notAvailMat;
    public Material particleMat;

    void Awake() {
        Instance = this;

        parts = new Dictionary<string, Part>();
        foreach (GameObject partObject in partObjects) {
            Part part = partObject.GetComponent<Part>();

            parts.Add(part.partName, part);
        }
    }
}
