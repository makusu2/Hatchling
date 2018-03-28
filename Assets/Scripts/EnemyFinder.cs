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
    
    public GameObject GetClosestEnemy() {
        
        GameObject[] nearbyEnemies = GetNearbyEnemies();
        if (nearbyEnemies.Length > 0) {
            GameObject targetGO = GetClosestGameObject(nearbyEnemies);
            return targetGO;
        }
        return null;
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
    
    GameObject[] GetNearbyEnemies() {
        Dictionary<Factions,GameObject[]> nearbyEntities = GetNearbyFactionGOs();
        List<GameObject> enemies = new List<GameObject>();
        Factions[] enemyFactions = GetComponent<Faction>().GetEnemyFactions();
        foreach (Factions fact in nearbyEntities.Keys) {
            if(enemyFactions.Contains(fact)) {
                enemies.AddRange(nearbyEntities[fact]);
            }
        }
        return enemies.ToArray();
    }
    
    Dictionary<Factions,GameObject[]> GetNearbyFactionGOs() {
        Dictionary<Factions,List<GameObject>> factLists = new Dictionary<Factions,List<GameObject>>();
        Collider[] possibleColliders = Physics.OverlapSphere(transform.position,distToNotice);
        foreach (Collider possibleCollider in possibleColliders) {
            GameObject possibleGO = possibleCollider.gameObject;
            Factions possibleFaction;
            try {
                possibleFaction = possibleGO.GetComponent<Faction>().faction;
            }
            catch {
                continue;
            }
            if (!factLists.ContainsKey(possibleFaction)) {
                factLists[possibleFaction] = new List<GameObject>();
            }
            factLists[possibleFaction].Add(possibleGO);
        }
        Dictionary<Factions,GameObject[]> factArrays = new Dictionary<Factions,GameObject[]>();
        foreach(Factions fact in factLists.Keys) {
            factArrays[fact] = factLists[fact].ToArray();
        }
        return factArrays;
    }
}
