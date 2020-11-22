using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPhaseState : CardGameState {

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering MainPhaseState");

        //reset the temporary variables in the RoTStateMachine, since this state will likely
        //result in performing new actions
        rsm.ResetTemporaryVariables();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void AddListeners() {
        base.AddListeners();
        Card.CardInHandSelectedEvent += HandleCardInHandSelected;
    }

    protected override void RemoveListeners() {
        base.RemoveListeners();
        Card.CardInHandSelectedEvent -= HandleCardInHandSelected;
    }

    /*
     * This method is called whenever the player clicks a card in their hand. It will cause
     * the RoTStateMachine to move to a different state, depending on whether or not it is a
     * HenchmanCard or a TacticCard. The card must be in that player's hand.
     *
     * FIXME: When paying costs is implemented, this method should always cause the
     *        RoTStateMachine to go to that state, and that state should determine
     *        where to go from there.
     */
    private void HandleCardInHandSelected(Card card) {
        //don't let the player play cards from their opponent's hand
        if(card.GetController() == rsm.GetActivePlayer()) {
            rsm.SetCardToBePlayed(card);
            if(card.GetType() == typeof(HenchmanCard)) {
                rsm.ChangeState<PlayingHenchmanState>();
            } else {
                Debug.Log("A non-HenchmanCard card in a player's hand was clicked");
                rsm.ChangeState<MainPhaseState>();
            }
        }
    }
}
