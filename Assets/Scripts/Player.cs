using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Player : PlayerHand
{
    public string Name = "Player ";

    private NetworkClient client;
        
    private class NetworkClient : NetworkBehaviour {

        [Command]
        public void CmdPlayCards() => Game.Instance().Server.RpcPlayCards(playerControllerId);         

        public override void OnStartLocalPlayer(){
            Debug.Log("Start Local Player");
            

        }

        public override void OnStartClient() {
            Debug.Log($"Player: {base.name} has joined the game!");
            
        }

        public override void OnNetworkDestroy() => Destroy(this.gameObject);
    }

    private void Awake() 
    {
        client = gameObject.AddComponent<NetworkClient>();
        if (client == null) Debug.LogError($"NetworkBehavour not found on {gameObject.name}");
    }

    public override void PlayCards() {
        
        if(client.isLocalPlayer)
            client.CmdPlayCards();
    }
}