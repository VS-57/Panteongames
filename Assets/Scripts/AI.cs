using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Transform[] finish;
    public Transform spawnpoint;
    public UnityEngine.AI.NavMeshAgent agent;
    private int count = 0;
    public Animator anim;
   
    void Update()
    {
        agent.SetDestination(finish[count].position); // Agent'i yönlendirme satırı
        if(count == finish.Length)
        {
            agent.Stop();
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Checkpoint") // Checkpoint'e gelen agent'i sonraki noktaya yönlendirme script'i
        {
            count++;
        }
    }

    void OnCollisionEnter(Collision col) 
    {
        if (col.gameObject.tag == "Obstacles") // Spawn noktasına gönderilme için oluşturulan fonksiyon.
        {
            transform.position = spawnpoint.position;
            transform.Rotate(0, 270, 0);
            count = 0;
        }


    }
}
