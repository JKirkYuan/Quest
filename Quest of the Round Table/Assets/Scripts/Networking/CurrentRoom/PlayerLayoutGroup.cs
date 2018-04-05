﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLayoutGroup : MonoBehaviour {

	[SerializeField]
    public GameObject _playerListingPrefab;
    public GameObject PlayerListingPrefab {
        get { return _playerListingPrefab; }
    }

    private List<PlayerListing> _playerListings = new List<PlayerListing>();
    private List<PlayerListing> PlayerListings {
        get { return _playerListings; }
    }

    private void OnJoinedRoom() {
        GameObject lobby = GameObject.Find("Canvas/Lobby");
        lobby.SetActive(false);
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++){
            Debug.Log("Joined room " + photonPlayers[i].NickName);
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    public void SetupAI1()
    {
        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        Text[] texts = playerListingObj.transform.GetComponentsInChildren<Text>();
        texts[0].text = "AI_One" + UnityEngine.Random.Range(1, 50);
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();

        PlayerListings.Add(playerListing);
    }

    private void OnPhotonPlayerConnected(PhotonPlayer photonPlayer){
        PlayerJoinedRoom(photonPlayer);
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer photonPlayer)
    {
        PlayerLeftRoom(photonPlayer);
    }

    private void PlayerJoinedRoom(PhotonPlayer photonPlayer) {
        if (photonPlayer == null){
            Debug.Log("Null, returning");
            return;   
        }
        PlayerLeftRoom(photonPlayer);
        GameObject playerListingObj = Instantiate(PlayerListingPrefab);
        playerListingObj.name = photonPlayer.NickName;
        playerListingObj.transform.SetParent(transform, false);

        PlayerListing playerListing = playerListingObj.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);

        PlayerListings.Add(playerListing);

		if (photonPlayer.IsMasterClient) {
			PhotonView view = PhotonView.Get (GameObject.Find ("DDOL/PunManager"));
			System.Random rand = new System.Random();
			string seedString = "";
			for (int i = 0; i < 5; i++) {
				seedString += rand.Next(0, 10);
			}
			view.RPC ("UpdateSeed", PhotonTargets.All, Int32.Parse(seedString));
		}
    }

    private void PlayerLeftRoom(PhotonPlayer photonPlayer){
        int index = PlayerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);
        if (index != -1)
        {
            Destroy(PlayerListings[index].gameObject);
            PlayerListings.RemoveAt(index);
        }
    }


    public void PunSwitchScene(string sceneName) {
        Debug.Log("Trying to switch scene");
        PhotonView view = PhotonView.Get(GameObject.Find("DDOL/PunManager"));
        view.RPC("SwitchScene", PhotonTargets.All, sceneName);
    }

    public static void SwitchScene(string SceneName) {
        List<Player> playerList = new List<Player>();
        foreach (var player in PhotonNetwork.playerList)
        {
            print("Instantiating player: " + player.NickName);
            Player tempPlayer = new HumanPlayer(player.NickName);
            print("Created player: " + tempPlayer.getName());
            playerList.Add(tempPlayer);
            print("Finished adding");
        }
        print("Exited.");
        ButtonManager.playerList = playerList;
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        print("Finished Loading scene");
    }
}
