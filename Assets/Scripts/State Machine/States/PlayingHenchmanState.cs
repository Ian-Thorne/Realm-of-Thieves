using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingHenchmanState : RoTState {

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering PlayingHenchmanState");
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void AddListeners() {
        base.AddListeners();
        BoardManager.EmptyBoardSpaceSelectedEvent += HandleBoardSpaceClicked;
    }

    protected override void RemoveListeners() {
        base.RemoveListeners();
        BoardManager.EmptyBoardSpaceSelectedEvent -= HandleBoardSpaceClicked;
    }

    private void HandleBoardSpaceClicked(BoardSpaceEnum space) {
        rsm.board.PutHenchmanAtSpace(space, (HenchmanCard) rsm.GetFirstSelectedCard());
        rsm.ChangeState<MainPhaseState>();
    }
}
