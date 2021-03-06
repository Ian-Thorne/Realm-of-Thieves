﻿using System.Collections;
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

/*
 * A struct used only to pair GameObjects with BoardSpaceEnums in the Unity
 * inspector, then parse that data into a Dictionary
 */
[System.Serializable]
public struct BoardSpaceHolderStruct {
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

    //NOTE: this list is only public to provide a way to associate GameObjects with board
    //      spaces in-inspector, it's parsed into a Dictionary in Start(); the GameObjects
    //      are assumed to have exactly one child with the BoardSpaceButton script attached
    public List<BoardSpaceHolderStruct> boardSpaceHolders;

    private Dictionary<BoardSpaceEnum, HenchmanCard> board;
    private Dictionary<BoardSpaceEnum, GameObject> visualBoardSpaces;
    private RemoveQueue<BoardSpaceEnum> henchmenOrder;

    private PlayerManager player;
    private PlayerManager opponent;

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

        //initialize the visual spaces
        visualBoardSpaces = new Dictionary<BoardSpaceEnum, GameObject>();
        foreach(BoardSpaceHolderStruct spaceHolder in boardSpaceHolders) {
            visualBoardSpaces.Add(spaceHolder.space, spaceHolder.holder);
            BoardSpaceButton spaceButton = spaceHolder.holder.transform.GetChild(0).GetComponent<BoardSpaceButton>();
            spaceButton.SetupButton(this, spaceHolder.space);
        }

        //initialize the henchmenOrder (empty, like the board)
        henchmenOrder = new RemoveQueue<BoardSpaceEnum>();
    }

    void OnEnable() {
        HenchmanCard.HenchmanRequestedDestructionEvent += HandleRequestedHenchmanDestruction;
        PlayerManager.PlayerReceivedPrizeCardEvent += TriggerVengeanceEvents;
    }

    void OnDisable() {
        HenchmanCard.HenchmanRequestedDestructionEvent -= HandleRequestedHenchmanDestruction;
        PlayerManager.PlayerReceivedPrizeCardEvent -= TriggerVengeanceEvents;
    }

    //NOTE: Should be called after switching the active player!
    public void HandleBeginningOfTurn(PlayerManager activePlayer) {
        //call HandleBeginningOfTurn() on all HenchmanCards in play, in the order they came into play
        RemoveQueue<BoardSpaceEnum> newOrder = new RemoveQueue<BoardSpaceEnum>();
        while(!henchmenOrder.IsEmpty()) {
            BoardSpaceEnum henchmanSpace = henchmenOrder.Dequeue();
            HenchmanCard henchman = board[henchmanSpace];
            if(henchman.GetController() == activePlayer) {
                henchman.HandleBeginningOfTurn();
            }
            newOrder.Enqueue(henchmanSpace);
        }
        henchmenOrder = newOrder;
    }

    //NOTE: Should be called before switching the active player!
    public void HandleEndOfTurn(PlayerManager activePlayer) {
        //call HandleEndOfTurn() on all HenchmanCards in play, in the order they came into play
        RemoveQueue<BoardSpaceEnum> newOrder = new RemoveQueue<BoardSpaceEnum>();
        while(!henchmenOrder.IsEmpty()) {
            BoardSpaceEnum henchmanSpace = henchmenOrder.Dequeue();
            HenchmanCard henchman = board[henchmanSpace];
            if(henchman.GetController() == activePlayer) {
                henchman.HandleEndOfTurn();
            }
            newOrder.Enqueue(henchmanSpace);
        }
        henchmenOrder = newOrder;
    }

    //----------------------------
    // handling player interaction
    //----------------------------

    public void EmptyBoardSpaceClicked(BoardSpaceEnum space) {
        Debug.Log("Empty board space " + space + " was clicked!");
        if(EmptyBoardSpaceSelectedEvent != null) {
            EmptyBoardSpaceSelectedEvent(space);
        }
    }

    public void EndTurnButtonClicked() {
        Debug.Log("The end turn button was clicked!");
        if(EndTurnEvent != null) {
            EndTurnEvent();
        }
    }

    //------------------------
    // affecting cards in play
    //------------------------

    public bool CanPutHenchmanAtSpace(HenchmanCard henchman, BoardSpaceEnum space) {
        //henchman can be placed if the space is on it's controller's side
        bool onControllersSide = false;
        if(henchman.GetController() == player) {
            if((space == BoardSpaceEnum.P1) || (space == BoardSpaceEnum.P2) || (space == BoardSpaceEnum.P3) ||
                    (space == BoardSpaceEnum.P4) || (space == BoardSpaceEnum.P5)) {
                onControllersSide = true;
            }
        } else if(henchman.GetController() == opponent) {
            if((space == BoardSpaceEnum.O1) || (space == BoardSpaceEnum.O2) || (space == BoardSpaceEnum.O3) ||
                    (space == BoardSpaceEnum.O4) || (space == BoardSpaceEnum.O5)) {
                onControllersSide = true;
            }
        } else {
            Debug.Log("The henchman trying to be placed has a controller that's neither the player nor the opponent...");
        }

        if(onControllersSide) {
            //the space must also be empty
            return (board[space] == null);
        } else {
            return false;
        }
    }

    //should only be called if CanPutHenchmanAtSpace() returns true
    public void PutHenchmanAtSpace(HenchmanCard henchman, BoardSpaceEnum space) {
        //put it on the board
        board[space] = henchman;
        henchman.SetPlayState(PlayStateEnum.BOARD);
        henchman.SetLocation(space);

        //move the henchman visually
        VisuallyPutHenchmanAtSpace(henchman, space);

        //invoke its flashy event
        henchman.FlashyEvent.Invoke();
        //invoke all other henchmen's attention-seeker events
        TriggerAttentionSeekerEvents();

        //add it to the order
        henchmenOrder.Enqueue(space);
    }

    private void VisuallyPutHenchmanAtSpace(HenchmanCard henchman, BoardSpaceEnum space) {
        RectTransform henchmanTransform = henchman.GetComponent<RectTransform>();
        henchmanTransform.SetParent(visualBoardSpaces[space].transform);
        henchmanTransform.anchorMin = new Vector2(0.5f, 0.5f);
        henchmanTransform.anchorMax = new Vector2(0.5f, 0.5f);
        henchmanTransform.localScale = new Vector3(1f, 1f, 1f);
        henchmanTransform.localPosition = new Vector3(0f, 0f, 0f);
    }

    //returns true if successful
    public bool TryPutHenchmanOnPlayerSide(HenchmanCard henchman) {
        for(int i = (int) BoardSpaceEnum.P1; i <= (int) BoardSpaceEnum.P5; i++) {
            BoardSpaceEnum currSpace = (BoardSpaceEnum) i;
            if(CanPutHenchmanAtSpace(henchman, currSpace)) {
                PutHenchmanAtSpace(henchman, currSpace);
                return true;
            }
        }
        return false;
    }

    //returns true if successful
    public bool TryPutHenchmanOnOpponentSide(HenchmanCard henchman) {
        for(int i = (int) BoardSpaceEnum.O1; i<= (int) BoardSpaceEnum.O5; i++) {
            BoardSpaceEnum currSpace = (BoardSpaceEnum) i;
            if(CanPutHenchmanAtSpace(henchman, currSpace)) {
                PutHenchmanAtSpace(henchman, currSpace);
                return true;
            }
        }
        return false;
    }

    //NOTE: This may be a useful function to implement!
    // public bool SummonToken(HenchmanCard token, bool forPlayer)

    public bool CanRemoveHenchmanFromBoard(BoardSpaceEnum space) {
        //henchman can be removed if the space isn't empty
        return (board[space] != null);
    }

    //should only be called if CanRemoveHenchmanFromBoard() returns true
    public void RemoveHenchmanFromBoard(BoardSpaceEnum space) {
        HenchmanCard henchman = board[space];

        //remove it from the board
        board[space] = null;
        henchman.SetPlayState(PlayStateEnum.DONE);
        henchman.SetLocation(BoardSpaceEnum.NONE);

        //remove it from the order
        henchmenOrder.RemoveArbitrary(space);

        //remove the henchman visually
        henchman.gameObject.SetActive(false);

        //set up the henchman to be destroyed
        henchman.GetController().GetDeck().DestroyCard(henchman);

        //invoke its closing-act event
        henchman.ClosingActEvent.Invoke();
    }

    public bool CanHenchmenFight(BoardSpaceEnum playerSpace, BoardSpaceEnum opponentSpace) {
        bool onOpposingSides = OnOpposingSides(playerSpace, opponentSpace);
        if((board[playerSpace] != null) && (board[opponentSpace] != null) && onOpposingSides) {
            //if both spaces are occupied by henchman and they're on opposing sides of the board, they could be able to fight
            HenchmanCard playerHenchman = board[playerSpace];
            HenchmanCard opponentHenchman = board[opponentSpace];
            if(!playerHenchman.HasActedThisTurn()) {
                //if the henchman hasn't already acted this turn, the henchmen could fight
                if(playerHenchman.IsEllusive() || (!playerHenchman.IsEllusive() && !opponentHenchman.IsEllusive())) {
                    //if the attacking henchman is ellusive or neither henchman is ellusive, they can fight
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

        //check if any damage needs to trample over
        int excess = (int) playerHenchman.GetAttack() - opponentHenchman.GetHealth();
        if((excess > 0) && playerHenchman.IsOverAchiever()) {
            Debug.Log("ROLLING DAMAGE OVER");
            uint reduction = playerHenchman.GetAttack() - (uint) excess;
            HaveHenchmanAttackTargetPlayer(playerSpace, opponentHenchman.GetController(), damageReduction: reduction);
        }

        //apply damage to the henchmen after applying trample damage to prevent issues arising from the
        //trampler dying after taking damage from the other henchman, then not being at playerSpace anymore
        //FIXME: find a more elegant solution, since this could cause some unintuitive interactions with
        //       triggered effects on the other henchman, since it technically won't "die" until after
        //       damage has trampled over
        playerHenchman.ApplyDamage((int) opponentHenchman.GetAttack());
        opponentHenchman.ApplyDamage((int) playerHenchman.GetAttack());
        playerHenchman.ActionTaken();
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

    //------------------
    // affecting players
    //------------------

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
            target.ApplyDamage(henchman.GetAttack() - damageReduction);
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

    private void HandleRequestedHenchmanDestruction(BoardSpaceEnum space) {
        Debug.Log("The henchman at " + space + " has requested to be destroyed!");
        if(CanRemoveHenchmanFromBoard(space)) {
            Debug.Log("There was a henchman at that space. Destroying it!");
            RemoveHenchmanFromBoard(space);
        } else {
            Debug.Log("There wasn't a henchman at that space...");
        }
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

    //----------
    // accessors
    //----------

    public void SetPlayers(PlayerManager player, PlayerManager opponent) {
        this.player = player;
        this.opponent = opponent;
    }
}
