using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class netSpawner : NetworkBehaviour
{

    [SerializeField]
    GameObject Prefab;
    [SerializeField]
    GameObject Spawn;
    [SerializeField]
    public int Count;
    [SerializeField]
    public int Amount = 10;

    public override void OnStartServer()
    {
        for (int i = 0; i < Amount; i++)
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        //Count

        GameObject go = GameObject.Instantiate(Prefab, Spawn.transform.position, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(go);
    }
}