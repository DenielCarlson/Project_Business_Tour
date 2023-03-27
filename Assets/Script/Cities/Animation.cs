using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    private Vector3 defaultPos;// essa receberá a posição atual desse bloco que no caso é a primeira posição

    void Start()
    {
        defaultPos = transform.position;//aqui ela é inicializada recebendo a posição atual desse bloco
    }

    // esse método por enquanto não foi chamando, mas quando ele for, ele vai fazer com que esse bloco receba defaultPos
    void ReturnPosition() { 

        transform.position = defaultPos;
    }


    private void OnTriggerEnter(Collider other)
    {

        //Se esse Bloco estiver colidindo com o player, esse bloco aumentará sua eixo y em 0.5
        // e 0.2 segundo depois o métódo Invoke() chamara o método ReturnPosition que fará com que a posição desse objeto seja a mesma de antes 
        // Isso é possível porque a variável default position armazenou a primeira posição
        if (other.gameObject.CompareTag("Player"))
        {

            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            Invoke("ReturnPosition", 0.2f);

        }
    }
}
