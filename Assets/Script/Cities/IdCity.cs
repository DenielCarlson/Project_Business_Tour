using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdCity : MonoBehaviour
{
    public int ID { get => _id; private set => _id = value; }
    [SerializeField] private int _id;
}
