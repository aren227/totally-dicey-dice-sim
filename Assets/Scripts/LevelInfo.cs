using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    public List<ItemWithAmount> items;
}

[System.Serializable]
public class ItemWithAmount {
    string itemName;
    int amount;
}