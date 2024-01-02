using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NamedValue : MonoBehaviour
{
    [SerializeField] string title;
    [SerializeField] int value;

    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text valueText;

    [SerializeField] bool showTitle = true;

    private void Update()
    {
        titleText.enabled = showTitle;
        if (showTitle)
        {
            titleText.text = title.ToUpper();
        }
        valueText.text = value.ToString();
    }

    public void SetTitle(string title)
    {
        this.title = title;
        titleText.text = title;
    }

    public void SetValue(int value)
    {
        this.value = value;
        valueText.text = value.ToString();
    }
}
