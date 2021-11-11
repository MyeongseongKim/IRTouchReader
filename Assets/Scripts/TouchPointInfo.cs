using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchPointInfo : MonoBehaviour
{
    public int id;
    public string phase;
    public TextMesh touchInfo;

    public Vector2 pos;
    public TextMesh touchPos;
    GameObject touchManager;

    void Start() {
    }

    // Update is called once per frame
    void Update() {
        touchInfo.text = "#" + id + " (" + phase + ")";
        touchPos.text = $"{pos}";
    }
}
