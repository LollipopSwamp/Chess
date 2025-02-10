using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    //square color
    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] public GameObject legalMoveHighlight;
    public bool isOffset;

    //piece in square
    [SerializeField] private GameObject pieceSprite;
    [SerializeField] public char pieceName;
    [SerializeField] public int squareName;
    [SerializeField] public string algebraicSquareName;
    [SerializeField] private GameObject squareNameCanvas;
    [SerializeField] private GameObject squareNameText;
    [SerializeField] public GameObject piece;
    private Vector3 screenPoint;
    private Vector3 offset;

    private GridManager gridManager;
    //sprites
    [SerializeField] Sprite[] spritesList;
    private Dictionary<char, Sprite> spritesDict = new Dictionary<char, Sprite>();


    void Start()
    {
        gridManager = transform.parent.gameObject.GetComponent<GridManager>();
    }

    public void Init(bool _isOffset, string _squareName, Vector3 _posOffset)
    {
        isOffset = _isOffset;
        renderer.color = _isOffset ? offsetColor : baseColor;

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
        nameTMP.text = algebraicSquareName;
        nameTMP.color = _isOffset ? Color.black : Color.white;
        squareNameCanvas.GetComponent<Canvas>().sortingLayerName = "Board";
        squareNameCanvas.transform.position = transform.position + _posOffset;
        //Debug.Log(squareNameCanvas.transform.position);

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
        pieceName = _pieceName;
        if (pieceName == '-')
        {
            piece.GetComponent<Piece>().color = -1;
        }
        else if (char.IsUpper(pieceName))
        {
            piece.GetComponent<Piece>().color = 0;
        }
        else if (!char.IsUpper(pieceName))
        {
            piece.GetComponent<Piece>().color = 1;
        }
        pieceSprite.GetComponent<SpriteRenderer>().sprite = spritesDict[pieceName];
    }

    void OnMouseEnter()
    {
        gridManager.highlightedTile = int.Parse(this.name);
        //Debug.Log(this.name);
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    void OnMouseDown()
    {
        gridManager.clickedTile = int.Parse(this.name);
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        var parentObject = this.transform.parent.gameObject;
        Debug.Log("Legal Moves Count: " + piece.GetComponent<Piece>().legalMoves.Count.ToString());
        Debug.Log("Legal Moves: " + string.Join(", ", piece.GetComponent<Piece>().legalMoves));
        gridManager.HighlightLegalMoveTiles(piece.GetComponent<Piece>().legalMoves);
        piece.GetComponent<Renderer>().sortingOrder = 1;
        //Debug.Log(this.name + " clicked");
    }
    void OnMouseUp()
    {
        gridManager.MovePiece();
        piece.transform.position = transform.position;
        gridManager.UnhighlightLegalMoveTiles();
        piece.GetComponent<Renderer>().sortingOrder = 0;
        //Debug.Log(this.name.ToString() + " " + piece.transform.position.ToString());
    }
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        piece.transform.position = curPosition;

    }
}
