using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveQueue<T> {

    //-----------------
    // member variables
    //-----------------

    LinkedList<T> internalStorage;

    public RemoveQueue() {
        internalStorage = new LinkedList<T>();
    }

    public void Enqueue(T value) {
        internalStorage.AddLast(value);
    }

    public T Dequeue() {
        if(internalStorage.First != null) {
            T first = internalStorage.First.Value;
            internalStorage.RemoveFirst();
            return first;
        } else {
            Debug.Log("The RemoveQueue was emtpy when trying to dequeue.");
            return default(T);
        }
    }

    public bool RemoveArbitrary(T value) {
        return internalStorage.Remove(value);
    }

    public bool IsEmpty() {
        return (internalStorage.Count == 0);
    }
}
