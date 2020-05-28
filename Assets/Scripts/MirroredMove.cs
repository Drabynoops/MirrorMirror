using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirroredMove : MonoBehaviour
{
    public GameObject realObject;
    public Vector3 newPosition;
    public float offset;
    public float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        yRotation = realObject.transform.eulerAngles.y * -1 + 180;
        newPosition = realObject.transform.position;
        newPosition.z = -newPosition.z;
        newPosition.z = newPosition.z + offset;
        transform.position = newPosition;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
    }
}
