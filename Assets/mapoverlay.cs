using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapoverlay : MonoBehaviour
{
    public float moveSpeed = 100f; // pixels per second
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 position = rectTransform.anchoredPosition;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if(position.x <= 215)
              position.x += 0.3f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (position.x >= 23)
                position.x -= 0.3f;
        }

        rectTransform.anchoredPosition = position;
    }
}
