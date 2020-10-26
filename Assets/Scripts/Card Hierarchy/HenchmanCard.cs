using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//FIXME: move this enum to BoardManager
public enum BoardSpaceEnum {
    NONE = 0,
    P1 = 1,
    P2 = 2,
    P3 = 3,
    P4 = 4,
    P5 = 5,
    O1 = 6,
    O2 = 7,
    O3 = 8,
    O4 = 9,
    O5 = 10
}

public class HenchmanCard : Card {

    //-----------------
    // member variables
    //-----------------

    //stat-related uints
    [SerializeField]
    private uint attack;
    private int tempAttackBuff;
    private int permanentAttackBuff;
    [SerializeField]
    private uint maxHealth;
    private uint health;
    private int tempHealthBuff;
    private int permanentHealthBuff;

    //mechanic-related flags
    private bool scheming;
    public bool ellusive;
    public bool eager;
    public bool overAchiever;

    //game-state-related members
    private BoardSpaceEnum location;
    private uint turnsInPlay;

    //-------------------------
    // references to components
    //-------------------------

    [SerializeField]
    private Text attackField;
    [SerializeField]
    private Text healthField;
    [SerializeField]
    private GameObject schemingMarker;
    //FIXME: delet this
    public GameObject destroyedMarker;

    //--------------------
    // managing game state
    //--------------------

    protected override void Start() {
        base.Start();
        //since all cards start in their owner's deck, the location should start as none
        location = BoardSpaceEnum.NONE;

        tempAttackBuff = 0;
        permanentAttackBuff = 0;
        health = maxHealth;
        tempHealthBuff = 0;
        permanentHealthBuff = 0;
        scheming = true;

        UpdateAttackField();
        UpdateHealthField();
        //the henchman is scheming (summoning sick) the first turn it comes into play,
        //unless it has the eager keyword
        if(eager) {
            StopScheming();
        }

        turnsInPlay = 0;
    }

    //NOTE: This should only happen if it's in play!
    //FIXME: This should be private post demo!
    public void HandleEndOfTurn() {
        //remove summoning sickness
        if(scheming) {
            StopScheming();
        }

        RemoveTempBuffsAndDebuffs();

        turnsInPlay++;
    }

    //FIXME: delet this
    public void ResetScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void UpdateAttackField() {
        attackField.text = GetAttack().ToString();
    }

    private void UpdateHealthField() {
        healthField.text = GetHealth().ToString();
    }

    private void StopScheming() {
        scheming = false;
        schemingMarker.SetActive(false);
    }

    public override void DestroyThisCard() {
        //request that the BoardManager destroy it
        destroyedMarker.SetActive(true);
    }

    //--------------------------------------------------
    // healing, damaging, buffing, and debuffing methods
    //--------------------------------------------------

    public void ApplyHealing(int healing) {
        if(healing >= 0) {
            health += (uint) healing;
            if(GetHealth() > GetMaxHealth()) {
                health = maxHealth;
            }

            //update visuals
            UpdateHealthField();
        }
    }

    public void ApplyDamage(int damage) {
        if(damage >= 0) {
            health -= (uint) damage;

            if(GetHealth() <= 0) {
                DestroyThisCard();
            } else {
                //update visuals
                UpdateHealthField();
            }
        }
    }

    public void ApplyTempAttackBuff(int attackBuff) {
        if(attackBuff >= 0) {
            tempAttackBuff += attackBuff;

            //update visuals
            UpdateAttackField();
        }
    }

    public void ApplyTempHealthBuff(int healthBuff) {
        if(healthBuff >= 0) {
            tempHealthBuff += healthBuff;

            //update visuals
            UpdateHealthField();
        }
    }

    public void ApplyTempAttackDebuff(int attackDebuff) {
        if(attackDebuff >= 0) {
            tempAttackBuff -= attackDebuff;

            //update visuals
            UpdateAttackField();
        }
    }

    public void ApplyTempHealthDebuff(int healthDebuff) {
        if(healthDebuff >= 0) {
            tempHealthBuff -= healthDebuff;

            if(GetHealth() <= 0) {
                DestroyThisCard();
            } else {
                //update visuals
                UpdateHealthField();
            }
        }
    }

    private void RemoveTempBuffsAndDebuffs() {
        tempAttackBuff = 0;
        tempHealthBuff = 0;

        //update visuals
        UpdateAttackField();
        UpdateHealthField();
    }

    public void ApplyPermanentAttackBuff(int attackBuff) {
        if(attackBuff >= 0) {
            permanentAttackBuff += attackBuff;

            //update visuals
            UpdateAttackField();
        }
    }

    public void ApplyPermanentHealthBuff(int healthBuff) {
        if(healthBuff >= 0) {
            permanentHealthBuff += healthBuff;

            //update visuals
            UpdateHealthField();
        }
    }

    public void ApplyPermanentAttackDebuff(int attackDebuff) {
        if(attackDebuff >= 0) {
            permanentAttackBuff -= attackDebuff;

            //update visuals
            UpdateAttackField();
        }
    }

    public void ApplyPermanentHealthDebuff(int healthDebuff) {
        if(healthDebuff >= 0) {
            permanentHealthBuff -= healthDebuff;

            if(GetHealth() <= 0) {
                DestroyThisCard();
            } else {
                //update visuals
                UpdateHealthField();
            }
        }
    }

    //----------
    // accessors
    //----------
    
    public uint GetAttack() {
        int totalAttack = ((int) attack) + tempAttackBuff + permanentAttackBuff;
        return (totalAttack >= 0) ? (uint) totalAttack : 0;
    }

    public int GetHealth() {
        return ((int) health) + tempHealthBuff + permanentHealthBuff;
    }

    public int GetMaxHealth() {
        return ((int) maxHealth) + tempHealthBuff + permanentHealthBuff;
    }
}
