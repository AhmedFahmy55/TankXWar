using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankAiming : NetworkBehaviour
{
    [SerializeField] InputReader inputReader;
    [SerializeField] Transform turret;


    private void LateUpdate()
    {
        if (!IsOwner || !Camera.main) return;

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(inputReader.MousePosition);

        turret.up = new Vector2 (
            mouseWorldPosition.x - turret.position.x,
            mouseWorldPosition.y- turret.position.y).normalized;
    }
}
