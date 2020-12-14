using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    //FIXME: This is only serialized for debugging, it shouldn't be!
    [SerializeField]
    private List<Card> cardsToDestroy;

    private GameObject visualDeck;
    private GameObject visualHand;
    private bool handVisible;
    private GameObject visualPrizeCards;
    private Text visualDeckText;
    private Text visualPrizeCardText;

    //--------------------
    // managing game state
    //--------------------

    void Start() {
        //set up visual objects
        visualDeck = transform.Find(Constants.DeckObjectName).gameObject;
        if(visualDeck == null) {
            Debug.Log("Didn't find a visualDeck game object for the DeckManager named " + this.name + "...");
        }
        visualDeckText = visualDeck.transform.Find(Constants.DeckCountIndicatorName).gameObject.GetComponent<Text>();
        if(visualDeckText == null) {
            Debug.Log("Didn't find a visualDeckText game object for the DeckManager named " + this.name + "...");
        }
        visualHand = transform.Find(Constants.HandObjectName).gameObject;
        if(visualHand == null) {
            Debug.Log("Didn't find a visualHand game object for the DeckManager named " + this.name + "...");
        }
        visualPrizeCards = transform.Find(Constants.PrizeCardsObjectName).gameObject;
        if(visualPrizeCards == null) {
            Debug.Log("Didn't find a visualPrizeCards game object for the DeckManager named " + this.name + "...");
        }
        visualPrizeCardText = visualPrizeCards.transform.Find(Constants.PrizeCardIndicatorName).gameObject.GetComponent<Text>();
        if(visualPrizeCardText == null) {
            Debug.Log("Didn't find a visualPrizeCardText game object for the DeckManager named " + this.name + "...");
        }

        //instantiate the cards in the deck
        deck = new List<Card>();
        foreach(Card card in cardsInDeck) {
            Card instantiatedCard = Instantiate(card, visualDeck.transform);
            //set the card's controller and play state correctly
            instantiatedCard.SetPlayState(PlayStateEnum.DECK);
            instantiatedCard.SetController(owner);
            instantiatedCard.gameObject.SetActive(false);
            deck.Add(instantiatedCard);
        }
        if((uint) deck.Count < Constants.MinDeckSize) {
            Debug.Log("The DeckManager named " + this.name + " didn't have at least " + Constants.MinDeckSize + " cards in it...");
        }

        ShuffleDeck();
        Debug.Log("Shuffled Deck:");
        foreach(Card card in deck) {
            Debug.Log(card.GetName());
        }

        //set three cards aside as prize cards
        prizeCards = new List<Card>();
        for(uint i = 0; i < Constants.NumPrizeCards; i++) {
            Card prizeCard = RemoveTopCardOfDeck();
            Debug.Log("Adding prize card " + prizeCard.name);
            prizeCard.SetPlayState(PlayStateEnum.PRIZE);
            prizeCards.Add(prizeCard);
            //put the card in the prize card pile visually
            prizeCard.transform.SetParent(visualPrizeCards.transform);
        }
        visualPrizeCardText.text = prizeCards.Count.ToString();

        //draw the player's opening hand with handVisible to leave the cards face-up
        handVisible = true;
        hand = new List<Card>();
        DrawCards(Constants.StartingHandSize);
        //then flip the cards face-down, the RoTStateMachine will flip the active player's hand back over
        ToggleHandVisibility();

        //initialize the cardsToDestroy list
        cardsToDestroy = new List<Card>();
    }

    public void ShuffleDeck() {
        //shuffle with the Fisher-Yates algorithm
        int indexA = deck.Count;
        while(indexA > 1) {
            indexA--;
            int indexB = Random.Range(0, indexA + 1);
            Card temp = deck[indexB];
            deck[indexB] = deck[indexA];
            deck[indexA] = temp;
        }
    }

    public void ShuffleCardIntoDeck(Card card) {
        if(card == null) {
            Debug.Log("Trying to shuffle a null card into the deck owned by the DeckManager named " + this.name + "...");
        }
        deck.Add(card);
        visualDeckText.text = deck.Count.ToString();
        ShuffleDeck();
    }

    public void DestroyCard(Card card) {
        if((deck.Contains(card)) || (hand.Contains(card)) || (prizeCards.Contains(card))) {
            Debug.Log("Trying to destroy a card that belongs to deck, hand, or prize cards owned the DeckManager named " + this.name + "...");
        } else {
            Debug.Log("Adding " + card.GetName() + " to the list of cardsToDestroy!");
            cardsToDestroy.Add(card);
        }
    }

    public void RemoveCardFromHand(Card card, bool played) {
        if(!hand.Contains(card)) {
            Debug.Log("Trying to remove the card " + card.name + " from the hand owned by the DeckManager named " + this.name + ", but it's not in its hand...");
        } else {
            hand.Remove(card);
            if(played) {
                if(card.GetType() == typeof(HenchmanCard)) {
                    card.SetPlayState(PlayStateEnum.BOARD);
                }
                //FIXME: do something for TacticCards, once implemented
            } else {
                card.SetPlayState(PlayStateEnum.DONE);
                DestroyCard(card);
            }
        }
    }

    //this is public for bounce effects
    public void PutCardInHand(Card card) {
        card.SetPlayState(PlayStateEnum.HAND);
        hand.Add(card);
        VisuallyPutCardInHand(card);
    }

    public void DrawCards(uint numCards) {
        for(uint i = 0; i < numCards; i++) {
            DrawCard();
        }
    }

    //returns the number of prize cards left to be collected
    public void DrawPrizeCard() {
        if(prizeCards.Count > 0) {
            int lastIndex = prizeCards.Count - 1;
            Card prizeCard = prizeCards[lastIndex];
            prizeCards.RemoveAt(lastIndex);
            visualPrizeCardText.text = prizeCards.Count.ToString();
            PutCardInHand(prizeCard);
            owner.HandleReceivedPrizeCard(Constants.NumPrizeCards - (uint) prizeCards.Count);
        } else {
            Debug.Log("Trying to draw a prize card from a DeckManager with none left...");
        }
    }

    /*
     * This method is called at the end of every turn. This allows "destroyed" cards to trigger any
     * closing act effects, then truly be cleaned up when those effects are done.
     *
     * It should be called by the owning PlayerManager.
     */
    public void CleanUpDestroyedCards() {
        //destroying in foreach may cause problems
        foreach(Card card in cardsToDestroy) {
            if(card.GetPlayState() != PlayStateEnum.DONE) {
                Debug.Log("Destroying a card that wasn't in the DONE state...");
            }
            if((card.GetType() == typeof(HenchmanCard)) && ((HenchmanCard) card).GetLocation() != BoardSpaceEnum.NONE) {
                Debug.Log("Destroying a henchman card that wasn't on board space NONE...");
            }
            Destroy(card.gameObject);
        }
        cardsToDestroy = new List<Card>();
    }

    //--------------------------
    // interface-related methods
    //--------------------------

    /*
     * This method will call Flip() on all cards in this DeckManager's hand. It's called at
     * the beginning and end of each players' turn to make sure their hand is only visible
     * during their own turn.
     *
     * Note: If the game ever stops having both players use the same computer, this method
     *       likely won't be needed anymore.
     */
    public void ToggleHandVisibility() {
        handVisible = !handVisible;
        foreach(Card card in hand) {
            card.Flip();
        }
    }

    //---------------
    // helper methods
    //---------------

    private Card RemoveTopCardOfDeck() {
        Debug.Log("Removing top card of " + this.name + "'s deck!");
        if(deck.Count > 0) {
            int lastIndex = deck.Count - 1;
            Card topCard = deck[lastIndex];
            deck.RemoveAt(lastIndex);
            visualDeckText.text = deck.Count.ToString();
            return topCard;
        } else {
            //if there are no cards left, return null
            return null;
        }
    }

    private void VisuallyPutCardInHand(Card card) {
        if(!handVisible) {
            card.Flip();
        }
        card.transform.SetParent(visualHand.transform);
        card.transform.localRotation = Quaternion.identity;
        card.transform.localScale = new Vector3(1f, 1f, 1f);
        card.gameObject.SetActive(true);
    }

    private void DrawCard() {
        Card drawnCard = RemoveTopCardOfDeck();
        if(drawnCard == null) {
            DrawPrizeCard();
        } else {
            PutCardInHand(drawnCard);
        }
    }
}
