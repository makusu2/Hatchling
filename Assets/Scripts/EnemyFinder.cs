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
        GameObject[] nearbyQualifiers = GetNearbyWithProp(qualificationFunc);
        if (nearbyQualifiers.Length > 0) {
            return GetClosestGameObject(nearbyQualifiers);
        }
        return null;
    }
    public GameObject[] GetNearbyWithProp(Func<GameObject,bool> qualificationFunc) {
        Collider[] possibleColliders = Physics.OverlapSphere(transform.position,distToNotice);
        List<GameObject> GOs = new List<GameObject>();
        foreach (Collider possibleCollider in possibleColliders) {
            if (qualificationFunc(possibleCollider.gameObject)) {
                GOs.Add(possibleCollider.gameObject);
            }
        }
        return GOs.ToArray();
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
    
    
    GameObject GetClosestGameObject(GameObject[] GOs) {
        if (GOs.Length == 0) {
            return null;
        }
        float minDist = Vector3.Distance(transform.position,GOs[0].transform.position);
        GameObject minGO = GOs[0];
        foreach (GameObject go in GOs) {
            float dist = Vector3.Distance(transform.position,go.transform.position);
            if (dist < minDist) {
                minDist = dist;
                minGO = go;
            }
        }
        return minGO;
    }
    
}
