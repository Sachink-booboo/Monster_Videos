using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBehaviourTimerInterruption : StateMachineBehaviour
{
    [SerializeField] float firstInterruptTime = 8f;
    [SerializeField] Vector2 idleTime = new(5, 10);
    [SerializeField] string animID = "Interrupt";
    [SerializeField] int maxAnimationCount = 0;
    string randomAnimationID => $"{animID}{Random.Range(0, maxAnimationCount)}";
    [SerializeField] float blendTime = .1f;
    float playbackProgress = 0f;
    float? timer = null;
    bool canCrossFade = false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // if (!animator.GetBool(UIAnimatorDefaults.animatorStateUseRandomInterrupts))
        // {
        //     Destroy(this);
        //     return;
        // }

        timer = timer.HasValue ? Random.Range(idleTime.x, idleTime.y) : firstInterruptTime;

        if (canCrossFade)
        {
            animator.CrossFadeInFixedTime(stateInfo.shortNameHash, blendTime, layerIndex, playbackProgress);
            canCrossFade = false;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = Mathf.Infinity;
            var nextAnimation = Animator.StringToHash(randomAnimationID);
            if (animator.HasState(layerIndex, nextAnimation))
            {
                animator.CrossFadeInFixedTime(nextAnimation, blendTime, layerIndex);
                canCrossFade = true;
                playbackProgress = Mathf.Repeat(stateInfo.normalizedTime, 1f);
            }
        }
    }
}
