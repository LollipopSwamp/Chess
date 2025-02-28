using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewGameMenu : MonoBehaviour
{
    public int positionMode;
    public GameObject radioStartPos;
    public GameObject radioCustomPos;
    public GameObject FENText;
    public GameObject FENInput;
    public GameObject setPosBtn;

    public void Start()
    {
        RadioButtonclicked(0);
    }
    public void RadioButtonclicked(int _positionMode)
    {
        positionMode = _positionMode;
        //start pos
        if(_positionMode == 0)
        {
            radioStartPos.SetActive(true);
            radioCustomPos.SetActive(false);

            FENText.GetComponent<TextMeshProUGUI>().color = Color.grey;
            FENInput.GetComponent<TMP_InputField>().interactable = false;
            setPosBtn.GetComponent<Button>().interactable = false;
        }
        //custom pos
        else
        {
            radioStartPos.SetActive(false);
            radioCustomPos.SetActive(true);

            FENText.GetComponent<TextMeshProUGUI>().color = Color.white;
            FENInput.GetComponent<TMP_InputField>().interactable = true;
            setPosBtn.GetComponent<Button>().interactable = true;
        }
    }
}
