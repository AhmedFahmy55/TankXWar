using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealZone : NetworkBehaviour
{


    


    private List<PlayerTank> playersTanks = new List<PlayerTank>();



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) return;
        if(!collision.attachedRigidbody.TryGetComponent<PlayerTank>(out PlayerTank playerTank)) return;
        if (playersTanks.Contains(playerTank)) return;

        playersTanks.Add(playerTank);
        Debug.Log("Adding player " + playerTank.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (!collision.attachedRigidbody.TryGetComponent<PlayerTank>(out PlayerTank playerTank)) return;
        if (!playersTanks.Contains(playerTank)) return;

        playersTanks.Remove(playerTank);
        Debug.Log("Removing player " + playerTank.name);
    }
}
