using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Part : MonoBehaviour
{
    public Match3Item item;
    private Sprite[] partImages;
    private Sprite[] brokenImages;
    [SerializeField] public SpriteRenderer spriteRenderer;
    public bool broken = false;

    private void Awake()
    {
        partImages = Resources.LoadAll<Sprite>("Sprites/PartImages");
        brokenImages = Resources.LoadAll<Sprite>("Sprites/BrokenPartImages");
    }

    public void SetType(Match3Item itemType)
    {
        this.item = itemType;
        spriteRenderer.sprite = GetImage(itemType.Broken);
    }

    protected Sprite GetImage(bool broken = false)
    {
        if ((int)item.ItemType >= partImages.Length)
        {
            Debug.LogError("No image for item type " + item);
            return null;
        }
        return broken ? brokenImages[(int)item.ItemType] : partImages[(int)item.ItemType];
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
        Init(new Match3Item(randomItem, broken));
    }
    /// <summary>
    /// Initializes the part with the specified type and broken state
    /// </summary>
    /// <param name="type"></param>
    /// <param name="broken"></param>
    public virtual void Init(Match3Item itemType) { SetType(itemType); }
}
