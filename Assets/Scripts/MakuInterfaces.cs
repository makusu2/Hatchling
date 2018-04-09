using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface ItemHolderInt {
    int CountOf(string item);
    void AddItem(string item);
    void RemoveItem(string item);
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