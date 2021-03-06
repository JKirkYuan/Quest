﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public abstract class Player {

	protected string name;
	protected int numShields;
	protected Rank rank;
	protected List<Adventure> hand;
	protected PlayerPlayArea playArea;
    protected BoardManagerMediator board;
	public bool discarded;
    System.Random random;

	protected Player(string name) {
		this.name = name;
		rank = new Squire ();
		numShields = 0;
		hand = new List<Adventure> ();
		playArea = new PlayerPlayArea ();
        board = BoardManagerMediator.getInstance();
		discarded = false;
		random = new System.Random();
		if (BoardManagerMediator.getInstance ().IsOnlineGame ()) {
			random = new System.Random (Deck.seed);
		}
	}


    //--------------------------------------------//
    //------------ Abstract Functions ------------//
    //--------------------------------------------//


    public abstract void PromptDiscardCards(Action action);

    public abstract void DiscardCards(Action invalidAction, Action continueAction);

    public abstract void PromptSponsorQuest(Quest quest);

    public abstract void SponsorQuest(Quest quest, bool firstPrompt);

    public abstract void PromptAcceptQuest(Quest quest);

    public abstract void PromptFoe(Quest quest);

    public abstract void DisplayStageResults(Stage stage, bool playerEliminated);

    public abstract void PromptTest(Quest quest, int currentBid);

    public abstract void PromptDiscardTest(Quest quest, int currentBid);

    public abstract void PromptEnterTournament(Tournament tournament);

    public abstract void PromptTournament(Tournament tournament);

	public abstract void DisplayTournamentResults (Tournament tournament, bool playerEliminated);

	public abstract void PromptDiscardWeaponKingsCallToArms (KingsCallToArms card);

	public abstract void PromptDiscardFoesKingsCallToArms (KingsCallToArms card, int numFoeCards);


    //--------------------------------------------//
    //---------- Non-Abstract Functions ----------//
    //--------------------------------------------//


    public void DrawCards(int numCards, Action action) {
        List<Adventure> cards = new List<Adventure>();
        Logger.getInstance().info("Dealing cards to player: " + getName() + ", " + numCards);
        for (int i = 0; i < numCards; i++) {
            cards.Add(board.drawAdventureCard());
        }
        DealCards(cards, action);
    }

    public void DealCards(List<Adventure> cards, Action action)
    {
        foreach (Adventure card in cards)
        {
            card.SetOwner(this);
            hand.Add(card);
        }
        if (hand.Count > 12)
        {
            Debug.Log("More than 12 cards in " + name + "'s hand");
			toggleDiscarded (true);
            PromptDiscardCards(action);
        } else {
			toggleDiscarded (false);
            if (action != null) {
                action.Invoke();
            }
        }
    }


    //public void PromptNextPlayer() {
    //    if (func != null) {
    //        func();
    //        func = null;
    //    }
    //}


	private void checkForRankUp() {
		if (numShields == rank.getShieldsToProgress ()) {
			numShields = 0;
			upgradeRank ();
			//numShields = numShields - rank.getShieldsToProgress ();
			Debug.Log ("Rank is now: " + getRank ());
		} else if (numShields > rank.getShieldsToProgress ()) {
			numShields -= rank.getShieldsToProgress ();
			upgradeRank ();
			Debug.Log ("Rank is now: " + getRank ());
		}
	}

	public void RemoveRandomCards(int numCards) {
		if (numCards <= hand.Count) {
            if (numCards == 1) {
                board.AddToDiscardDeck(hand.ElementAt(hand.Count - 1));
                hand.RemoveAt(hand.Count - 1);
            } else {
                for (int i = 0; i < numCards; i++) {
					int index = random.Next(hand.Count - 1);
                    board.AddToDiscardDeck(hand.ElementAt(index));
                    hand.RemoveAt(index);
                }
            }
            Debug.Log("Removed " + numCards + " cards from " + name + "'s hand");
		}
	}


    public int getBattlePoints() {
        return rank.getBattlePoints() + playArea.GetBattlePoints();
    }

		
	public string getName() {
		return this.name;
	}


	public int getNumShields() {
		return this.numShields;
	}


	public List<Adventure> GetHand() {
		return this.hand;
	}


	public PlayerPlayArea getPlayArea () {
		return this.playArea;
	}


	public Rank getRank() {
		return this.rank;
	}


    public int getTotalAvailableBids() { //TODO: loop hand as well
        int availableBids = 0;
        availableBids += getHandBid();
        Debug.Log("Hand bid is: " + getHandBid());
        availableBids += getPlayAreaBid();
        Debug.Log("Player area bid: " + getPlayAreaBid());
        Debug.Log("Total available bids are: " + availableBids);
		return availableBids;
	}

    public int getHandBid() {
        int availableBids = 0;
        foreach (Adventure card in hand) {
            if (card.IsAlly())
            {
                if (card.getBidPoints() == 0) {
                    availableBids += 1;
                }
                else {
                    availableBids += card.getBidPoints();
                }
            }
            else
            {
                availableBids += 1;
            }
        }
        return availableBids;
    }

    public int getPlayAreaBid() {
        int availableBids = 0;
        foreach (Adventure card in playArea.getCards())
        {
            if (card.IsAlly()) {
                availableBids += card.getBidPoints();
            }
            else {
                availableBids += 1;
            }
        }
        return availableBids;
    }


    public void RemoveCard(Adventure card)
    {
        for (int i = 0; i < hand.Count(); i++)
        {
            if (card.GetCardName() == hand[i].GetCardName())
            {
                board.AddToDiscardDeck(hand.ElementAt(i));
                hand.RemoveAt(i);
                break;
            }
        }
    }

	public void RemoveCardsResponse(List<Adventure> chosenCards)
    {
		if (chosenCards.Count > 0) {
            foreach (Adventure card in chosenCards) {
                RemoveCard(card);
            }
            Debug.Log("Removed " + chosenCards.Count + " cards from " + name + "'s hand");
        }

        BoardManagerMediator.getInstance().DestroyDiscardArea();
    }

	public void GetAndRemoveCards () {
		List<Adventure> chosenCards = board.GetDiscardedCards (this);
		if (board.IsOnlineGame () && chosenCards.Count > 0) {
            board.getPhotonView ().RPC ("RemoveCardsResponse", PhotonTargets.Others, PunManager.Serialize(this), PunManager.Serialize(chosenCards));
		}
		RemoveCardsResponse(chosenCards);
	}

	public void incrementShields(int numShields) {
		this.numShields += numShields;
        Debug.Log("Awarded " + numShields + " shields to " + name);
        Logger.getInstance().info("Awarded " + numShields + " shields to " + name);
		checkForRankUp ();
	}


	public void decrementShields(int numShields) {
		if (this.numShields-numShields < 0) {
			this.numShields = 0;
		} else {
			this.numShields -= numShields;
		}
	}


	public Player upgradeRank() {
		rank = rank.upgrade ();
        Debug.Log("Upgraded " + name + "'s rank to " + rank.GetCardName());
		return this;
	}

    public string toString()
    {
        string output = name;
        if (GetType() == typeof(AIPlayer)) {
            if (((AIPlayer)this).GetStrategy().GetType() == typeof(Strategy1))
            {
                output += " (AI - Strategy 1): ";
            }
            else if (((AIPlayer)this).GetStrategy().GetType() == typeof(Strategy2))
            {
                output += " (AI - Strategy 2): ";
            }
        }
        else {
            output += " (Human): ";
        }
        foreach (Adventure card in hand)
        {
            output += card.ToString() + ", ";
        }
        output = output.Substring(0, output.Length - 2);
        return output;
    }

	public void toggleDiscarded(bool discarded) {
		Debug.Log("Toggling discarded to " + discarded + " for player " + name);
		this.discarded = discarded;
	}
}


