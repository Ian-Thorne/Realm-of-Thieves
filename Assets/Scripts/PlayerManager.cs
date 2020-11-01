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

    //--------------------
    // managing game state
    //--------------------

    void Start() {
        //
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

    //-----------------
    // accessor methods
    //-----------------

    public PlayerManager GetOpponent() {
        return opponent;
    }
}
