using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    public PotionType potionType;
    public int xIndex;
    public int yIndex;
    public bool isMatched = false;
    private Vector2 currentPos;
    private Vector2 targetPos;
    public bool isMoving = false;

    public void SetIndices(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void MoveToTarget(Vector2 targetPos)
    {
        StartCoroutine(MoveCoroutine(targetPos));
    }

    private IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        isMoving = true;
        float duration = 0.2f;
        Vector2 startPos = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
    }
}

public enum PotionType
{
    Red,
    Blue,
    Purple,
    Green,
    White
}