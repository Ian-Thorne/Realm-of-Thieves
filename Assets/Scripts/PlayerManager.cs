using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

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
    private GameObject activeIndicator;

    //--------------------
    // managing game state
    //--------------------

    void Start() {
        activeIndicator.SetActive(false);
    }

    public void HandleBeginningOfTurn() {
        //draw a card for the turn
        deck.DrawCards(1);
    }

    public void HandleEndOfTurn() {
        //clean up any cards that have been "destroyed"
        deck.CleanUpDestroyedCards();
    }

    //-----------------------------
    // healing and damaging methods
    //-----------------------------

    public void ApplyHealing(uint healing) {
        Debug.Log("Applying " + healing + " points of healing!");
    }

    public void ApplyDamage(uint damage) {
        Debug.Log("Applying " + damage + " points of damage!");
    }

    //--------------------------
    // interface-related methods
    //--------------------------

    public void ToggleActiveIndicator() {
        if(activeIndicator != null) {
            activeIndicator.SetActive(!activeIndicator.activeSelf);
        } else {
            Debug.Log("Player " + this.name + "'s activeIndicator was null...");
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
