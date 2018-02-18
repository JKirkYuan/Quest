﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPlayArea {

	List<Card> cards;

	public PlayerPlayArea() {
        cards = new List<Card>();
	}

	public List<Card> getCards() {
		return cards;
	}

	public void addCard(Card card) {
		cards.Add (card);
	}

	public void discardWeapons() {
        List<Card> tempCards = new List<Card>();
		foreach (Card card in cards) {
            if (!card.GetType().IsSubclassOf(typeof(Weapon))) {
                tempCards.Add(card);
            }
		}
        cards = tempCards;
	}

	public void discardAmours() {
        List<Card> tempCards = new List<Card>();
        foreach (Card card in cards)
        {
            if (!card.GetType().Equals(typeof(Amour)))
            {
                tempCards.Add(card);
            }
        }
        cards = tempCards;
	}

	public void discardAllies() {
        List<Card> tempCards = new List<Card>();
        foreach (Card card in cards)
        {
            if (!card.GetType().IsSubclassOf(typeof(Ally)))
            {
                tempCards.Add(card);
            }
        }
        cards = tempCards;
	}

	public void discardAlly(Type type) {
		//TODO: maybe this can be implemented by specifically referencing the card to be discarded?
		foreach (Card card in cards) {
			if (card.GetType () == type) {
				cards.Remove (card);
				break;
			}
		}
	}
}