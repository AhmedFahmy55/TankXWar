using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TankMovement : NetworkBehaviour
{
    [Header("Refs")]
    [SerializeField] InputReader inputReader;
    [SerializeField] private Transform tankbody;

    [Space,Header("Settings")]
    [SerializeField] private float rotaionSpeed;
    [SerializeField] private float movementSpeed = 5;

        
    private Rigidbody2D rb;




    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (!IsOwner) return;
        tankbody.Rotate(0,0, inputReader.Movement.x * rotaionSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        rb.velocity = tankbody.up * movementSpeed * inputReader.Movement.y;
    }
}
