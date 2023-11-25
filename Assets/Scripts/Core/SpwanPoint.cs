using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPoint : MonoBehaviour
{
    
    private static List<SpwanPoint> spwanPoints = new List<SpwanPoint>();





    private void OnEnable()
    {
        spwanPoints.Add(this);
    }
    private void OnDisable()
    {
        spwanPoints.Remove(this);
    }

    public static Vector3 GetRandomSpwanPint()
    {
        if(spwanPoints.Count == 0) return Vector3.zero;
        return spwanPoints[UnityEngine.Random.Range(0, spwanPoints.Count)].transform.position;
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position,1);
    }
#endif
}
