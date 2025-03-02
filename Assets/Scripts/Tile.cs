using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    //parent
    private GridManager gridManager;

    //square color
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] public GameObject legalMoveHighlight;
    [SerializeField] public bool isOffset;

    //piece in square
    [SerializeField] private GameObject pieceSprite;
    [SerializeField] public Piece piece;
    [SerializeField] Sprite[] spritesList;
    private Dictionary<char, Sprite> spritesDict;

    //tile names
    [SerializeField] public int squareName;
    [SerializeField] public string algebraicSquareName;

    //square name text
    [SerializeField] private GameObject squareNameCanvas;
    [SerializeField] private GameObject squareNameText;

    //position
    private Vector3 screenPoint;
    private Vector3 offset;

    void Start()
    {
        gridManager = transform.parent.gameObject.GetComponent<GridManager>();
    }

    public void Init(bool _isOffset, string _squareName, Vector3 _posOffset)
    {
        isOffset = _isOffset;
        renderer.color = _isOffset ? offsetColor : baseColor;
        spritesDict = new Dictionary<char, Sprite>();

        //set spritesDict
        spritesDict.Add('b', spritesList[0]);
        spritesDict.Add('k', spritesList[1]);
        spritesDict.Add('n', spritesList[2]);
        spritesDict.Add('p', spritesList[3]);
        spritesDict.Add('q', spritesList[4]);
        spritesDict.Add('r', spritesList[5]);
        spritesDict.Add('B', spritesList[6]);
        spritesDict.Add('K', spritesList[7]);
        spritesDict.Add('N', spritesList[8]);
        spritesDict.Add('P', spritesList[9]);
        spritesDict.Add('Q', spritesList[10]);
        spritesDict.Add('R', spritesList[11]);
        spritesDict.Add('-', spritesList[12]);

        squareName = int.Parse(_squareName);
        TextMeshProUGUI nameTMP = squareNameText.GetComponent<TextMeshProUGUI>();
        algebraicSquareName = NumericToAlgebraicNotation(squareName);
        nameTMP.text = squareName.ToString();
        //nameTMP.text = algebraicSquareName;
        //nameTMP.color = _isOffset ? Color.black : Color.white;
        squareNameCanvas.GetComponent<Canvas>().sortingLayerName = "Board";
        squareNameCanvas.transform.position = transform.position + _posOffset;
        piece = new Piece('-',squareName);
        //UnityEngine.Debug.Log(squareNameCanvas.transform.position);

    }
    public void SetSquareNamePos(Vector3 _posOffset)
    {
        Vector3 newPos = transform.position + _posOffset;
        newPos = new Vector3(newPos.x, newPos.y, 2);
        squareNameCanvas.transform.position = newPos;
    }
    public string NumericToAlgebraicNotation(int _numericNotation)
    {
        string column;
        string algebraicNotation = "";
        switch (Mathf.Floor(_numericNotation / 10))
        {
            case 1:
                algebraicNotation = "a";
                break;
            case 2:
                algebraicNotation = "b";
                break;
            case 3:
                algebraicNotation = "c";
                break;
            case 4:
                algebraicNotation = "d";
                break;
            case 5:
                algebraicNotation = "e";
                break;
            case 6:
                algebraicNotation = "f";
                break;
            case 7:
                algebraicNotation = "g";
                break;
            case 8:
                algebraicNotation = "h";
                break;
        }
        algebraicNotation += (_numericNotation % 10).ToString();
        return algebraicNotation;
    }

    public void SetPiece(char _pieceName)
    {
        piece = new Piece(_pieceName, squareName);
        pieceSprite.GetComponent<SpriteRenderer>().sprite = spritesDict[piece.name];
    }

    void OnMouseEnter()
    {
        gridManager.highlightedTile = int.Parse(this.name);
        //UnityEngine.Debug.Log(this.name);
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    void OnMouseDown()
    {
        switch (gridManager.boardMode)
        {
            case 0://board off
                break;
            case 1: // play game
                gridManager.clickedTile = int.Parse(this.name);
                screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
                var parentObject = this.transform.parent.gameObject;
                //UnityEngine.Debug.Log("Legal Moves Count: " + piece.legalMoves.Count.ToString());
                //UnityEngine.Debug.Log("Legal Moves: " + string.Join(", ", piece.legalMoves));
                gridManager.HighlightLegalMoveTiles(piece.legalMoves);
                pieceSprite.GetComponent<Renderer>().sortingOrder = 1;
                //UnityEngine.Debug.Log(this.name + " clicked");
                break;
            case 2: // set position
                if(piece.name == gridManager.GetSelectedPieceSetPos())
                {
                    SetPiece('-');
                }
                else
                {
                    SetPiece(gridManager.GetSelectedPieceSetPos());
                }
                break;
            default:
                break;
        }
    }
    void OnMouseUp()
    {
        switch (gridManager.boardMode)
        {
            case 0://board off
                break;
            case 1: // play game
                gridManager.PlayerMovePiece();
                pieceSprite.transform.position = transform.position;
                gridManager.UnhighlightLegalMoveTiles();
                pieceSprite.GetComponent<Renderer>().sortingOrder = 0;
                break;
            case 2: // set position
                break;
            default:
                break;
        }
    }
    void OnMouseDrag()
    {
        switch (gridManager.boardMode)
        {
            case 0://board off
                break;
            case 1: // play game
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
                pieceSprite.transform.position = curPosition;
                break;
            case 2: // set position
                break;
            default:
                break;
        }

    }

    public Tile(int _squareName)
    {
        squareName = _squareName;
        algebraicSquareName = NumericToAlgebraicNotation(_squareName);
    }
    public void PrintTile()
    {
        UnityEngine.Debug.Log("== Print Tile ==");
        UnityEngine.Debug.Log("Square Name Int: " + squareName.ToString());
        UnityEngine.Debug.Log("Algebraic Square Name: " + algebraicSquareName.ToString());
        UnityEngine.Debug.Log("Piece Name: " + piece.name.ToString());
        UnityEngine.Debug.Log("Legal Moves Count: " + piece.legalMoves.Count.ToString());
        UnityEngine.Debug.Log("Legal Moves: " + string.Join(", ", piece.legalMoves));

    }
}
