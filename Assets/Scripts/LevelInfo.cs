using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInfo : MonoBehaviour
{
    public List<ItemWithAmount> items;
    public Vector3Int structureSize = new Vector3Int(5, 5, 5);
    public string nextLevel;
}

[System.Serializable]
public class ItemWithAmount {
    public string itemName;
    public int amount;
}