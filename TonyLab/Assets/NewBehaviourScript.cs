using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string a = "asd(";
        int leftIndex = a.LastIndexOf('(');
        int rightIndex = a.LastIndexOf(')');
        Debug.Log(leftIndex);
        Debug.Log(rightIndex);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
