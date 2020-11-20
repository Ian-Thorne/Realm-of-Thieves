using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPhaseState : RoTState {

    public override void Enter() {
        base.Enter();
        Debug.Log("Entering MainPhaseState");

        rsm.ResetTemporaryVariables();
    }

    public override void Exit() {
        base.Exit();
    }

    protected override void AddListeners() {
        base.AddListeners();
        Card.CardInHandSelectedEvent += HandleCardInHandClicked;
    }

    protected override void RemoveListeners() {
        base.RemoveListeners();
        Card.CardInHandSelectedEvent -= HandleCardInHandClicked;
    }

    private void HandleCardInHandClicked(Card card) {
        if(card.GetType() == typeof(HenchmanCard)) {
            rsm.SetFirstSelectedCard(card);
            rsm.ChangeState<PlayingHenchmanState>();
        }
        //else if it's a TacticCard...
    }
}
