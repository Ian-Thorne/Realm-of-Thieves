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

    [SerializeField]
    private string cardName;
    [SerializeField]
    private uint cost;
    [SerializeField]
    private bool isMasterPlan;
    [SerializeField]
    private PlayerManager controller;
    [SerializeField] //FIXME: this is only serialized for debugging/testing purposes, it shouldn't be!
    private PlayStateEnum playState;
    [SerializeField]
    private Image cardBack;

    //--------------------
    // managing game state
    //--------------------

    protected virtual void Start() {
        //
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
            controller.ApplyHealing(healing);
        } else {
            Debug.Log("Card trying to heal controller while not in play...");
        }
    }

    public void DirectHealingToOpponent(uint healing) {
        if(playState == PlayStateEnum.BOARD) {
            controller.GetOpponent().ApplyHealing(healing);
        } else {
            Debug.Log("Card trying to heal controller's opponent while not in play...");
        }
    }

    public void DirectDamageToController(uint damage) {
        if(playState == PlayStateEnum.BOARD) {
            controller.ApplyDamage(damage);
        } else {
            Debug.Log("Card trying to damage controller while not in play...");
        }
    }

    public void DirectDamageToOpponent(uint damage) {
        if(playState == PlayStateEnum.BOARD) {
            controller.GetOpponent().ApplyDamage(damage);
        } else {
            Debug.Log("Card trying to damage controller's opponent while not in play...");
        }
    }

    public void DrawCardsForController(uint number) {
        controller.GetDeck().DrawCards(number);
    }

    public void DrawCardsForOpponent(uint number) {
        controller.GetOpponent().GetDeck().DrawCards(number);
    }

    public void GivePrizeCardToController() {
        controller.GetDeck().DrawPrizeCard();
    }

    public void GivePrizeCardToOpponent() {
        controller.GetOpponent().GetDeck().DrawPrizeCard();
    }

    public void GivePermanentManaToController(uint mana) {
        controller.GetMana().IncreaseMaxManaBy(mana);
    }

    public void GivePermanentManaToOpponent(uint mana) {
        controller.GetOpponent().GetMana().IncreaseMaxManaBy(mana);
    }

    public void GiveUsablePermanentManaToController(uint mana) {
        controller.GetMana().IncreaseMaxManaBy(mana);
        controller.GetMana().IncreaseTempManaBy(mana);
    }

    public void GiveUsablePermanentManaToOpponent(uint mana) {
        controller.GetOpponent().GetMana().IncreaseMaxManaBy(mana);
        controller.GetOpponent().GetMana().IncreaseTempManaBy(mana);
    }

    public void GiveTemporaryManaToController(uint mana) {
        controller.GetMana().IncreaseTempManaBy(mana);
    }

    public void GiveTemporaryManaToOpponent(uint mana) {
        controller.GetOpponent().GetMana().IncreaseTempManaBy(mana);
    }

    //-----------------
    // accessor methods
    //-----------------

    public string GetName() {
        return cardName;
    }

    public uint GetCost() {
        return cost;
    }

    public bool IsCardMasterPlan() {
        return isMasterPlan;
    }

    public PlayerManager GetController() {
        return controller;
    }

    public void SetController(PlayerManager player) {
        controller = player;
    }

    public PlayStateEnum GetPlayState() {
        return playState;
    }

    //NOTE: This method will only be called externally!
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
