using System.Collections;
using System.Collections.Generic;
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
