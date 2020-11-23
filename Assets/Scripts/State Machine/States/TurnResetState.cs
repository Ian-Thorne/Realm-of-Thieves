using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FIXME: This state should really start the UpkeepState (or whatever it's called),
//       which should call rsm.GetBoard().HandleBeginningOfTurn(), and then that
//       state should change to MainPhaseState when it's done!
public class TurnResetState : CardGameState {

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering TurnResetState");

        rsm.GetBoard().HandleEndOfTurn();

        rsm.ToggleActivePlayer();

        rsm.GetBoard().HandleBeginningOfTurn();

        StartCoroutine(StartNextTurn());
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

    /*
     * Changes state to MainPhaseState using a coroutine to delay the call a
     * frame, to prevent issues related to trying to change state inside of
     * the Enter() method.
     */
    private IEnumerator StartNextTurn() {
        yield return null;
        rsm.ChangeState<MainPhaseState>();
    }
}
