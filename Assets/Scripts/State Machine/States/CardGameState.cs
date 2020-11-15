using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameState : State {

    protected RoTStateMachine rsm;

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering a CardGameState:");
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
