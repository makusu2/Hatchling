using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballBehavior : MonoBehaviour {

    private readonly int lifetime = 2; //lifetime in seconds
    private readonly int maxHits = 1;
    private int currentHits = 0;
    private int damage = 10;
	// Use this for initialization
	void Start () {
		Invoke("EndLife",lifetime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void GetShot(Vector3 forward, float speed = 10) {
        GetComponent<Rigidbody>().velocity = forward * speed;
    }
    void EndLife() {
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision col) {
        Health colHealth = col.gameObject.GetComponent<Health>();
        if (colHealth != null) {
            colHealth.GetDamaged(damage,Health.DamageTypes.fire);
            currentHits += 1;
            if (currentHits >= maxHits) {
                EndLife();
            }
        }
    }
}
