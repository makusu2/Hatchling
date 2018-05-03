using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamageTaker : MonoBehaviour {


    private Vector3[] previousPoses;
    
    private Vector3 currentPos { get { return transform.position;}}
    
    private Vector3 previousPreviousVelocity { get { return (previousPoses[1]-previousPoses[2])/Time.fixedDeltaTime;}}
    private Vector3 previousVelocity { get {return (previousPoses[0]-previousPoses[1])/Time.fixedDeltaTime;}}
    private Vector3 currentVelocity { get { return (currentPos-previousPoses[0])/Time.fixedDeltaTime;}}
    private Vector3 currentAcceleration { get { return (currentVelocity-previousVelocity)/(Time.fixedDeltaTime*2);}}
    private Vector3 previousAcceleration { get { return (previousVelocity-previousPreviousVelocity)/(Time.fixedDeltaTime*2);}}
    private Vector3 currentJerk { get { return (currentAcceleration-previousAcceleration)/(Time.fixedDeltaTime*3);}}
    private Vector3 accurateAcceleration { get { return (currentVelocity-previousPreviousVelocity)/(Time.fixedDeltaTime*3);}}
    private float vertAcceleration { get { return accurateAcceleration.y;}}
    
    private bool testDamageNextRound = false;
    private bool isTouchingGround { get { return GetComponent<CharacterController>().isGrounded;}}
    private bool wasTouchingGround = false;
    
    private int minToDamage = 500;
    
    void CycleValues() {
        previousPoses[2]=previousPoses[1];
        previousPoses[1]=previousPoses[0];
        previousPoses[0]=currentPos;
        testDamageNextRound = !wasTouchingGround && isTouchingGround;
        wasTouchingGround = isTouchingGround;
    }
	// Use this for initialization
	void Start () {
		previousPoses = new Vector3[] {currentPos,currentPos,currentPos};
	}
    
    void FixedUpdate() {
        if (testDamageNextRound) {
            //500 should be min to damage
            if (vertAcceleration > minToDamage) {
                int damageToTake = (int)((vertAcceleration-500.0f)/2);
                GetComponent<Health>().GetDamaged(damageToTake,damageType: Health.DamageTypes.fall);
            }
        }
        CycleValues();
    }
}
