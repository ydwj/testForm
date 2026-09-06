using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class AnimationEventHandler : MonoBehaviour
{
    public float fTime;
    public string animClipName;
    public UnityEvent events;

    private Animator anim;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        var clip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(_ => _.name== animClipName);
        if (clip == default)
        {
            return;
        }
        var animEvent = new AnimationEvent();
        animEvent.functionName = "OnEvent";
        animEvent.time = fTime;
        clip.AddEvent(animEvent);
        //Debuger.Log("注册砍树动画事件=================");
        anim.Rebind();
    }

    public void OnEvent()
    {
        events?.Invoke();
    }

    private void OnDestroy()
    {
        var clip = anim.runtimeAnimatorController.animationClips.FirstOrDefault(_ => _.name == animClipName);
        if (clip != default)
            clip.events = null ;
    }
}
