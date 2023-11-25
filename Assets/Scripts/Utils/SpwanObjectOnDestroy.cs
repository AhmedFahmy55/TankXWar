using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanObjectOnDestroy : MonoBehaviour
{

    [SerializeField] GameObject objectToSpwan;


    private void OnDestroy()
    {
        Instantiate(objectToSpwan,transform.position,Quaternion.identity);
    }
}
