using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

	float speed = 20.0F;                                                                        //Variavel de veolicade
    float rotationSpeed = 120.0F;                                                               //Variavel da velocidade de rotação
    public GameObject bulletPrefab;                                                             //Variavel que pega o prefab da bullet
    public Transform bulletSpawn;                                                               //Variavel que pega o transform do spawn da bullet

    void Update() {
        float translation = Input.GetAxis("Vertical") * speed;                                  //Variavel que pega o eixo vertical para movimentação
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;                           //Variavel que pega o eixo horizontal para a rotação
        translation *= Time.deltaTime;                                                          //Chama a variavel desde o ultimo quadro até o atual
        rotation *= Time.deltaTime;                                                             //Chama a variavel desde o ultimo quadro até o atual
        transform.Translate(0, 0, translation);                                                 //Movimenta o player
        transform.Rotate(0, rotation, 0);                                                       //Rotaciona o player

        if(Input.GetKeyDown("space"))                                                           //Se apertar a tecla espaço, é instanciado o prefab da bullet na scene com uma força de 2000
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
