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
        var newNotationMove = Instantiate(notationMovePrefab, Vector3.zero, Quaternion.identity, this.transform);
        newNotationMove.GetComponent<NotationMove>().SetWhiteMove(_move);
        notationPanels.Add(newNotationMove);
    }
    public void AddBlackMove(string _move)
    {
        notationPanels[notationPanels.Count - 1].GetComponent<NotationMove>().SetBlackMove(_move);
    }
}
