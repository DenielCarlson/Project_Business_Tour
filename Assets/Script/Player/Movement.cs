using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class Movement : MonoBehaviour
{

    private GameObject[] _cities;//Array que vai Armazenar todos os blocos do jogo
    private Vector3 direction;//Direção que o player vai seguir
    private Vector3 _currentPos;//posição atual do player
    public int RandomNum;//Essa variavel guarda um número aleatório
    
    [SerializeField] private int _round = 0;//Round que o player tá
    
    [SerializeField] private float _speed;//Velocidade que o player se movimenta


    private void Start()//Aqui o array _cities é inicializado pegando todos os gameObjects com a tag Houses, ou seja, as cidades
    {
        _cities = GameObject.FindGameObjectsWithTag("Houses");

    }


    private void Update()
    {


        //_currentPos recebe as coordenadas x e z de _cities[RandomNum] que no caso é a posição que o player deve ir
         _currentPos = new Vector3(_cities[RandomNum].transform.position.x, transform.position.y, _cities[RandomNum].transform.position.z);

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

        //Quando o player se colidir com algo, no caso as cidades, ele sempre ficará na mesma posição do eixo x, e z dessa mesma cidade
       transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z);

        //Se o player estiver colidindo com blocos da tag Houses que são as cidades o if se ativará
        if (other.gameObject.CompareTag("Houses"))
        {

            //se ele estiver no bloco Start, a direção do player é Subindo e roud é incrementado + 1
            if (other.gameObject == GameObject.Find("Start"))
            {
                    _round++;
                direction = new Vector3(0, 0, 1 * Time.deltaTime * _speed);
            }

            //se ele estiver no bloco Lost Island, a direção do player é Direita
            else if (other.gameObject == GameObject.Find("Lost Island"))
            {
                direction = new Vector3(1 * Time.deltaTime * _speed, 0, 0);
            }

            //se ele estiver no bloco World Championships, a direção do player é Descendo
            else if (other.gameObject == GameObject.Find("World Championships"))
            {
                direction = new Vector3(0, 0, -1 * Time.deltaTime * _speed);
            }

            //se ele estiver no bloco World Tour, a direção do player é Esquerda
            else if (other.gameObject == GameObject.Find("World Tour"))
            {
                direction = new Vector3(-1 * Time.deltaTime * _speed, 0, 0);
            }
        }

    }
}
