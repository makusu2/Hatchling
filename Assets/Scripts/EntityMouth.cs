using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMouth : MonoBehaviour {
    LivingEntity parentEntity;
    int attackLevel;
    
    [SerializeField] private AudioClip successBiteSound;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Setup(LivingEntity parentEntity, int attackLevel = 3) {
        this.parentEntity = parentEntity;
        this.attackLevel = attackLevel;
    }
    void OnTriggerEnter(Collider col) {
        bool heldByCol = transform.IsChildOf(col.gameObject.transform);
        if (heldByCol) {
            return;
        }
        Health colHealth = col.gameObject.GetComponent<Health>()??col.transform.parent.GetComponent<Health>();
        if (colHealth != null) {
            colHealth.GetDamaged(attackLevel);
            Vector3 contactPoint = col.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            MakuUtil.PlayBloodAt(contactPoint);
        }
    }
}
