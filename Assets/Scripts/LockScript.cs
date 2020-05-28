using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScript : MonoBehaviour
{
    public float fadeSpeed;
    public LockScript child;

    private Animator anim;
    private Animator childAnim;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        if (child != null)
        {
            childAnim = child.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Unlock()
    {
        if (null != anim)
        {
            AnimationClip unlock = anim.runtimeAnimatorController.animationClips[1];
            anim.Play("Unlocked");
            Destroy(gameObject, unlock.length);
            if (child != null)
            {
                childAnim.Play("Unlocked");
                Destroy(child.gameObject, unlock.length);
            }
        }
    }
}