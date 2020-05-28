using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour //attach thiswwwww script to a trigger collider object attached to the inside of pipes
{
    public PlayerController playControl;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        if (!playControl.boxMove)
        {
            playControl.playerControl = true;
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name != "Player")  // or if(gameObject.CompareTag("YourWallTag"))
        {
            Debug.Log("YOU");
            playControl.playerControl = false;
        } 
    }
}