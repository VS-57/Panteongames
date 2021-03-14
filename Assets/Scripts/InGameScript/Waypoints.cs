using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour {

    public GameObject[] waypoints; // Haraketli engellerin sekmesinin istediginiz noktaları tutan liste tanımlaması.
    public GameObject player; // Başlangıç noktası
    int current = 0;
    public float speed;
    float WPradius = 1;
	
	void Update () {
		if(Vector3.Distance(waypoints[current].transform.position, transform.position) < WPradius)
        {
            current = Random.Range(0,waypoints.Length);
            if (current >= waypoints.Length)
            {
                current = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[current].transform.position, Time.deltaTime * speed);

    }
}