using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Part : MonoBehaviour
{
    public Match3ItemType itemType;
    [SerializeField] private Sprite[] partImages;
    [SerializeField] private Sprite[] brokenImages;
    [SerializeField] public SpriteRenderer spriteRenderer;
    public bool broken = false;

    public void SetType(Match3ItemType newType, bool isBroken = false)
    {
        itemType = newType;
        spriteRenderer.sprite = GetImage(isBroken);
    }

    protected Sprite GetImage(bool broken = false)
    {
        if ((int)itemType >= partImages.Length)
        {
            Debug.LogError("No image for item type " + itemType);
            return null;
        }
        return broken ? brokenImages[(int)itemType] : partImages[(int)itemType];
    }

    /// <summary>
    /// Initializes the part with a random type and not broken
    /// </summary>
    public void Init() { Init(false); }
    /// <summary>
    /// Initializes the part with a random type and the specified broken state
    /// </summary>
    /// <param name="broken"></param>
    public void Init(bool broken)
    {
        var values = System.Enum.GetValues(typeof(Match3ItemType));
        Match3ItemType randomItem = (Match3ItemType)values.GetValue(Random.Range(0, values.Length));
        Init(randomItem, broken);
    }
    /// <summary>
    /// Initializes the part with the specified type and broken state
    /// </summary>
    /// <param name="type"></param>
    /// <param name="broken"></param>
    public void Init(Match3ItemType type, bool broken) { SetType(type, broken); }



}
