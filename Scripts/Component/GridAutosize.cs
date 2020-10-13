using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GridType
{
    Two,
    One,  
    None
};

public class GridAutosize : MonoBehaviour
{
    //[HideInInspector]
    public GridType _GT = GridType.Two;

    private void Update()
    {
        if (_GT == GridType.Two)
        {
            transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(transform.GetComponent<RectTransform>().rect.height / 2 - 20, transform.GetComponent<RectTransform>().rect.height / 2 - 20);
        }
        else if (_GT == GridType.One)
        {
            transform.GetComponent<GridLayoutGroup>().cellSize = new Vector2(transform.GetComponent<RectTransform>().rect.height - 20, transform.GetComponent<RectTransform>().rect.height - 20);
        }
    }
}
