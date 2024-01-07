using System.Collections;
using UnityEngine;

public class Match3Item : MonoBehaviour
{
    public Match3ItemType itemType;
    public int xIndex;
    public int yIndex;
    public bool isMatched = false;
    public bool isMoving = false;
    public Sprite mainImage;
    public Sprite altImage;
    public ParticleSystem matchParticlePrefab;


    public void SetIndices(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void MoveToTarget(Vector3 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
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

    public void YoureFired(bool goQuietly = false)
    {
        if (goQuietly)
        {
            Destroy(gameObject);
            return;
        }
        if (altImage != null)
        {
            GetComponent<SpriteRenderer>().sprite = altImage;
        }
        StartCoroutine(ExplodeCoroutine());
    }

    public void Cower(Vector3 moveDirection)
    {
        if (isMatched) return;
        StartCoroutine(CowerCoroutine(moveDirection));
    }

    private IEnumerator CowerCoroutine(Vector3 moveDirection)
    {
        float duration = 0.2f;
        float scale = 0.75f;
        float moveDistance = 0.25f;
        moveDirection *= moveDistance;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(scale, scale, scale);
        Vector3 startPosition = transform.position;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / duration));
            //move in the direction of moveDirection
            transform.position = Vector3.Lerp(startPosition, startPosition + moveDirection, (elapsedTime / duration));


            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = startScale;
        transform.position = startPosition;
    }


    private IEnumerator ExplodeCoroutine()
    {
        float duration = 0.2f;
        float scale = 1.5f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = new Vector3(scale, scale, scale);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;
        Instantiate(matchParticlePrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

public enum Match3ItemType
{
    Red,
    Blue,
    Purple,
    Green,
    White,
    Yellow,
    Pink
}
