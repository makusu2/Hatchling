using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        /*RaycastHit hit;
        Physics.Raycast(pos,fwd,out hit,maxDistance: maxDist);
        return hit;*/
        RaycastHit[] hits = Physics.RaycastAll(pos, fwd,maxDist).OrderBy(h=>h.distance).ToArray();
        foreach(RaycastHit hit in hits) {
            if(!hit.transform.CompareTag("MainPlayer")) {
                return hit;//return hits[0];
            }
        }
        return default(RaycastHit);
    }
}
