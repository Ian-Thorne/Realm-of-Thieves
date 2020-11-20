using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSpaceButton : MonoBehaviour
{
    [SerializeField]
    private BoardManager board;
    [SerializeField]
    private BoardSpaceEnum space;

    public void Clicked() {
        board.EmptyBoardSpaceClicked(space);
    }
}
