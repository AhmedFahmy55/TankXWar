using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpin : MonoBehaviour
{
    [SerializeField] private float spinDegreePerSec = 270;
    [SerializeField] private SpinAxis spinAxis = SpinAxis.Z;

    // Update is called once per frame
    void Update()
    {
        switch(spinAxis)
        {
            case SpinAxis.Z: transform.Rotate(0,0,-spinDegreePerSec * Time.deltaTime); break;
            case SpinAxis.X: transform.Rotate(-spinDegreePerSec * Time.deltaTime, 0, 0); break;
            case SpinAxis.Y: transform.Rotate(0, -spinDegreePerSec * Time.deltaTime, 0); break;
            case SpinAxis.None: break;
        }
    }

    private enum SpinAxis
    {
        None,
        X,
        Y,
        Z,
    }
}
