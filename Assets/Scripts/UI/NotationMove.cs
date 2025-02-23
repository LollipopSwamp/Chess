using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotationMove : MonoBehaviour
{
    private static int totalMoves = 1;
    [SerializeField] private GameObject moveNumberText;

    [SerializeField] private GameObject whiteMoveText;
    [SerializeField] private FEN whiteMoveFEN;

    [SerializeField] private GameObject blackMoveText;
    [SerializeField] private FEN blackMoveFEN;
    [SerializeField] private bool blackMoveActive = false;

    void Start()
    {
        moveNumberText.GetComponent<TextMeshProUGUI>().text = totalMoves.ToString();
        totalMoves++;
        //whiteMoveText.GetComponent<TextMeshProUGUI>().text = "";
        //blackMoveText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void SetWhiteMove(string _move, FEN fen)
    {
        whiteMoveFEN = fen;
        whiteMoveText.GetComponent<TextMeshProUGUI>().text = _move;
        blackMoveText.GetComponent<TextMeshProUGUI>().text = "";
    }
    public void SetBlackMove(string _move, FEN fen)
    {
        blackMoveFEN = fen;
        blackMoveText.GetComponent<TextMeshProUGUI>().text = _move;
        blackMoveActive = true;
    }
    public void ClickWhiteMove()
    {

    }
    public void ClickBlackMove()
    {
        if (!blackMoveActive)
        {
            return;
        }
        else
        {

        }
    }
}
