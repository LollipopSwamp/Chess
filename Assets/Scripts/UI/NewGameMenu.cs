using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewGameMenu : MonoBehaviour
{
    //game object refs
    public GameObject gridManager;
    public UIManager uiManager;

    //setting variables
    public int positionMode;

    //UI objects
    public GameObject radioStartPos;
    public GameObject radioCustomPos;
    public GameObject FENText;
    public GameObject FENInput;
    public GameObject setPosBtn;

    public void Start()
    {
        uiManager = transform.parent.gameObject.GetComponent<UIManager>();
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
    public void SetPositionBtn()
    {
        gridManager.GetComponent<GridManager>().boardMode = 1;
    }
    public void SetFENInputText(string _fen)
    {
        FENInput.GetComponent<TMP_InputField>().text = _fen;
    }
    public void StartBtn()
    {
        switch(positionMode)
        {
            case 0:
                uiManager.StartGame(new FEN());
                break;
            case 1:
                FEN customFEN = new FEN(FENInput.GetComponent<TMP_InputField>().text);
                uiManager.StartGame(customFEN);
                break;
            default:
                break;
        }
    }
}
