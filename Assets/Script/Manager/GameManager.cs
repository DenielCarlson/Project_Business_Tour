using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    public List<Player> Players { get => _players; private set => _players = value; }
    public List<GameObject> PlayerObjects { get => _playerObjects; private set => _playerObjects = value; }

    private List<Player> _players;
    private List<GameObject> _playerObjects;

    private int _playersInGame;
    private int _currentPlayerTurnIndex;
    private int _turnCount;

    [SerializeField] private Transform _spawn;
    [SerializeField] private Text _serverInfo;
    [SerializeField] private Button btnRoll;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }

        Instance = this;
        _players = new List<Player>();
        _playerObjects = new List<GameObject>();
        _playersInGame = 0;
        _currentPlayerTurnIndex = 0;

    }

    private void Update()
    {

        //photonView.RPC("InteractableBtnRoll", RpcTarget.All);

        _players.Sort((p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));

        photonView.RPC("ListValidation", RpcTarget.All, _players);
    }

    [PunRPC]
    private void ListValidation<T>(List<T> list)
    {
        if (list.Count > _playersInGame)
        {
            List<T> newListPlayer = list.Distinct().ToList();
            list = newListPlayer;
        }
    }

    [PunRPC]
    void NextTurn()
    {
        _serverInfo.text += "\nId" + _players[_currentPlayerTurnIndex].ActorNumber + ",  TurnCount: " + _turnCount + ", Players: " + _players.Count + ", CurrentPlayerNumber: " + _currentPlayerTurnIndex;

        int idPlayer = _playerObjects[_currentPlayerTurnIndex].GetComponent<Movement>().ID;
        _serverInfo.text += "\n" + idPlayer;
       _turnCount++;
        _currentPlayerTurnIndex = _turnCount % _players.Count;
    }


    public void OnRollDiceClick()
    {
        if (_currentPlayerTurnIndex == 0)
        {
            photonView.RPC("NextTurn", RpcTarget.All);
        }
        else
        {
            photonView.RPC("NextTurn", RpcTarget.All);
        }
    }

    /*[PunRPC]
     private void InteractableBtnRoll()
     {
         if (_players[_currentPlayerTurnIndex] == PhotonNetwork.LocalPlayer)
         {
             btnRoll.interactable = true;
         }
         else
         {
             btnRoll.interactable = false;
         }
     }*/

    [PunRPC]
    public void StartGame()
    {
        _playersInGame++;
    }

    [PunRPC]
    public void CreatePlayer()
    {
        GameObject myPlayerObject = PhotonNetwork.Instantiate("Player", _spawn.position, Quaternion.identity);
        var myPlayerMovement = myPlayerObject.GetComponent<Movement>();
        myPlayerMovement.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }
}
