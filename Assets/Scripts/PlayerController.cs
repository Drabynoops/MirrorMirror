using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region public global variables
    public float speed; //speed of player
    public float normSpeed;
    public bool limitDiagonalSpeed = true;
    public float grav; //gravity
    public float jumpSpeed; //jump height
    public float fallThreshold;
    public Vector3 moveDir = Vector3.zero; //vector of movement
    private bool airControl = true;
    public bool playerControl = true;
    public float pushPower = 2f;
    public float distance;
    public bool boxMove = false;
    #endregion

    #region private global variables
    private bool grounded = false;
    private Transform trans;
    private float fallStartLevel;
    private bool falling;
    private float slideLimit;
    private float slideSpeed;
    private bool slideWhenOverSlopeLimit = true;
    private float rayDistance;
    private Vector3 contactPoint;
    private int jumpTimer;
    private int antiBunnyHop = 1;
    private int numKeys = 0;
    #endregion

    #region public global objects
    public GameObject cam;
    public GameObject pushCam;
    public CharacterController charControl;
    public Animator anim;
    public Animator playerAnim;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        charControl = GetComponent<CharacterController>();
        //playerAnim = GetComponent<Animator>();
        rayDistance = charControl.height * .5f + charControl.radius;
        slideLimit = charControl.slopeLimit - .1f;
        jumpTimer = antiBunnyHop;
    }

    private void Update()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(trans.position, trans.TransformDirection(Vector3.forward), out hit, distance) && hit.transform.tag == "Moveable" && Input.GetKey(KeyCode.Mouse0) && grounded)
        {
            boxMove = true;
            Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            //Debug.Log("Did Hit");
            anim.SetBool("Pushing", false);
            playerAnim.SetBool("Pushing", false);
            hit.transform.parent = gameObject.transform;
            cam.SetActive(false);
            pushCam.SetActive(true);
            speed = pushPower;
        }
        else
        {
            boxMove = false;
            speed = normSpeed;
            hit.transform.parent = null;
            Debug.DrawRay(trans.position, trans.TransformDirection(Vector3.forward) * distance, Color.blue);
            //Debug.Log("Did not Hit");
            cam.SetActive(true);
            pushCam.SetActive(false);
            anim.SetBool("Pushing", false);
            playerAnim.SetBool("Pushing", false);
        }
    }

    void FixedUpdate()
    {
        float antiBump = .75f;

        float H = Input.GetAxis("Horizontal");
        float V = Input.GetAxis("Vertical");

        float inputModifyFactor = (H != 0.0f && V != 0.0f && limitDiagonalSpeed) ? .7071f : 1.0f;

        if (grounded)
        {
            if(H > 0.05 || H < -.05 || V > 0.05 || V < -.05)
            {
                anim.SetBool("Walk", true);
                playerAnim.SetBool("Walk", true);
            }
            else
            {
                anim.SetBool("Walk", false);
                playerAnim.SetBool("Walk", false);
            }

            if (playerControl)
            {
                moveDir = new Vector3(H * inputModifyFactor, -antiBump, V * inputModifyFactor);
                moveDir = trans.TransformDirection(moveDir) * speed;

                if (!Input.GetButton("Jump"))
                {
                    jumpTimer++;
                }
                else if (jumpTimer >= antiBunnyHop)
                {
                    moveDir.y = jumpSpeed;
                    jumpTimer = 0;
                }
            } else
            {
                moveDir = trans.TransformDirection(moveDir) * 0;
            }
        }
        else
        {
            if (!falling)
            {
                falling = true;
                fallStartLevel = trans.position.y;
            }

            if (airControl && playerControl)
            {
                moveDir.x = H * inputModifyFactor * speed;
                moveDir.z = V * inputModifyFactor * speed;
                moveDir = trans.TransformDirection(moveDir);
            }
        }

        moveDir.y -= grav * Time.deltaTime;

        grounded = (charControl.Move(moveDir * Time.deltaTime) & CollisionFlags.Below) != 0;
        var CharacterRotation = cam.transform.rotation;
        CharacterRotation.x = 0;
        CharacterRotation.z = 0;
        trans.rotation = CharacterRotation;
    }

    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        contactPoint = other.point;
        if (other.gameObject.tag == "goal")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (other.gameObject.tag == "key")
        {
            numKeys = numKeys + 1;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "lock")
        {
            if (numKeys >= 1)
            {
                numKeys = numKeys - 1;
                other.gameObject.tag = "unlocked";
                other.gameObject.GetComponent<LockScript>().Unlock();
            }
        }
    } 
}