using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingWithHenchmanState : RoTState {

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering AttackingWithHenchmanState");
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void AddListeners() {
        base.AddListeners();
    }

    protected override void RemoveListeners() {
        base.RemoveListeners();
    }
}
