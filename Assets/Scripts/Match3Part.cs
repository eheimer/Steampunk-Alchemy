using System.Collections;
using UnityEngine;

public class Match3Part : Part
{
    public int xIndex;
    public int yIndex;
    public bool isMatched = false;
    public bool isMoving = false;
    public AudioClip moveSound;

    //public ParticleSystem matchParticlePrefab;

    public void SetIndices(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public IEnumerator MoveToTarget(Vector3 targetPos)
    {
        yield return StartCoroutine(MoveCoroutine(targetPos));
    }

    private IEnumerator MoveCoroutine(Vector3 targetPos)
    {
        isMoving = true;
        float duration = 0.2f;
        Vector3 startPos = transform.position;
        //make sure the target position is in the same plane as the start position
        targetPos.z = startPos.z;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }

    public IEnumerator YoureFired()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(2, 6, transform.position.z);
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float duration = .5f; // Adjust this value to control the speed of the movement and scaling
        float elapsedTime = 0f;

        // Instantly scale up
        transform.localScale = startScale * 1.2f;

        // Wait before moving the parts
        yield return new WaitForSeconds(.25f);

        GameManager.instance.PlaySoundEffect(moveSound);
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // Move along a quadratic bezier curve for a smooth curved motion
            Vector3 controlPoint = startPosition + (targetPosition - startPosition) / 2 + Vector3.up;
            transform.position = (1 - t) * (1 - t) * startPosition + 2 * (1 - t) * t * controlPoint + t * t * targetPosition;

            // Gradually shrink to nothing
            transform.localScale = Vector3.Lerp(startScale * 1.2f, targetScale, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.localScale = targetScale;

        Destroy(gameObject);
    }
}


