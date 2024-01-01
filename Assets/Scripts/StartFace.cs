using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartFace : MonoBehaviour
{
    public Sprite normalFace;
    public Sprite funnyFace;
    public RuntimeAnimatorController normalFaceController;

    private void ToggleFace()
    {
        if (GetComponent<Image>().sprite == normalFace)
        {
            GetComponent<Image>().sprite = funnyFace;
        }
        else
        {
            GetComponent<Image>().sprite = normalFace;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().runtimeAnimatorController = normalFaceController;
    }

    // Update is called once per frame
    void Update()
    {
        //swap the faces when they are tapped
        if (Input.GetMouseButtonDown(0))
        {
            //see if we're clicking on the face
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(mousePosition))
            {
                ToggleFace();
            }
        }
    }
}
