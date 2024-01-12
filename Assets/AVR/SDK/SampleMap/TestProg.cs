using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProg : MonoBehaviour
{
    GameObject GetSpawn(GameObject[] Spawn) {
        return Spawn[Random.Range(0, Spawn.Length)];
    }
}
