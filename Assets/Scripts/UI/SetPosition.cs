using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetPosition : MonoBehaviour
{
    public List<GameObject> pieceButtons = new List<GameObject>();

    void Start()
    {
        DeselectAll();
        Select(0);
    }
    public void Select(int _btnIndex)
    {
        DeselectAll();
        pieceButtons[_btnIndex].GetComponent<Image>().color = Color.white;
    }
    private void DeselectAll()
    {
        foreach (GameObject button in pieceButtons)
        {
            button.GetComponent<Image>().color = Color.grey;
        }
    }
}
