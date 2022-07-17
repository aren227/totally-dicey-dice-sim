using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceUi : MonoBehaviour
{
    public void SetNumber(int number) {
        // @Hardcoded: Child indices.
        if (1 <= number && number <= 6) {
            transform.GetChild(number-1).gameObject.SetActive(true);
        }
        else {
            transform.GetChild(6).gameObject.SetActive(true);
            transform.GetChild(6).GetComponent<Text>().text = number.ToString();
        }
    }
}
