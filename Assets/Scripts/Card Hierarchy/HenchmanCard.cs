using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HenchmanCard : Card {

    //-------
    // events
    //-------

    public delegate void HenchmanInPlaySelectedAction(HenchmanCard card);
    public static event HenchmanInPlaySelectedAction HenchmanInPlaySelectedEvent;

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
    private bool hasActed;
    [SerializeField]
    private bool ellusive;
    [SerializeField]
    private bool eager;
    [SerializeField]
    private bool overAchiever;

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

    //---------------
    // keyword events
    //---------------

    public UnityEvent RushEvent;
    public UnityEvent VengeanceEvent;
    public UnityEvent FlashyEvent;
    public UnityEvent ClosingActEvent;
    public UnityEvent AttentionSeekerEvent;

    //--------------------
    // managing game state
    //--------------------

    protected override void Start() {
        base.Start();
        //since all cards start in their owner's deck, the location should start as none
        SetLocation(BoardSpaceEnum.NONE);

        tempAttackBuff = 0;
        permanentAttackBuff = 0;
        health = maxHealth;
        tempHealthBuff = 0;
        permanentHealthBuff = 0;

        UpdateAttackField();
        UpdateHealthField();

        //attach HandleBeingPlayed() to the FlashyEvent, since it'll be invoked when the
        //henchman is put into play
        FlashyEvent.AddListener(HandleBeingPlayed);

        //attach HandleBeingDestroyed() to the ClosingActEvent, since it'll be invoked
        //when the henchman is destroyed
        ClosingActEvent.AddListener(HandleBeingDestroyed);
    }

    //NOTE: This method should only be called when the henchman first enters play. It can
    //      be attached to the FlashyEvent.
    private void HandleBeingPlayed() {
        //the henchman is scheming (summoning sick) the first turn it comes into play,
        //unless it has the eager keyword
        if(eager) {
            AllowAction();
        } else {
            ActionTaken();
            schemingMarker.SetActive(true);
        }

        turnsInPlay = 0;
    }

    //NOTE: This method should only be called when the henchman is initially removed from
    //      play. IT can be attached to the ClosingActEvent.
    private void HandleBeingDestroyed() {
        SetLocation(BoardSpaceEnum.NONE);
        SetPlayState(PlayStateEnum.DONE);
    }

    //NOTE: This should only happen if it's in play!
    public void HandleBeginningOfTurn() {
        //remove summoning sickness
        if(hasActed) {
            AllowAction();
        }
    }

    //NOTE: This should only happen if it's in play!
    public void HandleEndOfTurn() {
        RemoveTempBuffsAndDebuffs();

        turnsInPlay++;
    }

    private void UpdateAttackField() {
        attackField.text = GetAttack().ToString();
    }

    private void UpdateHealthField() {
        healthField.text = GetHealth().ToString();
    }

    public void ActionTaken() {
        hasActed = true;
    }

    private void AllowAction() {
        hasActed = false;
        if(schemingMarker.activeSelf) {
            schemingMarker.SetActive(false);
        }
    }

    //FIXME: Should cause the BoardManager to call (Can)RemoveHenchmanFromBoard()!
    public override void RequestDestroy() {
        //request that the BoardManager destroy it
    }

    protected override void ClickedWhileOnBoard() {
        base.ClickedWhileOnBoard();
        if(HenchmanInPlaySelectedEvent != null) {
            HenchmanInPlaySelectedEvent(this);
        }
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
                RequestDestroy();
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
                RequestDestroy();
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
                RequestDestroy();
            } else {
                //update visuals
                UpdateHealthField();
            }
        }
    }

    //-----------------
    // accessor methods
    //-----------------
    
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

    public bool IsEllusive() {
        return ellusive;
    }

    public bool IsEager() {
        return eager;
    }

    public bool IsOverAchiever() {
        return overAchiever;
    }

    public void SetLocation(BoardSpaceEnum space) {
        location = space;
    }

    public BoardSpaceEnum GetLocation() {
        return location;
    }

    public bool HasActedThisTurn() {
        return hasActed;
    }
}
