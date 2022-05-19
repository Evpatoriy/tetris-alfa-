using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    
    public GameObject[] Tetraminoes;
 
    void Start()
    {
        NewTetraminoes();
    }

    public void NewTetraminoes () {
        Instantiate(Tetraminoes[Random.Range(0, Tetraminoes.Length)], transform.position, Quaternion.identity);
    }
}
