using UnityEngine;
using UnityEngine.Networking;

using System.Collections;
using System.Collections.Generic;
using System;

public class Game : NetworkManager {

    public Player LocalPlayer;

    public Dictionary<short, Player> Players = new Dictionary<short, Player>();

    public NetworkServer Server;
    private static Game _instance;

    public class NetworkServer : NetworkBehaviour {
        [ClientRpc]
        public void RpcPlayCards(short playerControllerId) {
            Debug.Log($"Playing cards for player {playerControllerId} on {this.playerControllerId}");
            Game.Instance().Players[playerControllerId].PlayCards();
        }
    }

    public static Game Instance()
    {
        if (_instance == null)
            _instance = FindObjectOfType<Game>();
        return _instance;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        GameObject playerObj;
        if (playerControllerId == 0)
            UnityEngine.Networking.NetworkServer.AddPlayerForConnection(conn, 
                playerObj=LocalPlayer.gameObject, playerControllerId);
        else
            UnityEngine.Networking.NetworkServer.AddPlayerForConnection(conn, playerObj = Instantiate(playerPrefab), playerControllerId);

        Players.Add(playerControllerId, playerObj.GetComponent<Player>());
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        Players.Remove(player.playerControllerId);

        UnityEngine.Networking.NetworkServer.DestroyPlayersForConnection(conn);
    }

    private void Awake() 
    {
        Server = gameObject.AddComponent<NetworkServer>();
        if (Server == null) Debug.LogError($"NetworkBehavour not found on {gameObject.name}");
    }
}