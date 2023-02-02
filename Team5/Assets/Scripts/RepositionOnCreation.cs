using UnityEngine;
using System.Collections;

public class RepositionOnCreation : MonoBehaviour {

    public Vector3 position;

	// Use this for initialization
	void Start () {
        transform.localPosition = position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
