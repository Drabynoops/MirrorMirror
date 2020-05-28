using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{

    public float speed = 2.0f;
    private float X = 0.0f;
    private float Y = 0.0f;
    public Vector3 offset;
    public GameObject player;


    private void Awake()
    {
        offset = transform.position - player.transform.position;
        Vector3 euler = transform.rotation.eulerAngles;
        X = euler.x;
        Y = euler.y;
    }
    void Update()
    {
            const float MIN_X = 0.0f;
            const float MAX_X = 360.0f;
            const float MIN_Y = -89.0f;
            const float MAX_Y = 89.0f;

            X += Input.GetAxis("Mouse X") * (speed * Time.deltaTime);
            if (X < MIN_X) X += MAX_X;
            else if (X > MAX_X) X -= MAX_X;
            Y -= Input.GetAxis("Mouse Y") * (speed * Time.deltaTime);
            if (Y < MIN_Y) Y = MIN_Y;
            else if (Y > MAX_Y) Y = MAX_Y;

            transform.rotation = Quaternion.Euler(Y, X, 0.0f);
    }
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.localPosition = player.transform.position + offset;
    }
}