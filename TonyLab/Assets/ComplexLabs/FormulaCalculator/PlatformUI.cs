using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformUI : MonoBehaviour {

    [SerializeField] private Button btn_Delete;

    private void Awake()
    {
        btn_Delete.onClick.AddListener(OnDeleteClick);
    }

    private void OnDeleteClick()
    {
        Destroy(gameObject);
    }
}
