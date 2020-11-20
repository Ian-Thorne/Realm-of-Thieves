using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoTStateMachine : StateMachine {

    //-----------------
    // member variables
    //-----------------

    public BoardManager board;

    private PlayerManager activePlayer;
    private Card firstSelectedCard;
    private Card secondSelectedCard;
    private BoardSpaceEnum selectedBoardSpace;

    void Start() {
        ChangeState<MainPhaseState>();
    }

    public void ResetTemporaryVariables() {
        firstSelectedCard = null;
        secondSelectedCard = null;
        selectedBoardSpace = BoardSpaceEnum.NONE;
    }

    public Card GetFirstSelectedCard() {
        return firstSelectedCard;
    }

    public void SetFirstSelectedCard(Card card) {
        firstSelectedCard = card;
    }

    public Card GetSecondSelectedCard() {
        return secondSelectedCard;
    }

    public BoardSpaceEnum GetSelectedBoardSpace() {
        return selectedBoardSpace;
    }
}
