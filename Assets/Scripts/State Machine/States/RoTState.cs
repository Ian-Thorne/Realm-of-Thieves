using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoTState : State {

    protected RoTStateMachine rsm;

    void Awake() {
        rsm = GetComponent<RoTStateMachine>();
    }

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering a RoTState:");
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
