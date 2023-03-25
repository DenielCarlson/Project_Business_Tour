using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour
{
    public int IdCity;
    public bool HasPlayer;
    public bool WasBought;
    public float InitialPrice;
    public float RentPrice { get; private set; }

    public GameObject Player { get; set; }

    private int _levelCity;

    private int _idOwner;
    private bool _isOwn;

    [SerializeField] private Transform spawn;
    [SerializeField] private GameObject _uiBuyCity;

    private void Awake()
    {
        _uiBuyCity.SetActive(false);
    }

    void EnableUIBuyHouse()
    {
        _uiBuyCity.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HasPlayer = true;
            Player = other.gameObject;
            Invoke("EnableUIBuyHouse", 0.8f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HasPlayer = false;
        Player = null;
    }
}
