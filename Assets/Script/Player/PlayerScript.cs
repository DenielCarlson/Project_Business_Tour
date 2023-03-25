using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

public class PlayerScript : MonoBehaviourPunCallbacks
{
    public GameObject[] Cities { get => _cities; private set => _cities = value; }
    public int CityIndexVar { get => _cityIndex; private set => _cityIndex = value; }
    
    private GameObject[] _cities;//Array que vai Armazenar todos os blocos do jogo

    private Vector3 direction;//Direção que o player vai seguir
    private Vector3 _currentPos;//posição atual do player
    PhotonView photonview;//Componente que faz com que outros possam receber as minhas informações

    public int RecipientIdCity { get; private set; }

    //Essas variáveis verificam eu qual lado meu player está
    public bool _isRightOrLeft;
    public bool _isUpOrDown;

    [SerializeField] private int _round = 0;//Round que o player tá
    [SerializeField] private float _speed;//Velocidade que o player se movimenta
    [SerializeField] private int _cityIndex;//Essa variavel guarda um número aleatório

    public Player PhotonPlayer { get => _photonPlayer; private set => _photonPlayer = value; }
    public int ID { get => _id; private set => _id = value; }

    private Player _photonPlayer;
    private int _id;

    public List<GameObject> MyCities;

    [PunRPC]
    public void Initialize(Player player)
    {
        _photonPlayer = player;
        _id = player.ActorNumber;

        GameManager.Instance.Players.Add(player);
        GameManager.Instance.PlayerObjects.Add(gameObject);
    }

    private void Awake()
    {
        photonview = GetComponent<PhotonView>();
        MyCities = new List<GameObject>();
    }


    private void Start()
    {
        //Aqui o array _cities é inicializado pegando todos os gameObjects, os blocos no caso
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

        _cityIndex = 0;//_cityIndex sempre começará com 0, pois é o index do bloco "Start"
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            Move();
            RecipientIdCity = _cities[_cityIndex].GetComponent<City>().IdCity;
            
        }
    }

    private void Move()
    {
        //De acordo com o lado do meu player, ele receberá diferentes posições - pode ser melhorado no futuro para não ficar tão fixo
        if (_isRightOrLeft)
        {
            _currentPos = new Vector3(_cities[_cityIndex].transform.position.x - 1, transform.position.y, _cities[_cityIndex].transform.position.z);
        }
        else if (_isUpOrDown)
        {
            _currentPos = new Vector3(_cities[_cityIndex].transform.position.x, transform.position.y, _cities[_cityIndex].transform.position.z - 1);
        }
        else
        {
            _currentPos = new Vector3(_cities[_cityIndex].transform.position.x, transform.position.y, _cities[_cityIndex].transform.position.z);
        }

        // Se a poção do player for diferente de _currentPos, ele deve se mover pelo tabuleiro até que sua posição seja igual a _currentPos
        if (transform.position != _currentPos)
        {

            // Player se movimento em seu próprio eixo de acordo com a direção da variavel direction
            transform.Translate(direction, Space.Self);

        }
    }

    public void CityIndex(int dice)//Esse método vai fazer com que _cityIndex receba o resultado do dado
    {
        _cityIndex += dice; //_cityIndex recebe esse númeto aleátorio + o seu próprio valor

        //Se randomNum for maior que 31, randomNum será ele menos 31, assim restando a diferença e completando a volta no tabuleiro
        //Isso evita com que o  _cityIndex que serve de parâmetro para _cities[], não ultrapasse os limites do vetor
        if (_cityIndex > 31)
        {
            int deltaRandomNum = _cityIndex - 31;
            _cityIndex = 0 + deltaRandomNum;
        }
    }



    private void OnTriggerEnter(Collider other)
    {

        //Se meu Player estiver nas cidades da direita ou esquerda, seu eixo x será o mesmo da cidade, porém - 1 
        if (other.gameObject.CompareTag("LeftCity") || other.gameObject.CompareTag("RightCity"))
        {
            _isRightOrLeft = true;
            _isUpOrDown = false;

            transform.position = new Vector3(other.gameObject.transform.position.x - 1, transform.position.y, other.gameObject.transform.position.z);
        }

        //Se meu Player estiver nas cidades de cima ou de baixo, seu eixo z será o mesmo da cidade, porém - 1 
        else if (other.gameObject.CompareTag("UpCity") || other.gameObject.CompareTag("DownCity"))
        {
            _isUpOrDown = true;
            _isRightOrLeft = false;
            transform.position = new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z - 1);
        }

        //Se meu player não estiver nem nas cidade dos lados nem em cima ou em baixo, sua posição será a mesma da cidade onde ele se encontra
        else
        {
            _isUpOrDown = false;
            _isRightOrLeft = false;
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

