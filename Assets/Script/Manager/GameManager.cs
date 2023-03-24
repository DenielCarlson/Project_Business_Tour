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
        _currentPlayerTurnIndex = 0;
        _rollUI.SetActive(false);

    }

    private void Update()
    {
        _players.Sort((p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));
        TurnSystem();
    }

    [PunRPC]
    void NextTurn()
    {
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
            OnRolledDice();
            photonView.RPC("NextTurn", RpcTarget.All);
        }
        else
        {
            OnRolledDice();
            photonView.RPC("NextTurn", RpcTarget.All);
        }
    }

    void OnRolledDice()
    {
        GameObject currentPlayerTurn = null;

        foreach(var obj in _playerObjects)
        {
            if (_players[_currentPlayerTurnIndex] == obj.GetComponent<Movement>().PhotonPlayer)
            {
                currentPlayerTurn = obj;
            }
        }

        int dice = Random.Range(2, 13);
        currentPlayerTurn.GetComponent<Movement>().CityIndex(dice);
    }

    //Ativa ou desativa o Button "Roll" de acordo com o Turno do personagem
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

    //Método responsável por criar o player
    [PunRPC]
    public void CreatePlayer()
    {
        GameObject myPlayerObject = PhotonNetwork.Instantiate("Player", _spawn.position, Quaternion.identity);
        var myPlayerMovement = myPlayerObject.GetComponent<Movement>();
        myPlayerMovement.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }
}
