using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // whether the space can be filled with potions or not
    public bool isUsable = true;
    public GameObject potion;

    public Node(bool _isUsable, GameObject _potion)
    {
        isUsable = _isUsable;
        potion = _potion;
    }
}
