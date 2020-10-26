using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayStateEnum {
    DECK = 0,
    HAND = 1,
    BOARD = 2,
    PRIZE = 3,
    DONE = 4
}

public abstract class Card : MonoBehaviour {

    //-----------------
    // member variables
    //-----------------

    //something to measure cost
    public string cardName;
    public bool isMasterPlan;
    //reference to PlayerManager
    //reference to BoardManager
    private PlayStateEnum playState;

    //--------------------
    // managing game state
    //--------------------

    protected virtual void Start() {
        //cards start in their owner's deck
        playState = PlayStateEnum.DECK;
    }

    public abstract void DestroyThisCard();

    //-------------------------
    // player-affecting methods
    //-------------------------

    public void DirectHealingToController(uint healing) {
        //
    }

    public void DirectHealingToOpponent(uint healing) {
        //
    }

    public void DirectDamageToController(uint damage) {
        //
    }

    public void DirectDamageToOpponent(uint damage) {
        //
    }

    public void DrawCardsForController(uint number) {
        //
    }

    public void DrawCardsForOpponent(uint number) {
        //
    }

    public void GivePrizeCardToController() {
        //
    }

    public void GivePrizeCardToOpponent() {
        //
    }
}
