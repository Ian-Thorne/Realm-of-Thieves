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

        rsm.GetBoard().HandleEndOfTurn(rsm.GetActivePlayer());
        rsm.GetActivePlayer().HandleEndOfTurn();

        rsm.ToggleActivePlayer();

        //now that we've toggled the active player, this coroutine will start that player's turn
        //updating the game state and changing to their MainPhaseState
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
        //add a small delay to give players a chance to swap who's playing without seeing the other's hand
        yield return new WaitForSeconds(Constants.TurnDelay);

        rsm.GetBoard().HandleBeginningOfTurn(rsm.GetActivePlayer());
        rsm.GetActivePlayer().HandleBeginningOfTurn(rsm.GetShouldDrawOnTurnStart());

        rsm.ChangeState<MainPhaseState>();
    }
}
