using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player;                                                            //Variavel que pega o transform do player
    public Transform bulletSpawn;                                                       //Variavel que pega o transform do spawn da bullet
    public Slider healthBar;                                                            //Variavel que pega o slider da vida do player
    public GameObject bulletPrefab;                                                     //Variavel que pega o prefab da bullet

    NavMeshAgent agent;                                                                 //Variavel do agent
    public Vector3 destination;                                                         //Variavel do destino
    public Vector3 target;                                                              //Variavel do alvo                                  
    float health = 100.0f;                                                              //Variavel da vida do npc
    float rotSpeed = 5.0f;                                                              //Varialvel da velocidade de rotação

    float visibleRange = 80.0f;                                                         //Variavel que passa o raio de visibilidade
    float shotRange = 40.0f;                                                            //Variavel que passa o alcance do tiro

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();                                      //Passa as propriedades do NavMeshAgent para a varivel agent
        agent.stoppingDistance = shotRange - 5;                                         //for a little buffer
        InvokeRepeating("UpdateHealth",5,0.5f);                                         //Atualiza a vidado npc a cada 5 segundos
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position); //Passa a posição da barra de vida do npc
        healthBar.value = (int)health;                                                  //Aplica o valor da variavel de vida ao slider
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);              //Posiciona a barra de vida do npc
    }

    void UpdateHealth()                                                                 //Se a vida for menor que 100, acrescenda +1 na barra de vida
    {
       if(health < 100)
        health ++;
    }

    void OnCollisionEnter(Collision col)                                                //Se colidir com a bullet, tira -10 da barra de vida
    {
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));//Variavel de vector3 que passa uma localização aleatória
        agent.SetDestination(dest);                                                     //move o agent para a localização
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()                                                     //Debuga o tempo em que o agent está se movendo
    {
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if(agent.remainingDistance >= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
}

