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

    //FIXME: The or for PlayStateEnum.DONE isn't super elegant, because it allows for much more than
    //       closing act effects. When you refactor the BoardManager to delay destroying henchmen,
    //       you may be able to let closing act effects trigger after being removed from the board,
    //       but before changing the playState from PlayStateEnum.BOARD to PlayStateEnum.DONE!

    public void DirectHealingToController(int healing) {
        if((playState == PlayStateEnum.BOARD) || (playState == PlayStateEnum.DONE)) {
            controller.ApplyHealing((uint) healing);
        } else {
            Debug.Log("Card trying to heal controller while not in play (or during closing act)...");
        }
    }

    public void DirectHealingToOpponent(int healing) {
        if((playState == PlayStateEnum.BOARD) || (playState == PlayStateEnum.DONE)) {
            controller.GetOpponent().ApplyHealing((uint) healing);
        } else {
            Debug.Log("Card trying to heal controller's opponent while not in play (or during closing act)...");
        }
    }

    public void DirectDamageToController(int damage) {
        if((playState == PlayStateEnum.BOARD) || (playState == PlayStateEnum.DONE)) {
            controller.ApplyDamage((uint) damage);
        } else {
            Debug.Log("Card trying to damage controller while not in play (or during closing act)...");
        }
    }

    public void DirectDamageToOpponent(int damage) {
        if((playState == PlayStateEnum.BOARD) || (playState == PlayStateEnum.DONE)) {
            controller.GetOpponent().ApplyDamage((uint) damage);
        } else {
            Debug.Log("Card trying to damage controller's opponent while not in play (or during closing act)...");
        }
    }

    //FIXME: All of the methods below need to check that they're in the correct state!

    public void DrawCardsForController(int number) {
        controller.GetDeck().DrawCards((uint) number);
    }

    public void DrawCardsForOpponent(int number) {
        controller.GetOpponent().GetDeck().DrawCards((uint) number);
    }

    public void GivePrizeCardToController() {
        controller.GetDeck().DrawPrizeCard();
    }

    public void GivePrizeCardToOpponent() {
        controller.GetOpponent().GetDeck().DrawPrizeCard();
    }

    public void GivePermanentManaToController(int mana) {
        controller.GetMana().IncreaseMaxManaBy((uint) mana);
    }

    public void GivePermanentManaToOpponent(int mana) {
        controller.GetOpponent().GetMana().IncreaseMaxManaBy((uint) mana);
    }

    public void GiveUsablePermanentManaToController(int mana) {
        controller.GetMana().IncreaseMaxManaBy((uint) mana);
        controller.GetMana().IncreaseTempManaBy((uint) mana);
    }

    public void GiveUsablePermanentManaToOpponent(int mana) {
        controller.GetOpponent().GetMana().IncreaseMaxManaBy((uint) mana);
        controller.GetOpponent().GetMana().IncreaseTempManaBy((uint) mana);
    }

    public void GiveTemporaryManaToController(int mana) {
        controller.GetMana().IncreaseTempManaBy((uint) mana);
    }

    public void GiveTemporaryManaToOpponent(int mana) {
        controller.GetOpponent().GetMana().IncreaseTempManaBy((uint) mana);
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
