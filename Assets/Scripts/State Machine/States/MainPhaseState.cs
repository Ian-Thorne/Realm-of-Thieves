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
        HenchmanCard.HenchmanInPlaySelectedEvent += HandleHenchmanInPlaySelected;
        BoardManager.EndTurnEvent += HandleEndOfTurn;
    }

    protected override void RemoveListeners() {
        base.RemoveListeners();
        Card.CardInHandSelectedEvent -= HandleCardInHandSelected;
        HenchmanCard.HenchmanInPlaySelectedEvent -= HandleHenchmanInPlaySelected;
        BoardManager.EndTurnEvent -= HandleEndOfTurn;
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
        if(rsm.GetActivePlayer().CanPlayCardFromHand(card)) {
            rsm.SetCardToBePlayed(card);
            if(card.GetType() == typeof(HenchmanCard)) {
                rsm.ChangeState<PlayingHenchmanState>();
            } else {
                Debug.Log("A non-HenchmanCard card in a player's hand was clicked");
            }
        }
    }

    /*
     * This method is called whenever the player clicks a henchman in play. It will cause the
     * RoTStateMachine to move to the AttackingWithHenchmanState as long as the henchman is
     * controlled by the active player and can still attack this turn.
     *
     * NOTE: Any behaviors related to clicking a henchman that has attacked or a henchman
     *       controlled by the opponent should be implemented in this method.
     */
    private void HandleHenchmanInPlaySelected(HenchmanCard henchman) {
        if(henchman.GetController() == rsm.GetActivePlayer()) {
            if(!henchman.HasActedThisTurn()) {
                rsm.SetAttackingHenchman(henchman);
                rsm.ChangeState<AttackingWithHenchmanState>();
            }
        }
    }

    /*
     * This method is called whenever the player clicks the end turn button. It will cause the
     * RoTStateMachine to move to the TurnResetState, where it will handle all EOT effects for
     * the player whose turn ended and all upkeep effects for the player whose turn just began.
     */
    private void HandleEndOfTurn() {
        rsm.ChangeState<TurnResetState>();
    }
}
