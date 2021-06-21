using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;
/*
public struct CreateMMOCharacterMessage : NetworkMessage
{
    public Color color;
}


public class CustomNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateMMOCharacterMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        // you can send the message here, or wherever else you want
        CreateMMOCharacterMessage characterMessage = new CreateMMOCharacterMessage
        {
            //material = Material
            color = Color.red
        };

        conn.Send(characterMessage);
    }

    void OnCreateCharacter(NetworkConnection conn, CreateMMOCharacterMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject gameobject = Instantiate(playerPrefab);

        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties
        PlayerController player = gameobject.GetComponent<PlayerController>();
        player.color = Color.red;

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }
}
*/