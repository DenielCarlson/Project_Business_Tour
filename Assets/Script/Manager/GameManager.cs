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
    private byte _count;

    private bool _isButtonRollPressed;

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
    [SerializeField] private Button _houseLevel1;
    [SerializeField] private GameObject _uiRebuy;
    [SerializeField] private GameObject _uiStart;
    [SerializeField] private GameObject _uiLostIsland;
    [SerializeField] private GameObject _uiWorldChampionship;
    [SerializeField] private GameObject _uiWorldTour;

    [SerializeField] private Material[] _materials;


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
        _isButtonRollPressed = false;
        _count = 0;

        //-----------------------------------
        currentCity = null;
        currentPlayer = null;

    }

    private void Update()
    {
        SetColorPlayer();

        //Organiza a lista em ordem de entrada para todos os jogadores
        _players.Sort((p1, p2) => p1.ActorNumber.CompareTo(p2.ActorNumber));
        TurnSystem();
        CurrentPlayerAndCity();
        ButtonsHouseValidation();

    }


    //Soma +1 na variavel _turnCount e passa para o próximo turno
    [PunRPC]
    void NextTurn()
    {
        _isButtonRollPressed = false;
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

        OnRolledDice();
        _isButtonRollPressed = true;
    }

    //Lógica dos dados, onde o player atual jogar os dados e faz o playerObj se movimentar no tabuleiro
    void OnRolledDice()
    {
        GameObject currentPlayerTurn = null;

        foreach (var obj in _playerObjects)
        {
            if (_players[_currentPlayerTurnIndex] == obj.GetComponent<PlayerScript>().PhotonPlayer)
            {
                currentPlayerTurn = obj;
            }
        }

        int dice = Random.Range(2, 13);
        _serverInfo.text = dice.ToString();
        currentPlayerTurn.GetComponent<PlayerScript>().CityIndex(dice);
    }

    //Ativa ou desativa o Button "Roll" de acordo com o Turno do personagem
    private void TurnSystem()
    {

        if (IsCurrentPlayerTurn())
        {
            if (_isButtonRollPressed == false)
            {
                _rollUI.SetActive(true);
            }
            else
            {
                _rollUI?.SetActive(false);
            }

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

    void SetColorPlayer()
    {
        foreach (var player in PlayerObjects)
        {

            if (player.GetComponent<PlayerScript>().ID == 1)
            {
                _materials[0].color = Color.blue;
                player.GetComponent<Renderer>().material = _materials[0];
            }else if (player.GetComponent<PlayerScript>().ID == 2)
            {
                _materials[1].color = Color.yellow;
                player.GetComponent<Renderer>().material = _materials[1];
            }
            else if (player.GetComponent<PlayerScript>().ID == 3)
            {
                _materials[2].color = Color.green;
                player.GetComponent<Renderer>().material = _materials[2];
            }
            else if (player.GetComponent<PlayerScript>().ID == 4)
            {
                _materials[3].color = Color.red;
                player.GetComponent<Renderer>().material = _materials[3];
            }

        }
    }





















    //Esse método  ele pega o objeto do jogador atual e apartir disso localiza a cidade em que o jogador atual está e a guarda em uma variável

    private void CurrentPlayerAndCity()
    {
        foreach (var obj in _playerObjects)
        {
            if (obj.GetComponent<PlayerScript>().ID == _players[_currentPlayerTurnIndex].ActorNumber)
            {
                currentPlayer = obj;
                currentCity = obj.GetComponent<PlayerScript>().Cities[obj.GetComponent<PlayerScript>().CityIndexVar];
            }
        }

    }


    //Método que será atribuído ao button Buy
    public void OnBuyCityClick()
    {
        //se for a vez do jogador atual, ele ativa essa condição
        if (IsCurrentPlayerTurn())
        {

            //se a casa na qual o jogador chegar tiver outro player e essa casa ainda não tiver sido comprada, o jogador atual da cidade passa a ser o jogador do turno atual 
            if (currentCity.GetComponent<City>().HasPlayer && currentCity.GetComponent<City>().WasBought == false)
            {
                currentCity.GetComponent<City>().Player = currentPlayer;

                //Se o dinheiro do jogador atual for maior que o valor da cidade, ele poderá comprar a cidade
                if (currentPlayer.GetComponent<PlayerWallet>().Money > currentCity.GetComponent<City>().InitialPrice)
                {
                    // o valor da cidade é debitado da conta do jogador
                    currentPlayer.GetComponent<PlayerWallet>().Withdraw(currentCity.GetComponent<City>().InitialPrice);
                    // e o jogador vira dono do turno atual
                    currentCity.GetComponent<City>().IdOwner = currentPlayer.GetComponent<PlayerScript>().ID;
                    currentCity.GetComponent<City>().photonView.RPC("CityWasBought", RpcTarget.All);
                    // e essa cidade é adicionada na lista de cidades pertencentes ao jogador atual
                    currentPlayer.GetComponent<PlayerScript>().MyCities.Add(currentCity);

                    //a tela para comprar desaparece e aparece a tela para construir casas
                    _uiBuyCity.SetActive(false);
                    _uiBuildHouse.SetActive(true);
                }

            }

            //Caso não tenha um jogador na cidade e ela n foi comprada, o jogador do turno atual pode compra-lá
            else if (currentCity.GetComponent<City>().HasPlayer == false && currentCity.GetComponent<City>().WasBought == false)
            {
                //essa condição é a mesma do if acima
                if (currentPlayer.GetComponent<PlayerWallet>().Money > currentCity.GetComponent<City>().InitialPrice)
                {
                    currentPlayer.GetComponent<PlayerWallet>().Withdraw(currentCity.GetComponent<City>().InitialPrice);
                    currentCity.GetComponent<City>().IdOwner = currentPlayer.GetComponent<PlayerScript>().ID;
                    currentCity.GetComponent<City>().photonView.RPC("CityWasBought", RpcTarget.All);
                    currentPlayer.GetComponent<PlayerScript>().MyCities.Add(currentCity);

                    _uiBuyCity.SetActive(false);
                    _uiBuildHouse.SetActive(true);
                }
            }

            //Como essa função só aparece quando o jogador clicka no button de comprar, então toda vez que ele comprar, irá spawnar uma bandeira na cidade que ele comprou
            //Porém essa função ainda está incompleta, pois se o jogador não tiver dinheiro, a bandeira não pode aparecer e portando no futuro essa função será melhorada ou substituída 
            currentCity.GetComponent<City>().BuildFlag(currentPlayer.GetComponent<PlayerScript>().ID);
        }
    }


    //Método chamando quando o jogador clickar no button de contruir casas do lvl 1
    public void OnBuildHouseLvlOneClick()
    {

        //Caso a cidade onde o jogador atual está seja dele, e ela esteja no level 1, essa condição se ativará
        if (currentCity.GetComponent<City>().LevelCity == 1 && currentPlayer.GetComponent<PlayerScript>().ID == currentCity.GetComponent<City>().IdOwner)
        {
            //é debitado o valor para contruir casas lvl 1 da conta do jogador atual
            currentPlayer.GetComponent<PlayerWallet>().Withdraw(200);
            //Uma casa lvl 1 é spawnada na cidade do jogador atual
            currentCity.GetComponent<City>().BuildHouse();
        }

        //Jogador atual passa a vez e a tela de contruir casas desaparece
        photonView.RPC("NextTurn", RpcTarget.All);
        _uiBuildHouse.SetActive(false);
    }

    //esse método será chamado qndo o jogador atual clickar no button close que fica na tela para comprar a cidade
    //ele passa a vez e a tela de comprar a cidade desaparece para o jogador 
    public void CloseBuyHouse()
    {
        _uiBuyCity.SetActive(false);
        photonView.RPC("NextTurn", RpcTarget.All);
    }


    //esse faz a mesma função do método acima, a diferença é que esse será chamando no close da tela de construir casas
    public void CloseBuildHouse()
    {
        _uiBuildHouse.SetActive(false);
        photonView.RPC("NextTurn", RpcTarget.All);
    }

    //esse faz a mesma função do método acima, a diferença é que esse será chamando no close da tela de recomprar a cidade
    public void CloseRebuy()
    {
        _uiRebuy.SetActive(false);
        photonView.RPC("NextTurn", RpcTarget.All);
    }

    //esse faz a mesma função do método acima, a diferença é que esse será chamando no close da tela do Lost Island
    public void CloseLostIsland()
    {
        _uiLostIsland.SetActive(false);
        photonView.RPC("NextTurn", RpcTarget.All);
    }

    //esse faz a mesma função do método acima, a diferença é que esse será chamando no close da tela do World Championship
    public void CloseWorldChampionship()
    {
        _uiWorldChampionship.SetActive(false);
        photonView.RPC("NextTurn", RpcTarget.All);
    }

    //esse faz a mesma função do método acima, a diferença é que esse será chamando no close da tela do World Tour
    public void CloseWorldTour()
    {
        _uiWorldTour.SetActive(false);
        photonView.RPC("NextTurn", RpcTarget.All);
    }

    public void CloseStart()
    {
        _uiStart.SetActive(false);
        photonView.RPC("NextTurn", RpcTarget.All);
    }


    //esse método valida se o button pode ou não ser interativo para o player atual
    private void ButtonsHouseValidation()
    {

        // caso a cidade tenha um level maior que 1, o jogador não poderá interagir com o button de contruir casas lvl 1
        if (currentCity.GetComponent<City>().LevelCity > 1)
        {
            _houseLevel1.interactable = false;
        }
        //Porém caso seja level 1, o jogador poderá interagir com o button de criar casas level 1
        else if (currentCity.GetComponent<City>().LevelCity == 1)
        {
            _houseLevel1.interactable = true;
        }

    }
}
