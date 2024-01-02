using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NamedValue : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text valueText;

    [SerializeField] bool showTitle = true;

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
}
