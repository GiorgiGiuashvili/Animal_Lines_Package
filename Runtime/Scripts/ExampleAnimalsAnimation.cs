using UnityEngine;
using Spine.Unity;
using System.Collections;

public class ExampleAnimalsAnimation : MonoBehaviour
{
    public string animationName = "Happy";
    public bool loop = true;
    public GameObject[] targetGameObjects;

    public float delayForFirst = 1.0f;  
    public float delayForSecond = 2.0f;
    public float delayForThird = 3.0f;  

    public void PlayAnimations()
    {
        if (targetGameObjects == null || targetGameObjects.Length == 0)
        {
            SkeletonAnimation[] allAnimations = FindObjectsOfType<SkeletonAnimation>();

            foreach (SkeletonAnimation skeleton in allAnimations)
            {
                StartCoroutine(PlayAnimationWithDelay(skeleton, delayForFirst));
            }
        }
        else
        {
            for (int i = 0; i < targetGameObjects.Length; i++)
            {
                GameObject obj = targetGameObjects[i];
                SkeletonAnimation skeleton = obj.GetComponent<SkeletonAnimation>();

                float delay = i == 0 ? delayForFirst : i == 1 ? delayForSecond : delayForThird;

                if (skeleton != null)
                {
                    StartCoroutine(PlayAnimationWithDelay(skeleton, delay));
                }
            }
        }
    }

    private IEnumerator PlayAnimationWithDelay(SkeletonAnimation skeleton, float delay)
    {
        yield return new WaitForSeconds(delay);

        skeleton.AnimationState.SetAnimation(0, animationName, loop);
    }
}
