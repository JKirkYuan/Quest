﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class ChivalrousDeedTest {

	[Test]
	public void ChivalrousDeedTestSimplePasses() {
		Assert.IsTrue (ChivalrousDeed.frequency == 1);
		ChivalrousDeed chivalrousDeed = new ChivalrousDeed ();
		Assert.AreEqual ("Chivalrous Deed", chivalrousDeed.getCardName ());

		chivalrousDeed.startBehaviour ();

		chivalrousDeed.runEvent ();

		//need to implement some sort of test case to test out processEvent function
		//Assert.IsTrue (false);
	}
}
