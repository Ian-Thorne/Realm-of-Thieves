using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : MonoBehaviour
{
    //-----------------
    // member variables
    //-----------------

    private uint maxMana;
    private uint tempMana;

    private Text visualMaxMana;
    private Text visualTempMana;

    //--------------------
    // managing game state
    //--------------------
    
    void Start() {
        visualMaxMana = transform.Find(Constants.MaxManaIndicatorName).gameObject.GetComponent<Text>();
        if(visualMaxMana == null) {
            Debug.Log("Didn't find a visualMaxMana object for the ManaManager named " + this.name + "...");
        }
        visualTempMana = transform.Find(Constants.TempManaIndicatorName).gameObject.GetComponent<Text>();
        if(visualTempMana == null) {
            Debug.Log("Didn't find a visualTempMana object for the ManaManager named " + this.name + "...");
        }

        SetMaxMana(0);
        SetTempMana(maxMana);
    }

    public void IncreaseMaxManaBy(uint amount) {
        SetMaxMana(maxMana + amount);
    }

    public void DecreaseMaxManaBy(uint amount) {
        uint newMaxMana = (amount > maxMana) ? 0 : maxMana - amount;
        SetMaxMana(newMaxMana);
        //if the new max is less than what the player has, decrease their temp mana as well
        if(newMaxMana < tempMana) {
            SetTempMana(newMaxMana);
        }
    }

    public void IncreaseTempManaBy(uint amount) {
        SetTempMana(tempMana + amount);
    }

    public void RefreshTempMana() {
        SetTempMana(maxMana);
    }

    public void HandleBeginningOfTurn() {
        IncreaseMaxManaBy(1);
        RefreshTempMana();
    }

    //----------------------------
    // cost-paying-related methods
    //----------------------------

    public bool CanPayCost(uint cost) {
        return (tempMana >= cost);
    }

    public void PayCost(uint cost) {
        Debug.Assert(CanPayCost(cost));
        SetTempMana(tempMana - cost);
    }

    //---------------
    // helper methods
    //---------------

    private void SetMaxMana(uint mana) {
        maxMana = mana;
        visualMaxMana.text = maxMana.ToString();
    }

    private void SetTempMana(uint mana) {
        tempMana = mana;
        visualTempMana.text = tempMana.ToString();
    }
}
