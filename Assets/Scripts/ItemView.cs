using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    public RenderTextureImage itemImage;
    public Text amountText;

    Button button;

    Builder builder;

    string itemName;

    void Awake() {
        builder = FindObjectOfType<Builder>();

        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void SetItem(string name, int amount) {
        itemName = name;
        amountText.text = amount.ToString();
    }

    void OnClick() {
        if (itemName != null) {
            builder.SelectItem(itemName);
        }
    }
}
