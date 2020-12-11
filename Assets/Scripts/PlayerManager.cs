using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {

    //-------
    // events
    //-------

    public delegate void PlayerReceivedPrizeCardAction(PlayerManager recipient);
    public static event PlayerReceivedPrizeCardAction PlayerReceivedPrizeCardEvent;

    public delegate void PlayerSelectedAction(PlayerManager player);
    public static event PlayerSelectedAction PlayerSelectedEvent;

    public delegate void PlayerWonAction(PlayerManager winner);
    public static event PlayerWonAction PlayerWonEvent;

    //-----------------
    // member variables
    //-----------------

    //health representation
    //something related to prize card thresholds

    [SerializeField]
    private PlayerManager opponent;
    [SerializeField]
    private DeckManager deck;
    [SerializeField]
    private ManaManager mana;

    private uint currentHealth;
    private List<uint> prizeCardThresholds;

    [SerializeField]
    private GameObject activeIndicator;

    [SerializeField]
    private Text visualCurrentHealth;
    [SerializeField]
    private Text visualSecondHealth;
    [SerializeField]
    private Text visualThirdHealth;

    //--------------------
    // managing game state
    //--------------------

    void Start() {
        //prize card thresholds are according to the number the opponent has stolen
        prizeCardThresholds = new List<uint>();
        prizeCardThresholds.Add(Constants.FirstHealthThreshold);
        prizeCardThresholds.Add(Constants.SecondHealthThreshold);
        prizeCardThresholds.Add(Constants.ThirdHealthThreshold);
        //setting currentHealth to 0 here is fine, since StartPrizeCardThreshold() will
        //set it to the correct starting value
        currentHealth = 0;
        StartPrizeCardThreshold(0);

        activeIndicator.SetActive(false);
    }

    public void HandleBeginningOfTurn(bool shouldDraw) {
        ToggleActiveIndicator();
        deck.ToggleHandVisibility();

        //increment max mana by one
        mana.HandleBeginningOfTurn();

        if(shouldDraw) {
            //draw a card for the turn
            deck.DrawCards(1);
        }
    }

    public void HandleEndOfTurn() {
        //clean up any cards that have been "destroyed"
        deck.CleanUpDestroyedCards();

        deck.ToggleHandVisibility();
        ToggleActiveIndicator();
    }

    public void HandleReceivedPrizeCard(uint prizeCardsStolen) {
        if(prizeCardsStolen == Constants.NumPrizeCards) {
            if(PlayerWonEvent != null) {
                PlayerWonEvent(this);
            }
        } else if(prizeCardsStolen < Constants.NumPrizeCards) {
            opponent.StartPrizeCardThreshold((int) prizeCardsStolen);
            if(PlayerReceivedPrizeCardEvent != null) {
                PlayerReceivedPrizeCardEvent(this);
            }
        } else {
            Debug.Log("Player " + this.name + " received more than " + Constants.NumPrizeCards + " prize cards... (?)");
        }
    }

    public bool CanPlayCardFromHand(Card card) {
        //the card must be controlled by this PlayerManagerand cost less than its available mana
        bool controlledByMe = (card.GetController() == this);
        bool castable = mana.CanPayCost(card.GetCost());
        return controlledByMe && castable;
    }

    public void PlayCardFromHand(Card card) {
        Debug.Assert(CanPlayCardFromHand(card));
        deck.RemoveCardFromHand(card, true);
        mana.PayCost(card.GetCost());
    }

    //-----------------------------
    // healing and damaging methods
    //-----------------------------

    public void ApplyHealing(uint healing) {
        Debug.Log("Applying " + healing + " points of healing to " + this.name + "!");
        currentHealth += healing;
        UpdateVisualCurrentHealth();
    }

    public void ApplyDamage(uint damage) {
        Debug.Log("Applying " + damage + " points of damage to " + this.name + "!");
        currentHealth = (damage > currentHealth) ? 0 : currentHealth - damage;
        UpdateVisualCurrentHealth();
        if(currentHealth == 0) {
            //the opponent drawing a prize card will result in this PlayerManager calling
            //StartPrizeCardThreshold() with the appropriate number, so nothing else has
            //to be done here
            opponent.GetDeck().DrawPrizeCard();
        }
    }

    private void StartPrizeCardThreshold(int opponentsStolenPrizeCards) {
        //only update currentHealth if the opponent still has prize cards to steal
        if(opponentsStolenPrizeCards < (int) Constants.NumPrizeCards) {
            ApplyHealing(prizeCardThresholds[opponentsStolenPrizeCards]);
            UpdateVisualAllHealth(opponentsStolenPrizeCards);
        }
    }

    //--------------------------
    // interface-related methods
    //--------------------------

    private void UpdateVisualCurrentHealth() {
        visualCurrentHealth.text = currentHealth.ToString();
    }

    private void UpdateVisualAllHealth(int currentThreshold) {
        UpdateVisualCurrentHealth();

        //update the visual second and third healths, if needed based on the current threshold
        switch(currentThreshold) {
            case 0:
                visualSecondHealth.text = prizeCardThresholds[1].ToString();
                visualThirdHealth.text = prizeCardThresholds[2].ToString();
                break;
            case 1:
                visualSecondHealth.text = prizeCardThresholds[2].ToString();
                visualThirdHealth.text = "";
                break;
            case 2:
                visualSecondHealth.text = "";
                visualThirdHealth.text = "";
                break;
            default:
                Debug.Log("Trying to update a PlayerManager's visual health numbers when the opponent collected all of their prize cards...");
                break;
        }
    }

    private void ToggleActiveIndicator() {
        if(activeIndicator != null) {
            activeIndicator.SetActive(!activeIndicator.activeSelf);
        } else {
            Debug.Log("Player " + this.name + "'s activeIndicator was null...");
        }
    }

    public void PlayerClicked() {
        if(PlayerSelectedEvent != null) {
            PlayerSelectedEvent(this);
        }
    }

    //-----------------
    // accessor methods
    //-----------------

    public PlayerManager GetOpponent() {
        return opponent;
    }

    public DeckManager GetDeck() {
        return deck;
    }
}
