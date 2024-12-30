using UnityEngine;
using Spine.Unity;
using System.Collections;

public class ExampleAnimalsAnimation : MonoBehaviour
{
    public string animationName = "Happy";
    public bool loop = true;
    public SkeletonAnimation[] targetAnimations; // Explicitly assign SkeletonAnimation components here

    public float delayForFirst = 1.0f;
    public float delayForSecond = 2.0f;
    public float delayForThird = 3.0f;

    public void PlayAnimations()
    {
        if (targetAnimations == null || targetAnimations.Length == 0)
        {
            Debug.LogWarning("No SkeletonAnimation targets assigned.");
            return;
        }

        for (int i = 0; i < targetAnimations.Length; i++)
        {
            SkeletonAnimation skeleton = targetAnimations[i];

            if (skeleton != null)
            {
                float delay = i == 0 ? delayForFirst : i == 1 ? delayForSecond : delayForThird;
                StartCoroutine(PlayAnimationWithDelay(skeleton, delay));
            }
            else
            {
                Debug.LogWarning($"SkeletonAnimation target at index {i} is null.");
            }
        }
    }

    private IEnumerator PlayAnimationWithDelay(SkeletonAnimation skeleton, float delay)
    {
        yield return new WaitForSeconds(delay);
        skeleton.AnimationState.SetAnimation(0, animationName, loop);
    }
}
