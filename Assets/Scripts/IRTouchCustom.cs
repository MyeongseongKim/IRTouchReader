using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRTouchCustom : MonoBehaviour
{
    public float width;
    public float height;

    void Awake() {
        transform.localScale = new Vector3(width/100.0f, height/100.0f, 1.0f);
    }

    void Start()
    {
        Debug.Log(transform.localScale);
        
        // transform.localScale.x = width/100.0;
        // transform.localScale.y = height/100.0;
    }

    void Update()
    {
        
    }
}
