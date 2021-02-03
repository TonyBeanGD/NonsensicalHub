using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
      Debug.Log ( transform.GetComponentInParent<Transform>().name);
	}
	
}
