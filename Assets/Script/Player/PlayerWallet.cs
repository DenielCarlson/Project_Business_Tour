using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    public float Money { get => _money; private set => _money = value; }
    [SerializeField] private float _money; 
    public void Deposit(float DepositValue)
    {
        Money += DepositValue;
    }

    public void Withdraw(float WithdrawValue)
    {
        Money -= WithdrawValue;
    }

    public void Transfer(GameObject originPlayer, GameObject recipientPlayer, float tranferValue)
    {
        originPlayer.GetComponent<PlayerWallet>().Withdraw(tranferValue);
        recipientPlayer.GetComponent<PlayerWallet>().Deposit(tranferValue);
    }
}
