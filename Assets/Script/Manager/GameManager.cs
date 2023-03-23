using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance { get; private set; }
    private List<Player> _players;
    public List<Player> Players { get => _players; private set => _players = value; }
    private int _playersInGame;
    private PhotonView photonView;

    [SerializeField] private Transform _spawn;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            return;
        }
        photonView = GetComponent<PhotonView>();

        Instance = this;
        _players = new List<Player>();
        _playersInGame = 0;

    }

    public void OnRollDiceClick()
    {
        if (photonView.IsMine)
        {
            int dice;
            foreach (Player players in _players)
            {
                if (players == PhotonNetwork.LocalPlayer)
                {
                    Movement[] myPlayerScript = FindObjectsOfType<Movement>().Where(x => x.PhotonPlayer == players).ToArray();

                    if (myPlayerScript != null)
                    {
                        dice = Random.Range(2, 13);
                        myPlayerScript[0].CityIndex(dice);

                        Debug.Log("GameManager: " + players);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void StartGame()
    {
        _playersInGame++;

        if (_playersInGame == PhotonNetwork.PlayerList.Length)
        {
            CreatePlayer();
        }
    }

    public void CreatePlayer()
    {
        GameObject myPlayerObject = PhotonNetwork.Instantiate("Player", _spawn.position, Quaternion.identity);
        var myPlayerMovement = myPlayerObject.GetComponent<Movement>();
        myPlayerMovement.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }
}
