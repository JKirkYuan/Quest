﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class AdventureDeckTest {

	[Test]
	public void testInitAdventureDeck() {
		Deck deck = new AdventureDeck ();
		Assert.IsTrue (deck.getCards ().Count == 67);
	}
}
