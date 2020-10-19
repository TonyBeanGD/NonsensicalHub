using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseModel : MonoBehaviour
{
    [SerializeField]
    private Transform part1;
    [SerializeField]
    private Transform part2;

    private bool isOpen;

    private string IOID;

    public void Awake()
    {
        isOpen = false;
    }

    private void OnDestroy()
    {
        
    }

    private void Open()
    {
        if (isOpen==true)
        {
            return;
        }
    }

    private void Close()
    {
        if (isOpen==false)
        {
            return;
        }
    }
}
