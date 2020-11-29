using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour {

    //-----------------
    // member variables
    //-----------------

    [SerializeField]
    private PlayerManager owner;
    
    //NOTE: This is a public list that will be filled with prefabs, which will be instantiated
    //      and stored in the list called "deck." This allows for cards to be shuffled into a
    //      player's deck during the game.
    public List<Card> cardsInDeck;
    private List<Card> deck;
    private List<Card> hand;
    private List<Card> prizeCards;

    private GameObject visualDeck;
    private GameObject visualHand;
    private GameObject visualPrizeCards;

    //--------------------
    // managing game state
    //--------------------

    void Start() {
        //set up visual objects
        visualDeck = transform.Find(Constants.DeckObjectName).gameObject;
        if(visualDeck == null) {
            Debug.Log("Didn't find a visualDeck game object for the DeckManager named " + this.name + "...");
        }
        visualHand = transform.Find(Constants.HandObjectName).gameObject;
        if(visualHand == null) {
            Debug.Log("Didn't find a visualHand game object for the DeckManager named " + this.name + "...");
        }
        visualPrizeCards = transform.Find(Constants.PrizeCardsObjectName).gameObject;
        if(visualPrizeCards == null) {
            Debug.Log("Didn't find a visualPrizeCards game object for the DeckManager named " + this.name + "...");
        }

        //instantiate the cards in the deck
        deck = new List<Card>();
        foreach(Card card in cardsInDeck) {
            Card instantiatedCard = Instantiate(card, visualDeck.transform);
            instantiatedCard.gameObject.SetActive(false);
            deck.Add(instantiatedCard);
        }
        ShuffleDeck();

        //set three cards aside as prize cards
        prizeCards = new List<Card>();

        //draw the player's opening hand
        hand = new List<Card>();
    }

    private void ShuffleDeck() {
        //shuffle with the Fisher-Yates algorithm
        int indexA = deck.Count;
        while(indexA > 1) {
            indexA--;
            int indexB = Random.Range(0, indexA + 1);
            Card temp = deck[indexB];
            deck[indexB] = deck[indexA];
            deck[indexA] = deck[indexB];
        }
    }

    public void ShuffleCardIntoDeck(Card card) {
        deck.Add(card);
        ShuffleDeck();
    }
}
