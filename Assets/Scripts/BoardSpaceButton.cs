using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceButton : MonoBehaviour
{
    private BoardManager board;
    private BoardSpaceEnum space;

    public void SetupButton(BoardManager gameBoard, BoardSpaceEnum buttonSpace) {
        board = gameBoard;
        space = buttonSpace;
    }

    public void Clicked() {
        if(board != null) {
            board.EmptyBoardSpaceClicked(space);
        } else {
            Debug.Log("A BoardSpaceButton was clicked, but it didn't have its reference to the BoardManager set...");
        }
    }
}
