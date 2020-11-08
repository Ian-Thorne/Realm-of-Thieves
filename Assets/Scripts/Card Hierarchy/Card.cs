using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField]
    private string cardName;
    [SerializeField]
    private bool isMasterPlan;
    [SerializeField]
    private PlayerManager controller;
    [SerializeField]
    private BoardManager board;
    private PlayStateEnum playState;
    [SerializeField]
    private Image cardBack;

    //--------------------
    // managing game state
    //--------------------

    protected virtual void Start() {
        //cards start in their owner's deck
        playState = PlayStateEnum.DECK;
    }

    public abstract void RequestDestroy();

    //-------------------------
    // player-affecting methods
    //-------------------------

    public void DirectHealingToController(uint healing) {
        if(playState == PlayStateEnum.BOARD) {
            board.HealTargetPlayer(controller, healing);
        } else {
            Debug.Log("Card trying to heal controller while not in play...");
        }
    }

    public void DirectHealingToOpponent(uint healing) {
        if(playState == PlayStateEnum.BOARD) {
            board.HealTargetPlayer(controller.GetOpponent(), healing);
        } else {
            Debug.Log("Card trying to heal controller's opponent while not in play...");
        }
    }

    public void DirectDamageToController(uint damage) {
        if(playState == PlayStateEnum.BOARD) {
            board.DamageTargetPlayer(controller, damage);
        } else {
            Debug.Log("Card trying to damage controller while not in play...");
        }
    }

    public void DirectDamageToOpponent(uint damage) {
        if(playState == PlayStateEnum.BOARD) {
            board.DamageTargetPlayer(controller.GetOpponent(), damage);
        } else {
            Debug.Log("Card trying to damage controller's opponent while not in play...");
        }
    }

    public void DrawCardsForController(uint number) {
        //ask PlayerManager's DeckManager
    }

    public void DrawCardsForOpponent(uint number) {
        //ask PlayerManager's DeckManager
    }

    public void GivePrizeCardToController() {
        //ask PlayerManager's DeckManager
    }

    public void GivePrizeCardToOpponent() {
        //ask PlayerManager's DeckManager
    }

    //-----------------
    // accessor methods
    //-----------------

    public string GetName() {
        return cardName;
    }

    public bool IsCardMasterPlan() {
        return isMasterPlan;
    }

    public PlayerManager GetController() {
        return controller;
    }

    public PlayStateEnum GetPlayState() {
        return playState;
    }

    public void SetPlayState(PlayStateEnum state) {
        playState = state;
    }

    public void Flip() {
        cardBack.gameObject.SetActive(!cardBack.gameObject.activeSelf);
    }

    public bool IsFaceUp() {
        return !cardBack.gameObject.activeSelf;
    }
}
