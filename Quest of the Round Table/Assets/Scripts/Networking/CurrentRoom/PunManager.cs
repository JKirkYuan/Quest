﻿using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PunManager : Photon.MonoBehaviour {

    BoardManagerMediator board;

    [PunRPC]
    public void SwitchScene(string seed, string sceneName)
    {
        PlayerLayoutGroup.SwitchScene(seed, sceneName);
    }

    [PunRPC]
    public void PromptSponsorQuestResponse(bool sponsorAccepted) {
        GetBoard();
        ((Quest)board.getCardInPlay()).PromptSponsorQuestResponse(sponsorAccepted);
    }

    [PunRPC]
    public void SponsorQuestComplete(byte[] stagesBytes) {
        GetBoard();
        List<Stage> stages = (List<Stage>)Deserialize(stagesBytes);
        ((Quest)board.getCardInPlay()).SponsorQuestComplete(stages);
    }

    [PunRPC]
    public void IncrementSponsor() {
        GetBoard();
        ((Quest)board.getCardInPlay()).IncrementSponsor();
    }

    [PunRPC]
    public void PromptAcceptQuestResponse() {
        GetBoard();
        ((Quest)board.getCardInPlay()).IncrementSponsor();
    }

    System.Object Deserialize(byte[] arrBytes)
    {
        using (var memStream = new MemoryStream())
        {
            var binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var obj = binForm.Deserialize(memStream);
            return obj;
        }
    }

    [PunRPC]
    public void CardsSelectionResponse(Tournament tournament, Player player)
    {
        BoardManager.DrawCover(player);
        tournament.CardsSelectionResponse();
    }

    void GetBoard(){
        board = BoardManagerMediator.getInstance();
    }

}

//public void PromptEnterTournament(Tournament tournament, Player player, bool entered)
//{
//    BoardManager.DrawCover(player);
//    tournament.PromptEnterTournamentResponse(entered);
//}