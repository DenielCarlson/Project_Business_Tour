using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPlayerController : MonoBehaviour
{
    //esse método acessa o método RollDice da classe Movement do gameObject Player 
    //RollDice retornará um númeto inteiro aleatório, esse número será um valor entre 2 e 12
    //O método Roll  dessa classe "CanvasPlayerController" só é chamado através do button chamado Roll
    // esse button é um objecto filho de Canvas Player que é um objeto filho do GameObject Player
    public void Roll()
    {

        GameObject player = GameObject.Find("Player");

        player.GetComponent<Movement>().RollDice();

       GameObject.Find("CanvasPlayer").SetActive(false);
    }
}
