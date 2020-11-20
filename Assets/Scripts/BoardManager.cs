using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static BoardSpaceEnum;

[System.Serializable]
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

[System.Serializable]
public struct BoardSpaceStruct {
    public BoardSpaceEnum space;
    public GameObject holder;
}

public class BoardManager : MonoBehaviour {

    //-------
    // events
    //-------

    public delegate void EmptyBoardSpaceSelectedAction(BoardSpaceEnum space);
    public static event EmptyBoardSpaceSelectedAction EmptyBoardSpaceSelectedEvent;

    public delegate void EndTurnAction();
    public static event EndTurnAction EndTurnEvent;

    //-----------------
    // member variables
    //-----------------

    public List<BoardSpaceStruct> boardSpaceHolders;

    private Dictionary<BoardSpaceEnum, HenchmanCard> board;
    private Dictionary<BoardSpaceEnum, GameObject> boardSpaces;
    private RemoveQueue<BoardSpaceEnum> henchmenOrder;
    private List<Card> cardsToDestroy;

    //--------------------
    // managing game state
    //--------------------

    void Start() {
        //initialize the board to be empty
        board = new Dictionary<BoardSpaceEnum, HenchmanCard>();
        board.Add(P1, null);
        board.Add(P2, null);
        board.Add(P3, null);
        board.Add(P4, null);
        board.Add(P5, null);
        board.Add(O1, null);
        board.Add(O2, null);
        board.Add(O3, null);
        board.Add(O4, null);
        board.Add(O5, null);

        boardSpaces = new Dictionary<BoardSpaceEnum, GameObject>();
        foreach(BoardSpaceStruct space in boardSpaceHolders) {
            boardSpaces.Add(space.space, space.holder);
        }

        //initialize the henchmenOrder (empty, like the board)
        henchmenOrder = new RemoveQueue<BoardSpaceEnum>();

        //initialize the list of cards to destroy at EOT
        cardsToDestroy = new List<Card>();
    }

    //called at the end of every turn, so "destroyed" cards can trigger closing act effects,
    //then truly be cleaned up when those effects are done
    private void CleanUpDestroyedCards() {
        //destroying in foreach may cause problems
        foreach(Card card in cardsToDestroy) {
            if(card.GetPlayState() != PlayStateEnum.DONE) {
                Debug.Log("Destroying a card that wasn't in the DONE state...");
            }
            Destroy(card.gameObject);
        }
    }

    //----------------------------
    // interfacing with the player
    //----------------------------

    public void EmptyBoardSpaceClicked(BoardSpaceEnum space) {
        Debug.Log("Space " + space + " clicked!");
        if(EmptyBoardSpaceSelectedEvent != null) {
            EmptyBoardSpaceSelectedEvent(space);
        }
    }

    public void EndTurnButtonClicked() {
        Debug.Log("End turn button clicked!");
        if(EndTurnEvent != null) {
            EndTurnEvent();
        }
    }

    //------------------------
    // affecting cards in play
    //------------------------

    public bool CanPutHenchmanAtSpace(BoardSpaceEnum space) {
        //henchman can be placed if the space is empty
        return (board[space] == null);
    }

    private void VisiblyPlaceHenchmanAtSpace(BoardSpaceEnum space, HenchmanCard henchman) {
        GameObject boardSpace = boardSpaces[space];
        henchman.gameObject.transform.parent = boardSpace.transform;
    }

    //should only be called if CanPutHenchmanAtSpace() returns true
    public void PutHenchmanAtSpace(BoardSpaceEnum space, HenchmanCard henchman) {
        //put it on the board
        board[space] = henchman;
        henchman.SetPlayState(PlayStateEnum.BOARD);
        VisiblyPlaceHenchmanAtSpace(space, henchman);

        //invoke its flashy event
        henchman.FlashyEvent.Invoke();
        //invoke all other henchmen's attention-seeker events
        TriggerAttentionSeekerEvents();

        //add it to the order
        henchmenOrder.Enqueue(space);
    }

    //returns true if successful
    public bool TryPutHenchmanOnPlayerSide(HenchmanCard henchman) {
        for(int i = (int) BoardSpaceEnum.P1; i <= (int) BoardSpaceEnum.P5; i++) {
            BoardSpaceEnum currSpace = (BoardSpaceEnum) i;
            if(CanPutHenchmanAtSpace(currSpace)) {
                PutHenchmanAtSpace(currSpace, henchman);
                return true;
            }
        }
        return false;
    }

    //returns true if successful
    public bool TryPutHenchmanOnOpponentSide(HenchmanCard henchman) {
        for(int i = (int) BoardSpaceEnum.O1; i<= (int) BoardSpaceEnum.O5; i++) {
            BoardSpaceEnum currSpace = (BoardSpaceEnum) i;
            if(CanPutHenchmanAtSpace(currSpace)) {
                PutHenchmanAtSpace(currSpace, henchman);
                return true;
            }
        }
        return false;
    }

    public bool CanRemoveHenchmanFromBoard(BoardSpaceEnum space) {
        //henchman can be removed if the space isn't empty
        return (board[space] != null);
    }

    //should only be called if CanRemoveHenchmanFromBoard() returns true
    public void RemoveHenchmanFromBoard(BoardSpaceEnum space) {
        HenchmanCard henchman = board[space];

        //remove it from the board
        board[space] = null;

        //remove it from the order
        henchmenOrder.RemoveArbitrary(space);

        //invoke its closing-act event
        henchman.ClosingActEvent.Invoke();

        //change its play state
        henchman.SetPlayState(PlayStateEnum.DONE);
    }

    public bool CanHenchmenFight(BoardSpaceEnum playerSpace, BoardSpaceEnum opponentSpace) {
        bool onOpposingSides = OnOpposingSides(playerSpace, opponentSpace);
        if((board[playerSpace] != null) && (board[opponentSpace] != null) && onOpposingSides) {
            //if both spaces are occupied by henchman and they're on opposing sides of the board, they could be able to fight
            HenchmanCard playerHenchman = board[playerSpace];
            HenchmanCard opponentHenchman = board[opponentSpace];
            if(!playerHenchman.HasActedThisTurn()) {
                //if the henchman hasn't already acted this turn, the henchmen could fight
                if((!playerHenchman.IsEllusive() && !opponentHenchman.IsEllusive()) || (playerHenchman.IsEllusive() && opponentHenchman.IsEllusive())) {
                    //if neither henchman is ellusive or both are, they can fight
                    return true;
                } else {
                    Debug.Log("Henchmen wouldn've been able to fight, except one was ellusive and the other wasn't...");
                    return false;
                }
            } else {
                Debug.Log("The henchman at playerSpace already moved this turn...");
                return false;
            }
        } else {
            if(board[playerSpace] == null) {
                Debug.Log("playerSpace passed to TryHaveHenchmanFight() didn't have a henchman in it...");
            }
            if(board[opponentSpace] == null) {
                Debug.Log("opponentSpace passed to TryHaveHenchmanFight() didn't have a henchman in it...");
            }
            if(!onOpposingSides) {
                Debug.Log("The two henchman trying to fight in TryHaveHenchmanFight() weren't on opposite sides of the board...");
            }
            return false;
        }
    }

    //should only be called if CanHenchmanFight() returns true
    public void HaveHenchmenFight(BoardSpaceEnum playerSpace, BoardSpaceEnum opponentSpace) {
        HenchmanCard playerHenchman = board[playerSpace];
        HenchmanCard opponentHenchman = board[opponentSpace];
        int excess = (int) playerHenchman.GetAttack() - opponentHenchman.GetHealth();
        playerHenchman.ApplyDamage((int) opponentHenchman.GetAttack());
        opponentHenchman.ApplyDamage((int) playerHenchman.GetAttack());
        if(excess > 0) {
            uint reduction = playerHenchman.GetAttack() - (uint) excess;
            HaveHenchmanAttackTargetPlayer(playerSpace, opponentHenchman.GetController(), damageReduction: reduction);
        }
        playerHenchman.ActionTaken();
    }

    //------------------
    // affecting players
    //------------------

    public void DamageTargetPlayer(PlayerManager target, uint damage) {
        target.ApplyDamage(damage);
    }

    public void HealTargetPlayer(PlayerManager target, uint healing) {
        target.ApplyHealing(healing);
    }

    public bool CanHenchmanAttackTargetPlayer(BoardSpaceEnum henchmanSpace, PlayerManager target) {
        HenchmanCard henchman = board[henchmanSpace];
        if(henchman != null) {
            if(!henchman.HasActedThisTurn()) {
                if(henchman.GetController() != target) {
                    return true;
                } else {
                    Debug.Log("Henchman can't attack a player because the player is its controller...");
                }
            } else {
                Debug.Log("Henchman can't attack a player because it already acted this turn...");
            }
        } else {
            Debug.Log("Henchman can't attack a player because it doesn't exist...");
        }
        return false;
    }

    //should only be called if CanHenchmanAttackTargetPlayer() returns true
    public void HaveHenchmanAttackTargetPlayer(BoardSpaceEnum henchmanSpace, PlayerManager target, uint damageReduction = 0) {
        HenchmanCard henchman = board[henchmanSpace];
        if(henchman.GetAttack() > 0) {
            DamageTargetPlayer(target, henchman.GetAttack() - damageReduction);
        }
        //trigger rush for combat damage
        henchman.RushEvent.Invoke();
        henchman.ActionTaken();
    }

    //---------------------
    // responding to events
    //---------------------

    private void TriggerVengeanceEvents(PlayerManager prizeCardRecipient) {
        //invoke vengeance events on relevant henchman, in the order they came into play
        RemoveQueue<BoardSpaceEnum> newOrder = new RemoveQueue<BoardSpaceEnum>();
        while(!henchmenOrder.IsEmpty()) {
            BoardSpaceEnum henchmanSpace = henchmenOrder.Dequeue();
            HenchmanCard henchman = board[henchmanSpace];
            if(henchman.GetController() != prizeCardRecipient) {
                //only invoke the event if the recipient of the prize card was not the henchman's controller
                henchman.VengeanceEvent.Invoke();
            }
            newOrder.Enqueue(henchmanSpace);
        }
        henchmenOrder = newOrder;
    }

    //this method assumes the new henchman hasn't yet been added to henchmenOrder
    private void TriggerAttentionSeekerEvents() {
        //invoke attention-seeker events on all henchman, in the order they came into play
        RemoveQueue<BoardSpaceEnum> newOrder = new RemoveQueue<BoardSpaceEnum>();
        while(!henchmenOrder.IsEmpty()) {
            BoardSpaceEnum henchmanSpace = henchmenOrder.Dequeue();
            HenchmanCard henchman = board[henchmanSpace];
            henchman.AttentionSeekerEvent.Invoke();
            newOrder.Enqueue(henchmanSpace);
        }
        henchmenOrder = newOrder;
    }

    //---------------
    // helper methods
    //---------------

    private bool OnOpposingSides(BoardSpaceEnum spaceOne, BoardSpaceEnum spaceTwo) {
        bool spaceOneIsValid = (int) spaceOne != 0;
        bool spaceTwoIsValid = (int) spaceTwo != 0;
        if(spaceOneIsValid && spaceTwoIsValid) {
            bool spaceOneIsPlayer = ((int) spaceOne > 0) && ((int) spaceOne <= 5);
            bool spaceTwoIsPlayer = ((int) spaceTwo > 0) && ((int) spaceTwo <= 5);
            if((spaceOneIsPlayer && !spaceTwoIsPlayer) || (!spaceOneIsPlayer && spaceTwoIsPlayer)) {
                return true;
            }
        }
        return false;
    }
}
