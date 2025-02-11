using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotationMove : MonoBehaviour
{
    private static int moveNumber = 1;
    [SerializeField] private GameObject moveNumberText;
    [SerializeField] private GameObject whiteMoveText;
    [SerializeField] private GameObject blackMoveText;

    void Start()
    {
        whiteMoveText.GetComponent<TextMeshProUGUI>().text = moveNumber.ToString();
        moveNumber++;
        whiteMoveText.GetComponent<TextMeshProUGUI>().text = "";
        blackMoveText.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void SetWhiteMove(string _move)
    {
        whiteMoveText.GetComponent<TextMeshProUGUI>().text = _move;
    }
    public void SetBlackMove(string _move)
    {
        blackMoveText.GetComponent<TextMeshProUGUI>().text = _move;
    }
}
