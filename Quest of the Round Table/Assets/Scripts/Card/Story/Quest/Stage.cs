using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stage {
	BoardManagerMediator board;

	public int stageNum, currentBid;
    bool isInProgress;
	Adventure stageCard;
	List<Adventure> weapons;
    List<Player> playersToRemove;
    Action action;
	Quest quest;
    Player playerToPrompt, originalPlayer, highestBiddingPlayer;

	public Stage(Adventure stageCard, List<Adventure> weapons, int stageNum) {
		Logger.getInstance ().info ("Starting the Stage class");
		board = BoardManagerMediator.getInstance ();

        isInProgress = false;
		this.stageCard = stageCard;
		this.weapons = weapons;
        this.stageNum = stageNum;
	}

	public int getTotalBattlePoints() {
		int battlePoints = 0;
		if (stageCard.IsFoe()) {
			battlePoints += stageCard.getBattlePoints ();
            if (weapons != null) {
                foreach (Weapon weapon in weapons) {
                    battlePoints += weapon.getBattlePoints();
                }
            }
		}
		Logger.getInstance ().trace ("The total battle points are " + battlePoints);
		return battlePoints;
	}

    public List<Adventure> getCards() {
        List<Adventure> cards = new List<Adventure>();
        cards.Add(stageCard);
        if (weapons != null) {
            foreach (Weapon weapon in weapons) {
                cards.Add(weapon);
            }
        }
        return cards;
    }

	public int getTotalCards() {
        int totalCards = 1;
        if (weapons != null) {
            totalCards += weapons.Count;
        }
		Logger.getInstance ().trace ("The total cards are " + totalCards);
        return totalCards;
	}

    public void SetParentQuest(Quest quest) {
        this.quest = quest;
    }

	public void Prepare() {
		board = BoardManagerMediator.getInstance();
		Logger.getInstance ().debug ("Prepare function has started");

        isInProgress = true;
		playersToRemove = new List<Player> ();

		if (stageCard.IsFoe()) {
			Logger.getInstance ().trace ("Stage card is subclass type of foe");
			Debug.Log ("Is foe, going to player");
            Debug.Log("quest player after sponsor is: " + board.getNextPlayer(quest.getSponsor()).getName());
            playerToPrompt = board.getNextPlayer(quest.getSponsor());
            Debug.Log("Player to prompt is: " + playerToPrompt.getName());
			Logger.getInstance().info("playerToPrompt is: " + playerToPrompt.getName());
            //TODO: this is probably causing an infinite loop
            Debug.Log("After moving to next player");
            Logger.getInstance().info("Checking amount of players: " + quest.getPlayers().Count);
            while (!quest.getPlayers().Contains(playerToPrompt)) {
                playerToPrompt = board.getNextPlayer(playerToPrompt);
            }
            originalPlayer = playerToPrompt;
            playerToPrompt.PromptFoe(quest);
		} else {
			Logger.getInstance ().trace ("Stage card is NOT subclass type of foe");
			currentBid = ((Test)stageCard).getMinBidValue() - 1;
			Debug.Log ("Current bid is: " + currentBid);
            playerToPrompt = board.getNextPlayer(quest.getSponsor());
            //TODO: this is probably causing an infinite loop
            while (!quest.getPlayers().Contains(playerToPrompt))
            {
                playerToPrompt = board.getNextPlayer(playerToPrompt);
            }
            PromptTest ();
		}
	}


    public void PromptFoeResponse(bool dropOut) {
        if (!dropOut) {
            playerToPrompt = quest.getNextPlayer(playerToPrompt);
            ContinueQuest();
        }
        else {
            Debug.Log("Dropped out");
			playersToRemove.Add (playerToPrompt);
            playerToPrompt = quest.getNextPlayer(playerToPrompt);
            Debug.Log("Next player: " + playerToPrompt.getName());
            ContinueQuest();
        }
	}


    public void ContinueQuest(){
		if (quest.getPlayers().Count - playersToRemove.Count < 1)
        {
            Logger.getInstance().info("Moving to next stage");
            Debug.Log("No quest participants left");
			foreach (Player player in playersToRemove) {
				Debug.Log ("Removing player: " + player.getName ());
				quest.removeParticipatingPlayer (player);
			}
            quest.PlayStage();
        }
        else{
			if (playerToPrompt.getName() != originalPlayer.getName()) {
                playerToPrompt.PromptFoe(quest);
            }
            else {
                Debug.Log("All players have been prompted");
				foreach (Player player in playersToRemove) {
					Debug.Log ("Removing player: " + player.getName ());
					quest.removeParticipatingPlayer (player);
				}
                PlayFoe();
            }
        }

    }

	void PromptTest() {
		Logger.getInstance ().debug ("prompting Test...");
        Debug.Log("Prompting test");
        Debug.Log("The player to prompt is: " + playerToPrompt.getName());
        if (currentBid > (((Test)quest.getCurrentStage().getStageCard()).getMinBidValue() - 1) && quest.getPlayers().Count == 1)
        {
            playerToPrompt.PromptDiscardTest(quest, currentBid);
        }
        else
        {
            playerToPrompt.PromptTest(quest, currentBid);
        }
	}

	bool isValidBidder() {
		return (playerToPrompt.getTotalAvailableBids () > currentBid);
	}

	void incrementBidder() {
		playerToPrompt = quest.getNextPlayer (playerToPrompt);
        PromptTest ();
	}

	public void PromptTestResponse(bool dropOut, int interactionBid) {
		Logger.getInstance ().debug ("Prompting Test Response...");
        Debug.Log("Prompting test response");

        if(!dropOut) {
            currentBid = interactionBid;
            highestBiddingPlayer = playerToPrompt;
            Debug.Log("Continue test");
            Debug.Log("Current bid before incrementing is: " + currentBid);
            playerToPrompt = board.getNextPlayer(playerToPrompt);
            while (!quest.getPlayers().Contains(playerToPrompt)) {
                playerToPrompt = board.getNextPlayer(playerToPrompt);
            }
            Debug.Log("Current bid after incrementing is: " + currentBid);
            PromptTest();
        }
        else {
            Debug.Log("Dropped out of Test");
            Player temp = playerToPrompt;
            playerToPrompt = quest.getNextPlayer(playerToPrompt);
            if (originalPlayer == temp)
            {
                originalPlayer = quest.getNextPlayer(originalPlayer);
            }
            Debug.Log("Removing player: " + temp.getName());
            quest.removeParticipatingPlayer(temp);
            Debug.Log("New total participant: " + quest.getPlayers().Count);
            if(quest.getPlayers().Count == 0){
                ContinueQuest();
            }
            else {
                PromptTest();
            }
        }
	}

    public void removeBidsFromHand() {
        Debug.Log("Removing number of bids " + currentBid + " from " + playerToPrompt.getName());
        board.PromptCardRemoveSelection(playerToPrompt, action);
    }

    void PlayFoe() {
        playersToRemove = new List<Player>();
		while (!quest.getPlayers ().Contains (playerToPrompt)) {
			playerToPrompt = board.getNextPlayer (playerToPrompt);
		}
		originalPlayer = playerToPrompt;
        EvaluatePlayerForFoe();
    }

	void EvaluatePlayerForFoe() {
        int playerBattlePoints = playerToPrompt.getRank().getBattlePoints();
        bool playerEliminated = false;
        List<Adventure> stageCards = playerToPrompt.getPlayArea ().getCards ();
        foreach (Adventure card in stageCards) {
            playerBattlePoints += card.getBattlePoints ();
        }
        if (playerBattlePoints >= getTotalBattlePoints ()) {
            Logger.getInstance ().trace ("playerBattlePoints >= getTotalbattlePoints");
            Debug.Log("Player " + playerToPrompt.getName() + " passed the stage.");
        } else {
            Logger.getInstance ().trace ("Did not pass. Player will be removed: " + playerToPrompt.getName());
            Debug.Log("Player " + playerToPrompt.getName() + " did not pass the stage.");
            playerEliminated = true;
        }
        playerToPrompt.DisplayStageResults(this, playerEliminated);
	}

    public void EvaluateNextPlayerForFoe(bool previousPlayerEliminated)
    {
        Player previousPlayer = playerToPrompt;
        playerToPrompt = quest.getNextPlayer(playerToPrompt);
        if (previousPlayerEliminated)
        {
            playersToRemove.Add(previousPlayer);
        }
        if (playerToPrompt != originalPlayer)
        {
            EvaluatePlayerForFoe();
        }
        else
        {
            foreach (Player player in playersToRemove)
            {
                quest.removeParticipatingPlayer(player);
            }
            if (quest.getPlayers().Count > 0) {
                while (!quest.getPlayers().Contains(playerToPrompt)) {
                    playerToPrompt = board.getNextPlayer(playerToPrompt);
                }
                originalPlayer = playerToPrompt;
                DealCards();
            } else {
                quest.PlayStage();
            }
        }
    }

    public void DealCards() {
        action = () => {
			Action completeAction = () => {
				Debug.Log("Player discarded: " + playerToPrompt.discarded);
				if (board.IsOnlineGame() && playerToPrompt.discarded) {
					playerToPrompt.toggleDiscarded(false);
					Logger.getInstance ().debug ("In Stage DealCards(), about to RPC DealCardsNextPlayer");
					Debug.Log("In Stage DealCards(), about to RPC DealCardsNextPlayer");
					board.getPhotonView().RPC("DealCardsNextPlayer", PhotonTargets.Others);
				}				
				DealCardsNextPlayer();
			};
            if (playerToPrompt.GetHand().Count > 12) {
                playerToPrompt.DiscardCards(action, completeAction);
            }
            else {
                DealCardsNextPlayer();
            }
        };
        playerToPrompt.DrawCards(1, action);
    }


    public void DealCardsNextPlayer() {
        playerToPrompt = quest.getNextPlayer(playerToPrompt);
        if (playerToPrompt != originalPlayer) {
            DealCards();
        } else {
            quest.PlayStage();
        }
    }

    public int getStageNum() {
        return stageNum;
    }

    public Card getStageCard() {
        return stageCard;
    }

    public bool IsInProgress() {
        return isInProgress;
    }
}
