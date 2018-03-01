﻿using System.Collections.Generic;

public class Strategy1 : Strategy
{
    public Strategy1() : base (50, 20) {
        
    }

    public override void DiscardAfterWinningTest()
    {
        throw new System.NotImplementedException();
    }

    public override bool DoIParticipateInQuest()
    {
        throw new System.NotImplementedException();
    }

    public override void DoIParticipateInTournament()
    {
        throw new System.NotImplementedException();
    }

    public override bool DoISponsorAQuest()
    {
        if (SomeoneElseCanWinOrEvolveWithQuest())
        {
            return false;
        }
        else if (SufficientCardsToSponsorQuest())
        {
            return true;
        }
        return false;
    }

    public override void NextBid()
    {
        throw new System.NotImplementedException();
    }

    public override void PlayQuestStage(Stage stage)
    {
        throw new System.NotImplementedException();
    }

    public override void SponsorQuest()
    {
        throw new System.NotImplementedException();
    }

    protected override bool CanPlayCardForStage(Card card, List<Card> participationList)
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayFoeStage(Stage stage)
    {
        throw new System.NotImplementedException();
    }

    protected override void PlayTestStage(Stage stage)
    {
        throw new System.NotImplementedException();
    }
}
