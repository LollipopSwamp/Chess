using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //object refs
    public GameObject gridManager;
    public GameObject chessManager;

    //menu objects
    public GameObject tooltip;
    public GameObject newGameMenu;
    public GameObject setPositionMenu;
    public GameObject newGameBtn;
    public GameObject flipBoardBtn;

    void Start()
    {
        ShowGame();
    }
    public void ShowGame()
    {
        gridManager.GetComponent<GridManager>().boardMode = 1;
        HideAll();
        newGameBtn.SetActive(true);
        flipBoardBtn.SetActive(true);
    }
    public void ShowNewGameMenu()
    {
        gridManager.GetComponent<GridManager>().boardMode = 0;
        HideAll();
        newGameMenu.SetActive(true);
    }
    public void ShowSetPositionMenu()
    {
        gridManager.GetComponent<GridManager>().boardMode = 2;
        HideAll();
        setPositionMenu.SetActive(true);
        flipBoardBtn.SetActive(true);
    }
    public void HideAll()
    {
        newGameMenu.SetActive(false);
        setPositionMenu.SetActive(false);
        newGameBtn.SetActive(false);
        flipBoardBtn.SetActive(false);
    }
    public void SaveCustomPosition(string _currTurn)
    {
        chessManager.GetComponent<ChessManager>().SetCustomFEN(_currTurn);
        newGameMenu.GetComponent<NewGameMenu>().SetFENInputText(chessManager.GetComponent<ChessManager>().customFEN.GetFEN());
    }
    public void StartGame(FEN _fen)
    {
        chessManager.GetComponent<ChessManager>().StartGame(_fen);
        ShowGame();
    }
}
