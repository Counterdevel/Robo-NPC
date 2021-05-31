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
        Task.current.Succeed();                                                         //Indica que o processo foi executa de forma correta
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

[Task]
    public void PickDestination(int x, int z)
    {
        Vector3 dest = new Vector3(x, 0, z);                                            //Variavel de vector3 que passa uma localização
        agent.SetDestination(dest);                                                     //move o agent para a localização
        Task.current.Succeed();                                                         //Indica que o processo foi executa de forma correta
    }

[Task]
    public void TargetPlayer()
    {
        target = player.transform.position;                                             //Pega a posição do player;
        Task.current.Succeed();                                                         //Indica que o processo foi executa de forma correta
    }

[Task]
    public bool Fire()
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation); //Instancia o prefab da bullet
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);                           //Adiciona uma força a bullet de 2000

        return true;                                                                    //Retorna o valor com verdadeiro
    }

[Task]
    public void LookAtTarget()
    {
        Vector3 direction = target - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed); //Passa a velocidade e suaviza a rotação do npc

        if(Task.isInspected)
        {
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction)); //Informa o angulo
        }

        if(Vector3.Angle(this.transform.forward, direction) < 5.0f)                     //Se o angulo for menor que 5
        {
            Task.current.Succeed();                                                     //Indica que o processo foi executa de forma correta
        }
    }

[Task]
    public bool SeePlayer()                                                                                     //Método que recebe informações de colisão com player e com a parede
    {
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);

        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }

        if(Task.isInspected)
        {
            Task.current.debugInfo = string.Format("wall={0}", seeWall);
        }

        if(distance.magnitude < visibleRange && !seeWall)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

[Task]
    bool Turn(float angle)                                                                                  //Método que rotaciona para uma posição e a deixa preestabelecida
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;

        return true;
    }

[Task]
    public bool IsHealthLessThan(float health)                                                              //Método que retorna a vida atual do robo
    {
        return this.health < health;
    }

[Task]
    public bool Explode()                                                                                   //Método que destroi o robo
    {
        Destroy(healthBar.gameObject);
        Destroy(this.gameObject);
        return true;
    }
}

