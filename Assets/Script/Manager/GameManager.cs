using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject _rollUI;

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
        _rollUI.SetActive(false);

    }

    private void Update()
    {
        OrganizeListWithPhotonID(ref _playerObjects);
        _players.Sort((p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));
        ListValidation(ref _players);
        ListValidation(ref _playerObjects);

        TurnSystem();
    }

    private void OrganizeListWithPhotonID(ref List<GameObject> list)
    {
        List<PhotonView> toOrganize = new List<PhotonView>();

        for (int i = 0; i < list.Count; i++)
        {
            toOrganize.Add(list[i].GetComponent<PhotonView>());
        }

        toOrganize.Sort((x1, x2) => x1.ViewID.CompareTo(x2.ViewID));

        for (int i = 0; i < list.Count; i++)
        {
            list[i] = toOrganize[i].gameObject;
        }

    }

    private void ListValidation<T>(ref List<T> list)
    {
        List<T> newList = list.Distinct().ToList();

        list = newList;
    }

    [PunRPC]
    void NextTurn()
    {
        _serverInfo.text += "\nId" + _players[_currentPlayerTurnIndex].ActorNumber + ", CurrentPlayerNumber: " + _currentPlayerTurnIndex + ", Players: " + _players.Count + ", PlayersInGame: " + _playersInGame;
        _turnCount++;
        _currentPlayerTurnIndex = _turnCount % _players.Count;
    }

    private bool IsCurrentPlayerTurn()
    {
        return _players[_currentPlayerTurnIndex] == PhotonNetwork.LocalPlayer;
    }


    public void OnRollDiceClick()
    {
        if (_currentPlayerTurnIndex == 0)
        {
            photonView.RPC("OnRolledDice", RpcTarget.All);
            photonView.RPC("NextTurn", RpcTarget.All);
        }
        else
        {
            photonView.RPC("NextTurn", RpcTarget.All);
        }
    }

    private void TurnSystem()
    {

        if (IsCurrentPlayerTurn())
        {
            _rollUI.SetActive(true);
        }
        else 
        {
           _rollUI.SetActive(false);
        }

    }
    [PunRPC]
    void OnRolledDice()
    {
        GameObject player = _playerObjects[_currentPlayerTurnIndex];
        int dice = 0;

        if (_players[_currentPlayerTurnIndex].ActorNumber == player.GetComponent<Movement>().ID)
        {
            Debug.Log("encontrou meu player");
            Debug.Log(_players[_currentPlayerTurnIndex] + ", " + player.GetComponent<Movement>().ID);
            Random.Range(2, 13);
            player.GetComponent<Movement>().CityIndex(dice);
        }
        else
        {
            Debug.Log("não encontrou meu player");
            Debug.Log(_players[_currentPlayerTurnIndex] + ", " + player.GetComponent<Movement>().ID);
        }
    }

    //Método responsável por criar o player
    [PunRPC]
    public void CreatePlayer()
    {
        GameObject myPlayerObject = PhotonNetwork.Instantiate("Player", _spawn.position, Quaternion.identity);
        myPlayerObject.name = "Player " + _playersInGame;
        var myPlayerMovement = myPlayerObject.GetComponent<Movement>();
        myPlayerMovement.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }
}
