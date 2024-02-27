using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;

public class NamedValue : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text valueText;

    [SerializeField] bool showTitle = true;
    [SerializeField] AnimatorController updateAnimation;
    /// <summary>
    /// If true, the animation will be applied to a duplicate gameObject.  An animation handler
    /// will need to be attached to the gameObject to dispose of it after the animation is finished.
    /// If false, the animation will be applied to the valueText and no handler is needed.
    /// </summary>
    [SerializeField] bool animateDuplicate = false;

    private void Start()
    {
        titleText.gameObject.SetActive(showTitle);
    }

    private string _title;
    public string Title
    {
        get { return _title; }
        set
        {
            _title = value;
            titleText.text = value.ToUpper();
        }
    }

    private int _value;
    public int Value
    {
        get { return _value; }
        set
        {
            _value = value;
            valueText.text = value.ToString();
        }
    }
    /// <summary>
    /// Changes the value, triggering any defined animations
    /// </summary>
    /// <param name="value"></param>
    public void Change(int value)
    {
        if (updateAnimation != null)
        {
            TMP_Text text = valueText;
            if (animateDuplicate)
            {
                text = Instantiate(valueText, valueText.transform.parent);
            }
            Animator animation = text.gameObject.GetComponent<Animator>();
            if (animation == null)
            {
                animation = text.gameObject.AddComponent<Animator>();
            }
            animation.runtimeAnimatorController = updateAnimation;
        }
        Value = value;
    }
}
