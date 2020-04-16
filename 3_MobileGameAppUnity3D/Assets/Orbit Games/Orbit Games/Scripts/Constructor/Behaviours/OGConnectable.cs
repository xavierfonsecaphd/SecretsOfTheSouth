using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OGConnectable : MonoBehaviour {

    public static readonly float OBJECT_SIZE = 50f;
    public static readonly float SIDE_INDICATOR_SIZE = 0.1f;
    public static readonly float SCALE_INDICATOR_SIZE = 0.04f;
    
    private void OnDrawGizmos()
    {
        var scale = transform.TransformVector(Vector3.right).magnitude * OBJECT_SIZE; 
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(Vector3.left * OBJECT_SIZE), scale * SIDE_INDICATOR_SIZE);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.TransformPoint(Vector3.right * OBJECT_SIZE), scale * SIDE_INDICATOR_SIZE);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, scale);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.TransformPoint(Vector3.left * OBJECT_SIZE), transform.TransformPoint(Vector3.right * OBJECT_SIZE));
        Gizmos.DrawLine(transform.position, transform.TransformPoint(Vector3.up * OBJECT_SIZE));
        Gizmos.DrawSphere(transform.TransformPoint(Vector3.up * OBJECT_SIZE), scale * SCALE_INDICATOR_SIZE);
        Gizmos.DrawLine(transform.position, transform.TransformPoint(Vector3.down * OBJECT_SIZE));
        Gizmos.DrawSphere(transform.TransformPoint(Vector3.down * OBJECT_SIZE), scale * SCALE_INDICATOR_SIZE);
    }
}
