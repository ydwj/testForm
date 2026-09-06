using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FancyScrollView;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class UISlotAnimFancy : FancyCell<UIItemDataFancy>
{
    Animator animator = default;

    static class AnimatorHash
    {
        public static readonly int Scroll = Animator.StringToHash("scroll");
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void UpdateContent(UIItemDataFancy itemData)
    {
    }

    public override void UpdatePosition(float position)
    {
        currentPosition = position;

        if (animator.isActiveAndEnabled)
        {
            animator.Play(AnimatorHash.Scroll, -1, position);
        }

        animator.speed = 0;
    }

    float currentPosition = 0;

    void OnEnable() => UpdatePosition(currentPosition);
}