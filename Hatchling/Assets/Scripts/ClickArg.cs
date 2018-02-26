using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickArg{
    public PlayerBehavior player;
    public string currentItem;
    
	public ClickArg(PlayerBehavior player, string currentItem) {
        this.player = player;
        this.currentItem = currentItem;
    }
}
