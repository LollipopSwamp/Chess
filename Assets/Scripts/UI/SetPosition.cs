using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetPosition : MonoBehaviour
{
    //game object refs
    public GameObject ui;
    public GameObject gridManager;

    //set position variables
    public List<GameObject> pieceButtons = new List<GameObject>();
    public char selectedPiece = 'K';
    private string pieceList = "KQRBNPkqrbnp-";

    //side to move radio button
    public string currTurn = "w";
    public GameObject whiteToMoveCheck;
    public GameObject blackToMoveCheck;

    void Start()
    {
        SideToMoveWhite();
        DeselectAll();
        Select(0);
    }
    public void Select(int _btnIndex)
    {
        DeselectAll();
        pieceButtons[_btnIndex].GetComponent<Image>().color = Color.white;
        selectedPiece = pieceList[_btnIndex];
    }
    private void DeselectAll()
    {
        foreach (GameObject button in pieceButtons)
        {
            button.GetComponent<Image>().color = Color.grey;
        }
    }
    public void BackBtn()
    {
        
        ui.GetComponent<UIManager>().SaveCustomPosition(currTurn);
        ui.GetComponent<UIManager>().ShowNewGameMenu();
    }
    public void StartPosBtn()
    {

    }
    public void ClearBoardBtn()
    {

    }
    public void SideToMoveWhite()
    {
        currTurn = "w";
        whiteToMoveCheck.SetActive(true);
        blackToMoveCheck.SetActive(false);
    }
    public void SideToMoveBlack()
    {
        currTurn = "b";
        whiteToMoveCheck.SetActive(false);
        blackToMoveCheck.SetActive(true);
    }
}
