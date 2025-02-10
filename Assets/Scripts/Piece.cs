using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] public int color = -1; //white =0, black = 1, empty = -1
    [SerializeField] public int square; //numeric notation
    public List<int> legalMoves = new List<int>();
}
