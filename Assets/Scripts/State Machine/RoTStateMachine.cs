using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoTStateMachine : StateMachine {

    //-----------------
    // member variables
    //-----------------

    //game management references
    [SerializeField]
    private PlayerManager player;
    [SerializeField]
    private PlayerManager opponent;
    [SerializeField]
    private BoardManager board;

    //variables updated per-state
    private PlayerManager activePlayer;
    private Card cardToBePlayed;
    private HenchmanCard attackingHenchman;

    //--------------------
    // managing game state
    //--------------------

    //FIXME: move some of this logic to a setup state (?)
    void Start() {
        //FIXME: make sure the player's opponent is the opponent and the opponent's is the player
        //FIXME: randomly select the starting player
        activePlayer = player;
        ResetTemporaryVariables();

        board.SetPlayers(player, opponent);

        ChangeState<MainPhaseState>();
    }

    //-----------------
    // accessor methods
    //-----------------

    public PlayerManager GetPlayer() {
        return player;
    }

    public PlayerManager GetOpponent() {
        return opponent;
    }

    public BoardManager GetBoard() {
        return board;
    }

    public void ToggleActivePlayer() {
        activePlayer = activePlayer.GetOpponent();
    }

    public PlayerManager GetActivePlayer() {
        return activePlayer;
    }

    /*
     * This method is used to clear out any temporary variables stored in the RoTStateMachine
     * that are used only by particular states, so there is never incorrect information being
     * used by those states.
     */
    public void ResetTemporaryVariables() {
        cardToBePlayed = null;
        attackingHenchman = null;
    }

    public void SetCardToBePlayed(Card card) {
        cardToBePlayed = card;
    }

    public Card GetCardToBePlayed() {
        return cardToBePlayed;
    }

    public void SetAttackingHenchman(HenchmanCard henchman) {
        attackingHenchman = henchman;
    }

    public HenchmanCard GetAttackingHenchman() {
        return attackingHenchman;
    }
}
