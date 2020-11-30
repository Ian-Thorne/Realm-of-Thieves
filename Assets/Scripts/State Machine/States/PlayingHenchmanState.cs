using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingHenchmanState : CardGameState {

    private HenchmanCard henchmanToBePlayed;

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering PlayingHenchmanState");

        //the card being played must be owned by the active player
        Debug.Assert(rsm.GetCardToBePlayed().GetController() == rsm.GetActivePlayer());

        //the card being played must be a henchman
        Debug.Assert(rsm.GetCardToBePlayed().GetType() == typeof(HenchmanCard));
        henchmanToBePlayed = (HenchmanCard) rsm.GetCardToBePlayed();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void AddListeners() {
        base.AddListeners();
        BoardManager.EmptyBoardSpaceSelectedEvent += HandleEmptyBoardSpaceSelected;
    }

    protected override void RemoveListeners() {
        base.RemoveListeners();
        BoardManager.EmptyBoardSpaceSelectedEvent -= HandleEmptyBoardSpaceSelected;
    }

    /*
     * This method is called when the player clicks an empty space on the board. It will
     * try to place the henchmanToBePlayed at that space on the board. Whether it succeeds
     * or not, it will then change the RoTStateMachine back to the MainPhaseState.
     */
    private void HandleEmptyBoardSpaceSelected(BoardSpaceEnum space) {
        if(rsm.GetBoard().CanPutHenchmanAtSpace(henchmanToBePlayed, space)) {
            rsm.GetActivePlayer().GetDeck().RemoveCardFromHand(henchmanToBePlayed, true);
            rsm.GetBoard().PutHenchmanAtSpace(henchmanToBePlayed, space);
        }
        rsm.ChangeState<MainPhaseState>();
    }
}
