using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum PlayStateEnum {
    DECK = 0,
    HAND = 1,
    BOARD = 2,
    PRIZE = 3,
    DONE = 4
}

public abstract class Card : MonoBehaviour {

    //-------
    // events
    //-------

    public delegate void CardInHandSelectedAction(Card card);
    public static event CardInHandSelectedAction CardInHandSelectedEvent;

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

    public void HandleCardClicked() {
        switch(playState) {
            case PlayStateEnum.DECK:
                ClickedWhileInDeck();
                break;
            case PlayStateEnum.HAND:
                ClickedWhileInHand();
                break;
            case PlayStateEnum.BOARD:
                ClickedWhileOnBoard();
                break;
            case PlayStateEnum.PRIZE:
                ClickedWhileAPrize();
                break;
            case PlayStateEnum.DONE:
                Debug.Log(cardName + " clicked while in the done state... That shouldn't be possible...");
                break;
            default:
                Debug.Log(cardName + " clicked without a valid state...");
                break;
        }
    }

    protected virtual void ClickedWhileInDeck() {
        Debug.Log(cardName + " clicked while in deck (?)");
    }

    protected virtual void ClickedWhileInHand() {
        Debug.Log(cardName + " clicked while in hand");
        if(CardInHandSelectedEvent != null) {
            CardInHandSelectedEvent(this);
        }
    }

    protected virtual void ClickedWhileOnBoard() {
        Debug.Log(cardName + " clicked while on board");
    }

    protected virtual void ClickedWhileAPrize() {
        //FIXME: don't reveal the card's name when it's a prize card
        Debug.Log(cardName + " clicked while being a prize card");
    }

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
