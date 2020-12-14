## Realm of Thieves ##

A collectable card game being developed by me, Ian Thorne!

### Rules ###

#### The Start of the Game ###

Each player starts the game with a thirty card deck. At the beginning of the game, three cards from each player's deck will be set aside as "Prize Cards," and each player will draw three cards as their starting hand. Each player begins the game with zero mana.

#### The Turn Cycle ####

At the start of each turn, players will gain one mana and (if it's not the first turn) draw a card. They'll then be allowed to spend their mana on any cards they're able to pay for and attack with any henchman they have in play that are eligible to attack.

To play a henchman card (which are the only cards in the game, at the moment), you must have the required mana, indicated in the card's top right corner. If you do, you can click the card in your hand, then one of the five board slots on your side of the game board. If those five slots are full, you won't be able to play any more henchmen until you free up some space. If you'd like to change your mind, clicking on the card you originally tried to play will back out, letting you make another decision.

(Almost) All henchmen cards must "scheme" for the first turn they're in play, preventing them from attacking that turn. This is indicated by the message that says "Planning a heist..." on top of the card. If a henchman can attack, you can do so by first clicking the henchman, then the target you'd like it to attack, either an opposing henchman or the opponent. If you'd like to change your mind, clicking one of your own henchman will back out, letting you play other cards or attack with other henchmen first.

Henchmen can't attack more than once each turn, this is indicated by the message that says "I'm done!" on top of the card.

Henchmen don't regain their health at the end of the turn, so if a henchman takes damage during combat with another henchman (or from some other effect), that damage will stay there, unless some other effect heals it.

Once you're done with your turn, you can click the button on the right of the game board, labeled "I'm finished!" This will make the turn pass to the opponent, after a short delay to allow you and your opponent to switch out, since both players currently play on the same computer.

#### Winning the Game ####

To win the game, you need to steal back all three "Prize Cards" that were removed from your deck at the start of the game. When you steal a prize card, it will go into your hand, allowing you to play it like any other card.

There are two ways to steal back your prize cards: 1) deal enough damage to your opponent or 2) draw enough cards. If you would draw a card from your deck when it has no cards left, you'll instead steal back one of you prize cards. The more common way of doing so is dealing enough damage to your opponent, each player has three life total thresholds: 5, 10, and 15. After dealing the damage required by your opponent's current threshold, you'll steal a prize card and their life total will be set to the next threshold (if the prize card wasn't your last).

There are three things to note about that threshold system. First, if you steal a prize card by drawing from an empty deck or from some effect other than passing a damage threshold, the next threshold will be added onto your opponent's current life total, so they don't lose life unfairly. Second, the thresholds are not health caps. For example, if a player gains 4 life while on the first threshold, their opponent will need to deal at least 9 total damage to pass that threshold. Third, excess damage from one threshold does not roll over to the next, so if a player takes 6 total damage on the 5 life threshold, they'll still get the full 10 life from the next threshold, meaning that you may have to do more than 30 total damage to win the game based only on damage.

#### Keyword Mechanics ####

Static Keywords:

1. **Eager** - Henchmen with this keyword don't need to "scheme," they can attack the turn they're played.
2. **Elusive** - Henchmen with this keyword can only be attacked by other henchmen with this keyword.
3. **Over-Achiever** - Henchmen with this keyword will deal any excess damage from defeating another henchman in combat to that henchman's controller.

Non-Static Keywords:

1. **Attention-Seeker** - Henchmen with this keyword have some special effect whenever _another_ henchman enters the battlefield
2. **Closing-Act** - Henchmen with this keyword have some special effect whenever they are defeated.
3. **Flashy** - Henchmen with this keyword have some special effect whenever _they_ enter the battlefield.
4. **Rush** - Henchmen with this keyword have some special effect whenever they deal _combat_ (directly or through Over-Achiever) damage to the opponent.
5. **Vengeance** - Henchmen with this keyword have some special effect whenever the opponent steals a prize card.
