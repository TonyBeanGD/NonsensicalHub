using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fuck : MonoBehaviour {


[SerializeField]    InputField ipf;
    private void Update()
    {
        Debug.Log(ipf.caretPosition);
        Debug.Log(ipf.selectionAnchorPosition);
        Debug.Log(ipf.selectionFocusPosition);
    }

}
