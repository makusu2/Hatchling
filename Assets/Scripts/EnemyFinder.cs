using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnemyFinder : MonoBehaviour {
    
    private int distToNotice;
    
    
    public void Setup(int distToNotice = 10) {
        this.distToNotice = distToNotice;
    }
    
    public GameObject GetClosestWithProp(Func<GameObject,bool> qualificationFunc) {
        IEnumerable<GameObject> nearbyQualifiers = GetNearbyWithProp(qualificationFunc);
        return GetClosestGameObject(nearbyQualifiers);
    }
    public IEnumerable<GameObject> GetNearbyWithProp(Func<GameObject,bool> qualificationFunc) {
        foreach (Collider possibleCollider in Physics.OverlapSphere(transform.position,distToNotice)) {
            if (qualificationFunc(possibleCollider.gameObject)) {
                yield return possibleCollider.gameObject;
            }
        }
        yield break;
    }
    
    public bool IsEnemy(GameObject go) {
        Factions[] enemyFactions = GetComponent<Faction>().GetEnemyFactions();
        Faction enemyFaction = go.GetComponent<Faction>();
        return (enemyFaction != null && enemyFactions.Contains(enemyFaction.faction));
    }
    public GameObject GetClosestEnemy() {
        Func<GameObject,bool> qualFunc = (GameObject go) => IsEnemy(go);
        return GetClosestWithProp(qualFunc);
    }
    
    public GameObject GetClosestFood() {
        Func<GameObject,bool> qualFunc = (GameObject go) => go.GetComponent<FoodBehavior>() != null;
        return GetClosestWithProp(qualFunc);
    }
    
    
    GameObject GetClosestGameObject(IEnumerable<GameObject> GOs) {
        try {
            return GOs.OrderBy(go => Vector3.Distance(transform.position,go.transform.position)).First();
        }
        catch(InvalidOperationException) {
            return null;
        }
        
    }
    
}
