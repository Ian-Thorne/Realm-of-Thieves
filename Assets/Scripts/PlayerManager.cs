using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    //-----------------
    // member variables
    //-----------------

    //health representation
    //something related to prize card thresholds
    //DeckManager reference

    [SerializeField]
    private PlayerManager opponent;

    [SerializeField]
    private GameObject activeIndicator;

    //--------------------
    // managing game state
    //--------------------

    void Start() {
        activeIndicator.SetActive(false);
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
}
