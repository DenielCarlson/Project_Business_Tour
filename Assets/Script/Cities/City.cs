using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public int IdCity;
    public int IdOwner;
    public bool HasPlayer;
    public bool WasBought;
    public float InitialPrice;
    public float RentPrice { get; private set; }

    public GameObject Player { get; set; }

    private int _levelCity;
    private bool _isOwn;

    [SerializeField] private Transform _spawn;
    [SerializeField] private GameObject _uiBuyCity;

    private void Awake()
    {
        _uiBuyCity.SetActive(false);
    }

    private void Update()
    {
        if (IdOwner > 0)
        {
            _isOwn = true;
            WasBought = true;
        }
    }

    void EnableUIBuyHouse()
    {
        _uiBuyCity.SetActive(true);
    }

    public void BuildHouse()
    {
        GameObject house = Resources.Load("HouseLevel1") as GameObject;
        Instantiate(house, _spawn.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HasPlayer = true;
            Player = other.gameObject;
            Invoke("EnableUIBuyHouse", 0.5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HasPlayer = false;
        Player = null;
    }
}
