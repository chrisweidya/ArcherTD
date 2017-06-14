using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Prototype.NetworkLobby {
    public class PlayerHook : LobbyHook {

        public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
            LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
            PlayerProperties playerProperties = gamePlayer.GetComponent<PlayerProperties>();

            playerProperties.SetTeam(lobby.playerFaction);
        }
    }
}