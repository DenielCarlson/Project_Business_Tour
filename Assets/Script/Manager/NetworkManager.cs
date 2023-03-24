using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;
    public Text ServerInfo;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        NetworkManager.Instance = this;
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.LocalPlayer.NickName = "DenielTeste";
        ServerInfo.text += "\nConectando no Server...";

    }

    public override void OnConnectedToMaster()
    {
        ServerInfo.text += "\nConectado no Server";

        if (PhotonNetwork.InLobby == false)
        {
            ServerInfo.text += "\nConectado ao lobby...";
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        ServerInfo.text += "\nConectado no lobby";

        PhotonNetwork.JoinRoom("ServerTeste");
        ServerInfo.text += "\nEntrando na sala";
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
    
        ServerInfo.text += "\nErro ao entrar na sala, " + message;

        if (returnCode == ErrorCode.GameDoesNotExist)
        {
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = 4};
            PhotonNetwork.CreateRoom("ServerTeste", roomOptions);
            ServerInfo.text += "\nSala criada com sucesso";

        }
    }

    public override void OnJoinedRoom()
    {
        ServerInfo.text += "\nVocê entrou na sala";
        ServerInfo.text += "\nMeu id: " + PhotonNetwork.LocalPlayer;
        GameManager.Instance.photonView.RPC("CreatePlayer", RpcTarget.AllBuffered);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ServerInfo.text += "\nOutro jogador entou na sala, jogador nickname: " + newPlayer.NickName;
        /*if (PhotonNetwork.PlayerList.Length == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameManager.Instance.photonView.RPC("CreatePlayer", RpcTarget.AllBuffered);
            }
        }*/

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ServerInfo.text += "\nOutro Jogador deixou a sala, jogador nickname: " + otherPlayer.NickName;

    }



}
