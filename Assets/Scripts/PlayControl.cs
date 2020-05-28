using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayControl : MonoBehaviour
{
    public float speed = 10.0f;
    public float normSpeed = 10f;
    public float pushSpeed = 3f;
    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public bool grounded = false;
    public Rigidbody rb;
    public GameObject cam;
    public GameObject pushCam;
    private int numKeysRed = 0;
    private int numKeysPurp = 0;
    private int numKeysBlue = 0;
    public Animator anim;
    public Animator playerAnim;
    public bool moveBox = false;
    private GameObject movingObject;
    public float distToGround;
    public Collider coll;
    public AudioSource sourcePush;
    public AudioSource sourceJump;
    public AudioSource sourceWalk;
    public AudioSource sourceMusic;
    public AudioSource chatter;
    public AudioClip chattering;
    public Text Redkey;
    public Text Bluekey;
    public Text Purplekey;

    void Awake()
    {
        chatter.PlayOneShot(chattering);
        //sourcePush = GetComponent<AudioSource>();
        //sourceJump = GetComponent<AudioSource>();
        distToGround = coll.bounds.extents.y;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }
    RaycastHit hit;
    bool IsBox()
    {
        return Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1f);
    }
    private void Update()
    {
        
        if (numKeysBlue > 0)
        {
            Bluekey.text = ("Blue Keys: " + numKeysBlue);
        } else
        {
            Bluekey.text = ("");
        }
        if (numKeysRed > 0)
        {
            Redkey.text = ("Red Keys: " + numKeysRed);
        }
        else
        {
            Redkey.text = ("");
        }
        if (numKeysPurp > 0)
        {
            Purplekey.text = ("Purple Keys: " + numKeysPurp);
        }
        else
        {
            Purplekey.text = ("");
        }

        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (IsGrounded())
        {
            if (Input.GetKey(KeyCode.Mouse0) && moveBox == true)
            {
                movingObject.transform.parent = gameObject.transform;
                cam.SetActive(false);
                pushCam.SetActive(true);
                anim.SetBool("Pushing", true);
                playerAnim.SetBool("Pushing", true);
                speed = pushSpeed;
                if (!sourcePush.isPlaying)
                {
                    sourcePush.Play();
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                moveBox = false;
                cam.SetActive(true);
                pushCam.SetActive(false);
                movingObject.transform.parent = null;
                anim.SetBool("Pushing", false);
                playerAnim.SetBool("Pushing", false);
                speed = normSpeed;
                sourcePush.Stop();
            }
            sourceJump.Stop();
            anim.SetBool("Jumping", false);
            if (velocity.x > .05f || velocity.z > .05f || velocity.x < -.05f || velocity.z < -.05f)
            {
                if (!sourceWalk.isPlaying)
                {
                    sourceWalk.Play();
                }
                anim.SetBool("Moving", true);
                playerAnim.SetBool("Moving", true);
            }
            else
            {
                sourceWalk.Stop();
                anim.SetBool("Moving", false);
                playerAnim.SetBool("Moving", false);
            }
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                Debug.Log("aight");
            }
        }
        else
        {
            if (!sourceJump.isPlaying)
            {
                sourceJump.Play();
            }
            anim.SetBool("Jumping", true);
        }

        // We apply gravity manually for more tuning control
        rb.AddForce(new Vector3(0, -gravity * rb.mass, 0));
        var CharacterRotation = cam.transform.rotation;
        CharacterRotation.x = 0;
        CharacterRotation.z = 0;
        transform.rotation = CharacterRotation;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "goal")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (other.gameObject.tag == "BlueKey")
        {
            numKeysBlue = numKeysBlue + 1;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "BlueLock")
        {
            if (numKeysBlue >= 1)
            {
                numKeysBlue = numKeysBlue - 1;
                other.gameObject.tag = "unlocked";
                other.gameObject.GetComponent<LockScript>().Unlock();
            }
        }
        else if (other.gameObject.tag == "RedKey")
        {
            numKeysRed = numKeysRed + 1;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "RedLock")
        {
            if (numKeysRed >= 1)
            {
                numKeysRed = numKeysRed - 1;
                other.gameObject.tag = "unlocked";
                other.gameObject.GetComponent<LockScript>().Unlock();
            }
        }
        else if (other.gameObject.tag == "PurpKey")
        {
            numKeysPurp = numKeysPurp + 1;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "PurpLock")
        {
            if (numKeysPurp >= 1)
            {
                numKeysPurp = numKeysPurp - 1;
                other.gameObject.tag = "unlocked";
                other.gameObject.GetComponent<LockScript>().Unlock();
            }
        }
        else if (other.gameObject.tag == "Moveable" && IsBox())
        {
            Debug.Log("Goober");
            moveBox = true;
            movingObject = other.gameObject;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Moveable")
        {
            movingObject = null;
            moveBox = false;
        }
    }
}
