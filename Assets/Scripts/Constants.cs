using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {

    //--------------------------------
    // PlayerManager-related constants
    //--------------------------------

    public const uint FirstHealthThreshold = 5;
    public const uint SecondHealthThreshold = 10;
    public const uint ThirdHealthThreshold = 15;

    //------------------------------
    // DeckManager-related constants
    //------------------------------

    public const uint MinDeckSize = 9;
    public const uint StartingHandSize = 3;
    public const uint NumPrizeCards = 3;
    public const string DeckObjectName = "Deck";
    public const string DeckCountIndicatorName = "Number";
    public const string HandObjectName = "Hand";
    public const string PrizeCardsObjectName = "Prize Cards";
    public const string PrizeCardIndicatorName = "Number";

    //------------------------------
    // ManaManager-related constants
    //------------------------------

    public const string MaxManaIndicatorName = "Max Mana";
    public const string TempManaIndicatorName = "Temp Mana";
}
