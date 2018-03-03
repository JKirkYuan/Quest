﻿public class TestOfTheQuestingBeast : Test {

	public static int frequency = 2;
	private int empoweredMinBidValue;

	public TestOfTheQuestingBeast() : base ("Test Of The Questing Beast", 3) {
		empoweredMinBidValue = 4;
	}

	public override int getMinBidValue() {
        if (BoardManagerMediator.getInstance().getCardInPlay().getCardName() == "Search for the Questing Beast") {
            return empoweredMinBidValue;
        }
		return minBidValue;
	}

}
