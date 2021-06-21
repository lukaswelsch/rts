using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CreateTrike : MonoBehaviour
{
    public void SpawnTrike()
    {
        GameObject localPlayer = NetworkClient.localPlayer.gameObject;
        localPlayer.GetComponent<PlayerController>().SpawnTrikeCmd(null);
    }
}
