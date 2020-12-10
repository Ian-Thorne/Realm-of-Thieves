using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        //FIXME: this should be a private method
        activePlayer.ToggleActiveIndicator();
        //FIXME: this should be more elegant
        activePlayer.GetOpponent().GetDeck().ToggleHandVisibility();

        ResetTemporaryVariables();

        board.SetPlayers(player, opponent);

        ChangeState<MainPhaseState>();
    }

    void OnEnable() {
        PlayerManager.PlayerWonEvent += HandlePlayerWon;
    }

    void OnDisable() {
        PlayerManager.PlayerWonEvent -= HandlePlayerWon;
    }

    //FIXME: there needs to be a delay!
    private void HandlePlayerWon(PlayerManager winner) {
        if(winner == player) {
            SceneManager.LoadScene("Player Win Screen");
        } else {
            Debug.Assert(winner == opponent);
            SceneManager.LoadScene("Opponent Win Screen");
        }
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
        //FIXME: Remove or change the logic about toggling indicators!
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
