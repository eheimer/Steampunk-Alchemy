using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenNoChildren : MonoBehaviour
{

    void Update()
    {
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }
}
