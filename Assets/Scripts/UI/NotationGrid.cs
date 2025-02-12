using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotationGrid : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject notationMovePrefab;
    public List<GameObject> notationPanels = new List<GameObject>();

    public void AddWhiteMove(string _move)
    {
        var newNotationPanel = Instantiate(notationMovePrefab, Vector3.zero, Quaternion.identity, this.transform);
        newNotationPanel.transform.parent = content.transform;
        newNotationPanel.GetComponent<NotationMove>().SetWhiteMove(_move);
        newNotationPanel.GetComponent<NotationMove>().SetBlackMove("");
        ResizeScrollContent();
        notationPanels.Add(newNotationPanel);
    }
    public void AddBlackMove(string _move)
    {
        notationPanels[notationPanels.Count - 1].GetComponent<NotationMove>().SetBlackMove(_move);
    }
    public void ResizeScrollContent()
    {
        int newHeight = (notationPanels.Count * 50) - 62;
        if( newHeight < 1080 ) { newHeight = 1030 - 62; }
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(400, newHeight);
        content.transform.localPosition = new Vector2(0, (newHeight * -0.5f) + 50);
    }
}
