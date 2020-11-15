using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    //-----------------
    // member variables
    //-----------------

    protected State currentState;
    protected bool changingState;

    public void ChangeState<T>() where T : State {
        T newState = GetComponent<T>();
        if(newState == null) {
            //make an instance of the state if there wasn't one
            newState = gameObject.AddComponent<T>();
        }

        if((currentState != newState) && !changingState) {
            changingState = true;
            if(currentState != null) {
                currentState.Exit();
            }

            currentState = newState;

            if(currentState != null) {
                currentState.Enter();
            }
            changingState = false;
        }
    }
}
