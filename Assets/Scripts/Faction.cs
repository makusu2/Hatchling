using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction : MonoBehaviour {
    
    public Factions faction;
    
    public Factions[] GetEnemyFactions() {
        return enemyFactions[faction];
    }
    
    public static Dictionary<Factions,Factions[]> enemyFactions = new Dictionary<Factions,Factions[]>(){
        {Factions.player, new Factions[]{Factions.wolf,}},
        {Factions.wolf, new Factions[]{Factions.player,}}
    };
}
public enum Factions { player, wolf, };