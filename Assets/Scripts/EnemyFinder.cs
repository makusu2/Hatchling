using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EnemyFinder : MonoBehaviour {
    
    private float distToNotice;
    
    
    
    public void Setup(float distToNotice = 10.0f) {
        this.distToNotice = distToNotice;
    }
    
    public GameObject GetClosestWithProp(Func<GameObject,bool> qualificationFunc) {
        IEnumerable<GameObject> nearbyQualifiers = GetNearbyWithProp(qualificationFunc);
        return GetClosestGameObject(nearbyQualifiers);
    }
    public IEnumerable<GameObject> GetNearbyWithProp(Func<GameObject,bool> qualificationFunc, float? requiredDist = null) {
        if (requiredDist == null) {
            requiredDist = distToNotice;
        }
        foreach (Collider possibleCollider in Physics.OverlapSphere(transform.position,distToNotice)) {
            if (qualificationFunc(possibleCollider.gameObject)) {
                yield return possibleCollider.gameObject;
            }
        }
        yield break;
    }
    
    public bool IsEnemy(GameObject go) {
        Factions enemyFaction;
        if(go.GetComponent<PlayerBehavior>() != null) {
            enemyFaction = Factions.player;
        }
        else if (go.GetComponent<LivingEntity>() == null || go.GetComponent<LivingEntity>().IsDead) {
            return false;
        }
        else {
            enemyFaction = go.GetComponent<LivingEntity>().Fac;
        }
        Factions[] enemyFactions = GetComponent<LivingEntity>().GetEnemyFactions();
        return enemyFactions.Contains(enemyFaction);
    }
    public GameObject GetClosestEnemy() {
        Func<GameObject,bool> qualFunc = (GameObject go) => IsEnemy(go);
        return GetClosestWithProp(qualFunc);
    }
    
    public GameObject GetClosestFood() {
        Func<GameObject,bool> qualFunc = (GameObject go) => go.GetComponent<FoodBehavior>() != null;
        return GetClosestWithProp(qualFunc);
    }
    
    public GameObject GetClosestFire(float? requiredDist = null, int minHeatLevel = 3) {
        if (requiredDist == null) {
            requiredDist = distToNotice;
        }
        Func<GameObject,bool> qualFunc = (GameObject go) => go.GetComponent<Heated>() != null && go.GetComponent<Heated>().HeatLevel >= minHeatLevel;
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
