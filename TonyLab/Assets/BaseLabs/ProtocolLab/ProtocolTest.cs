using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtocolTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
       Debug.Log( ProtocolManager.Instance.ToString());
	}
}
