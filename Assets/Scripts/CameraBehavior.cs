using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraBehavior : MonoBehaviour, WaterEnterer {

	// Use this for initialization
	void Start () {
        RenderSettings.fog = true;
        RenderSettings.fogDensity=0;
	}
    
    static Color normalColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);
    static Color underwaterColor = new Color (0.22f, 0.65f, 0.77f, 0.5f);
    
    bool inWater;
    public bool InWater{
        get {
            return inWater;
        }
        set {
            inWater = value;
        }
    }
    public void OnWaterEnter() {
        InWater = true;
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = 0.01f;
    }
    public void OnWaterExit() {
        InWater = false;
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity=0;
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
