using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public GameObject tooltip;

    void Start()
    {
        tooltip.SetActive(false);
    }
    private void OnMouseEnter()
    {
        UnityEngine.Debug.Log("mouse enter");
        tooltip.SetActive(true);
    }
    private void OnMouseExit()
    {
        tooltip.SetActive(false);
    }
}
