﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenIseult : Ally {

	private int empoweredBidPoints;

	public SirPercival() {
		bidPoints = 2;
		empoweredBidPoints = 4;
	}

	public new int getBidPoints() {
		/*
		 * if (condition) {
		 *     return empoweredBidPoints;
		 * }
		*/
		return bidPoints;
	}
}
