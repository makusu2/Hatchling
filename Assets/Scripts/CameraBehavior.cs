using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public RaycastHit GetRayHit(float maxDist = Mathf.Infinity) {
        Vector3 pos = transform.position;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        bool didHit = Physics.Raycast(pos,fwd,out hit,maxDistance: maxDist);
        return hit;
    }
}
