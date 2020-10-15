using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _3DPrinterPlus : MonoBehaviour {


    private List<Material> mats;
    private float value = 0;
    [SerializeField]
    private readonly float speed = 0.0005f;

    void Start()
    {
        mats = new List<Material>();

        MeshRenderer[] mrs = transform.GetComponentsInChildren<MeshRenderer>();

        foreach (var item in mrs)
        {
            foreach (var item2 in item.materials)
            {
                mats.Add(item2);

            }
        }
    }

    void Update()
    {
        if (value < 1)
        {
            foreach (var item in mats)
            {
                item.SetFloat("_ConstructY", value += speed);
                item.SetFloat("_ParentY", transform.position.y);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            value = 0;
        }

    }
}
