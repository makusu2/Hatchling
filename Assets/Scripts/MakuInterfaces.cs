using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface ItemHolderInt {
    int CountOf(string item);
    void AddItem(string item);
    void RemoveItem(string item);
}

interface ContainerInt : ItemHolderInt{
    void CreateIcons();
    void DestroyIcons();
    void ItemMovedTo(string item);
    void ItemTakenFrom(string item);
}

interface ItemActivator {
    void ActivateItem();
}

interface ReceiveSwing {
    void GetSwungAt();
    AudioClip SuccessHitSound {get;}
    AudioClip FailHitSound{get;}
    GameObject Particles{get;}
}

interface WaterEnterer {
    void OnWaterEnter();
    void OnWaterExit();
    bool InWater{get;set;}
}