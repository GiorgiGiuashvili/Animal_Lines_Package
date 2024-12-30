using Spine.Unity;
using System.Collections;
using UnityEngine;

public class AnimalDrag : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private bool isPlaced = false;

    public Transform targetPosition;
    public string correctTag = "";

    private AnimalSpawner _Spawner;
    private bool moving = false;

    public SkeletonAnimation skeletonAnimation;
    private float snapThreshold = 0.1f;
    private float snapSpeed = 3;
    public float animationDelay;
    public int SortingOrder;

    private bool isInsideTriggerArea = false;

    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        transform.localScale = new Vector3(0.20f, 0.20f, 0.20f);
        skeletonAnimation.GetComponent<Renderer>().sortingOrder = SortingOrder;  
    }

    private void Update()
    {
        if (isPlaced) return;

        HandleTouchOrMouseInput();
    }

    private void HandleTouchOrMouseInput()
    {
        if (Input.touchCount > 0)
        {
            HandleTouchInput();
        }
        else
        {
            HandleMouseInput();
        }
    }

    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (IsTouchingObject(touchPosition)) StartDragging(touchPosition);
                break;
            case TouchPhase.Moved:
                if (isDragging) DragObject(touchPosition);
                break;
            case TouchPhase.Ended:
                if (isDragging) StopDragging();
                break;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (IsTouchingObject(mousePosition)) StartDragging(mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            DragObject(mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            StopDragging();
        }
    }

    private bool IsTouchingObject(Vector3 position)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(position);
        return hitCollider != null && hitCollider.gameObject == gameObject;
    }

    private void DragObject(Vector3 position)
    {
        transform.position = new Vector3(position.x + offset.x, position.y + offset.y, transform.position.z);
        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
    }

    private void StopDragging()
    {
        transform.localScale = new Vector3(0.20f, 0.20f, 0.20f);
        isDragging = false;
        CheckDropPosition();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TagValue tagValue = other.GetComponent<TagValue>();
        if (tagValue != null && tagValue.Tag == correctTag)
        {
            isInsideTriggerArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        TagValue tagValue = other.GetComponent<TagValue>();
        if (tagValue != null && tagValue.Tag == correctTag)
        {
            isInsideTriggerArea = false;
        }
    }

    private void CheckDropPosition()
    {
        if (targetPosition != null && isInsideTriggerArea)
        {
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            StartCoroutine(SmoothSnapToTargetPosition(targetPosition.position));
        }
        else
        {
            StartCoroutine(WrongMoveAnimation());
        }
    }

    public void SetSpineAnimation(string animationName, bool loop = false)
    {
        if (skeletonAnimation != null)
        {
            StartCoroutine(PlayAnimationWithDelay(animationName, loop));
        }
    }

    private IEnumerator PlayAnimationWithDelay(string animationName, bool loop)
    {
        yield return new WaitForSeconds(animationDelay);
        skeletonAnimation.state.SetAnimation(0, animationName, loop);
    }

    private IEnumerator WrongMoveAnimation()
    {
        transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        yield return StartCoroutine(RotateObject(4f));
        yield return StartCoroutine(RotateObject(-4f));
        yield return StartCoroutine(RotateObject(0f));
        StartCoroutine(SmoothSnapToTargetPosition(originalPosition));
    }

    private IEnumerator RotateObject(float targetAngle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, targetAngle);
        float time = 0;
        float duration = 0.2f;

        while (time < 1)
        {
            time += Time.deltaTime / duration;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time);
            yield return null;
        }
    }

    private void StartDragging(Vector3 position)
    {
        originalPosition = transform.position;
        offset = transform.position - position;
        isDragging = true;
        SortingOrder += 1;
    }

    private IEnumerator SmoothSnapToTargetPosition(Vector3 targetPos)
    {
        float time = 0;
        Vector3 startPos = transform.position;

        while (time < 1)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, time);
            time += Time.deltaTime * snapSpeed;
            yield return null;
        }

        transform.position = targetPos;

        if (targetPos == originalPosition)
        {
            transform.localScale = new Vector3(0.20f, 0.20f, 0.20f);
        }
        else
        {
            isPlaced = true;
            AnimalSpawner.Instance?.ObjectPlaced();
            transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            SortingOrder -= 1;
        }
    }
}
