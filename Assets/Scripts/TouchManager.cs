using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class TouchManager : MonoBehaviour
{
    public GameObject touchFrame;
    public GameObject prefab;    
    public Dictionary<int, GameObject> touchPoints;

    // Dimensions of ctive area (mm)
    public float width;
    public float height;
    private int id;

    


    void Start()
    {
        EnhancedTouchSupport.Enable();
        Input.simulateMouseWithTouches = false;

        touchFrame.transform.localScale = new Vector3(width/1000.0f, height/1000.0f, 1.0f);

        touchPoints = new Dictionary<int, GameObject>();
        id = 0;
    }


    void Update()
    {
        Debug.Log($"Number of Touches: {Touch.activeTouches.Count}");
        if (Touch.activeTouches.Count > 0) {
            foreach(Touch touch in Touch.activeTouches) {
                Debug.Log(touch.phase);

                if (touchPoints.ContainsKey(touch.touchId)) {
                    var script = touchPoints[touch.touchId].GetComponent<TouchPointInfo>();
                    script.id = touch.touchId;
                    script.phase = touch.phase.ToString();
                    script.pos = TransformTouchCoord(touch.screenPosition); 
                } 

                if (touch.phase == TouchPhase.Began) {
                    touchPoints.Add(touch.touchId, GameObject.Instantiate(prefab, transform));
                    touchPoints[touch.touchId].name = $"{touch.touchId}";

                    Vector2 worldTouchCoord = TransformTouchCoord(touch.screenPosition);
                    Vector3 worldPosition = new Vector3(0.001f * (worldTouchCoord.x - width/2), 0.0f, 0.001f * (worldTouchCoord.y-height/2));
                    touchPoints[touch.touchId].transform.localPosition = worldPosition;
                }
                else if (touch.phase == TouchPhase.Moved) {
                    if (touch.delta.magnitude > 10.0f ) {
                        touchPoints[touch.touchId].GetComponent<TouchPointInfo>().duration = 0.0f;
                        touchPoints[touch.touchId].SetActive(false);
                    }

                    Vector2 worldTouchCoord = TransformTouchCoord(touch.screenPosition);
                    Vector3 worldPosition = new Vector3(0.001f * (worldTouchCoord.x - width/2), 0.0f, 0.001f * (worldTouchCoord.y-height/2));
                    touchPoints[touch.touchId].transform.localPosition = worldPosition;
                }
                else if (touch.phase == TouchPhase.Stationary) {
                    touchPoints[touch.touchId].GetComponent<TouchPointInfo>().duration += Time.deltaTime;
                    if (touchPoints[touch.touchId].GetComponent<TouchPointInfo>().duration > touchPoints[touch.touchId].GetComponent<TouchPointInfo>().timeForRecognition) {
                        touchPoints[touch.touchId].SetActive(true);
                        Debug.Log("Activate : " + touch.touchId);
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    touchPoints[touch.touchId].SetActive(false);
                    Destroy(touchPoints[touch.touchId]);
                    touchPoints.Remove(touch.touchId);
                }

                    
            }
        }
        
    }


    private Vector2 TransformTouchCoord(Vector2 pos) {
        Vector2 worldCoord = new Vector2(pos.x * width / (float)Screen.width, pos.y * height / (float)Screen.height);
        return worldCoord;
    }

}
