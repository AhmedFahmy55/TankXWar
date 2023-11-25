using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamageOnCollision : MonoBehaviour
{

    [SerializeField] int damage;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody == null) return;

        if(collision.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damage);
        }
    }
}
