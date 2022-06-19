using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAnim : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        //refactor. not working
        //AnimationClip clip = GetComponent<Animator>().runtimeAnimatorController.animationClips[0];
        //AnimationEvent evt = new AnimationEvent();
        
        //evt.functionName = "OnExplosionAnimEnd";
        //clip.AddEvent(evt);
    }

    // Update is called once per frame
    void Update()
    {
        if (!AnimatorIsPlaying())
        {
            Destroy(gameObject);
        }
    }

    bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length >
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void OnExplosionAnimEnd()
    {
        Destroy(gameObject);
    }
}
