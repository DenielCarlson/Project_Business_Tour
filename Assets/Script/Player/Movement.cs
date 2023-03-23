using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class Movement : MonoBehaviour
{
    private Vector3 direction;//Direção que o player vai seguir
    private Vector3 _currentPos;//posição atual do player

    private GameObject[] _cities;//Array que vai Armazenar todos os blocos do jogo

    public int RandomNum;//Essa variavel guarda um número aleatório

    [SerializeField] private int _round = 0;//Round que o player tá
    [SerializeField] private float _speed;//Velocidade que o player se movimenta


    private void Start()//Aqui o array _cities é inicializado pegando todos os gameObjects, os blocos no caso
    {
        _cities = new GameObject[]
        {
            GameObject.Find("Start"),
            GameObject.Find("City 1"),
            GameObject.Find("City 2"),
            GameObject.Find("City 3"),
            GameObject.Find("City 4"),
            GameObject.Find("City 5"),
            GameObject.Find("City 6"),
            GameObject.Find("City 7"),
            GameObject.Find("Lost Island"),
            GameObject.Find("City 8"),
            GameObject.Find("City 9"),
            GameObject.Find("City 10"),
            GameObject.Find("City 11"),
            GameObject.Find("City 12"),
            GameObject.Find("City 13"),
            GameObject.Find("City 14"),
            GameObject.Find("World Championships"),
            GameObject.Find("City 15"),
            GameObject.Find("City 16"),
            GameObject.Find("City 17"),
            GameObject.Find("City 18"),
            GameObject.Find("City 19"),
            GameObject.Find("City 20"),
            GameObject.Find("City 21"),
            GameObject.Find("World Tour"),
            GameObject.Find("City 22"),
            GameObject.Find("City 23"),
            GameObject.Find("City 24"),
            GameObject.Find("City 25"),
            GameObject.Find("City 26"),
            GameObject.Find("City 27"),
            GameObject.Find("City 28"),
        };


    }


    private void Update()
    {


        //_currentPos recebe as coordenadas x e z de _cities[RandomNum] que no caso é a posição que o player deve ir
        _currentPos = new Vector3(Random.Range(_cities[RandomNum].transform.position.x - 1, _cities[RandomNum].transform.position.x - 1),
            transform.position.y, Random.Range(_cities[RandomNum].transform.position.z - 1, _cities[RandomNum].transform.position.z - 1));

        // Se a poção do player for diferente de _currentPos, ele deve se mover pelo tabuleiro até que sua posiçãi seja igual a _currentPos
        if (transform.position != _currentPos)
        {

            // Player se movimento em seu próprio eixo de acordo com a direção da variavel direction
            transform.Translate(direction, Space.Self);

        }
    }

    public void RollDice()// esse método me retorna um número aleatório
    {
        int dice = Random.Range(2, 12);// vai ser gerando um número aleatório entre 2 e 12
        RandomNum += dice; //randomNum recebe esse númeto aleátorio + o seu próprio valor

        //Se randomNum for maior que 31, randomNum será ele menos 31, assim restando a diferença e completando a volta no tabuleiro
        //Isso evita com que o RandomNum que serve de parâmetro para _cities[], não ultrapasse os limites do vetor
        if (RandomNum > 31)

        {
            int deltaRandomNum = RandomNum - 31;
            RandomNum = 0 + deltaRandomNum;
        }
    }




    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("LeftCity") || other.gameObject.CompareTag("RightCity"))
        {
            transform.position = new Vector3(other.gameObject.transform.position.x - 1, transform.position.y, other.gameObject.transform.position.z);
        }else if (other.gameObject.CompareTag("UpCity") || other.gameObject.CompareTag("DownCity"))
        {
            transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z - 1);
        }
        else
        {
            transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z);
        }


        //se ele estiver no bloco Start, a direção do player é Subindo e roud é incrementado + 1
        if (other.gameObject == GameObject.Find("Start"))
        {
            _round++;
            direction = Vector3.forward * Time.deltaTime * _speed;
        }

        //se ele estiver no bloco Lost Island, a direção do player é Direita
        else if (other.gameObject == GameObject.Find("Lost Island"))
        {
            direction = Vector3.right * Time.deltaTime * _speed;
        }

        //se ele estiver no bloco World Championships, a direção do player é Descendo
        else if (other.gameObject == GameObject.Find("World Championships"))
        {
            direction = Vector3.back * Time.deltaTime * _speed;
        }

        //se ele estiver no bloco World Tour, a direção do player é Esquerda
        else if (other.gameObject == GameObject.Find("World Tour"))
        {
            direction = Vector3.left * Time.deltaTime * _speed;
        }
    }
}
