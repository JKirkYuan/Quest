using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyThroughTheEnchantedForest : Quest {

	public static int frequency = 1;

	public JourneyThroughTheEnchantedForest() : base ("Journey through the Enchanted Forest", 3) {
		dominantFoes = new List<Type> ();
		dominantFoes.Add(Type.GetType("EvilKnight", true));
	}

}
