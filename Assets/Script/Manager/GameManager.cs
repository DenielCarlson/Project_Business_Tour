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

    //Lista pública com todos os jogadores na sala
    public List<Player> Players { get => _players; private set => _players = value; }
    //Lista pública com todos os objetos de jogares no tabuleiro
    public List<GameObject> PlayerObjects { get => _playerObjects; private set => _playerObjects = value; }

    private List<Player> _players;
    private List<GameObject> _playerObjects;

    //Index que mudará a cada turno pecorrendo a lista de jogadores e decidindo de quem será o turno atual
    private int _currentPlayerTurnIndex;
    //Número que é aumentado a cada turno, necessário para decidir o jogador seguinte
    private int _turnCount;

    //spawn dos objetos de jogadores
    [SerializeField] private Transform _spawn;
    //informações na tela - isso será temporário
    [SerializeField] private Text _serverInfo;
    //canvas que é ativado quando o turno atual é do player local e desativado quando não é
    [SerializeField] private GameObject _rollUI;


    //Var responsável pelas cidades
    private GameObject currentPlayer;
    private GameObject currentCity;
    [SerializeField] private GameObject _uiBuyCity;
    [SerializeField] private GameObject _uiBuildHouse;

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

        //-----------------------------------
        currentCity = null;
        currentPlayer = null;

    }

    private void Update()
    {
        //Organiza a lista em ordem de entrada para todos os jogadores
        _players.Sort((p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));
        TurnSystem();
    }


    //Soma +1 na variavel _turnCount e passa para o próximo turno
    [PunRPC]
    void NextTurn()
    {
        _turnCount++;
        _currentPlayerTurnIndex = _turnCount % _players.Count;
    }

    //Retorna se é a vez do seu jogador ou não
    private bool IsCurrentPlayerTurn()
    {
        return _players[_currentPlayerTurnIndex] == PhotonNetwork.LocalPlayer;
    }


    //Método que é atrelado ao btnRoll, que faz o player jogar os dados e passar o turno
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

    //Lógica dos dados, onde o player atual jogar os dados e faz o playerObj se movimentar no tabuleiro
    void OnRolledDice()
    {
        GameObject currentPlayerTurn = null;

        foreach(var obj in _playerObjects)
        {
            if (_players[_currentPlayerTurnIndex] == obj.GetComponent<PlayerScript>().PhotonPlayer)
            {
                currentPlayerTurn = obj;
            }
        }

        int dice = Random.Range(2, 13);
        currentPlayerTurn.GetComponent<PlayerScript>().CityIndex(dice);
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
        var myPlayerMovement = myPlayerObject.GetComponent<PlayerScript>();
        myPlayerMovement.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }


    //Manager das cidade

    public void OnBuyCityClick()
    {
        if (IsCurrentPlayerTurn())
        {
            foreach (var obj in _playerObjects) 
            {
                if (obj.GetComponent<PlayerScript>().ID == _players[_currentPlayerTurnIndex].ActorNumber)
                {
                    currentPlayer = obj;
                    currentCity = obj.GetComponent<PlayerScript>().Cities[obj.GetComponent<PlayerScript>().CityIndexVar];
                }
            }

            if (currentCity.GetComponent<City>().HasPlayer && currentCity.GetComponent<City>().WasBought == false)
            {
                currentCity.GetComponent<City>().Player = currentPlayer;

                if (currentPlayer.GetComponent<PlayerWallet>().Money > currentCity.GetComponent<City>().InitialPrice)
                {
                    currentPlayer.GetComponent<PlayerWallet>().Withdraw(currentCity.GetComponent<City>().InitialPrice);
                    currentCity.GetComponent<City>().IdOwner = currentPlayer.GetComponent<PlayerScript>().ID;
                    currentPlayer.GetComponent<PlayerScript>().MyCities.Add(currentCity);
                    currentCity.GetComponent<City>().BuildFlag();

                    _uiBuyCity.SetActive(false);
                    _uiBuildHouse.SetActive(true);
                }

            }
            else if(currentCity.GetComponent<City>().HasPlayer == false && currentCity.GetComponent<City>().WasBought == false)
            {
                if (currentPlayer.GetComponent<PlayerWallet>().Money > currentCity.GetComponent<City>().InitialPrice)
                {
                    currentPlayer.GetComponent<PlayerWallet>().Withdraw(currentCity.GetComponent<City>().InitialPrice);
                    currentCity.GetComponent<City>().IdOwner = currentPlayer.GetComponent<PlayerScript>().ID;
                    currentPlayer.GetComponent<PlayerScript>().MyCities.Add(currentCity);
                    currentCity.GetComponent<City>().BuildFlag();

                    _uiBuyCity.SetActive(false);
                    _uiBuildHouse.SetActive(true);
                }
            }
        }
    }


    public void OnBuildHouseLvlOneClick()
    {
        if (currentPlayer.GetComponent<PlayerScript>().ID == currentCity.GetComponent<City>().IdOwner)
        {
            if (currentCity.GetComponent<City>().LevelCity == 0)
            {
                currentPlayer.GetComponent<PlayerWallet>().Withdraw(200);
                currentCity.GetComponent<City>().BuildHouse();
            }
        
        }
        _uiBuildHouse.SetActive(false);
    }
}
