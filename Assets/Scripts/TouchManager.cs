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
    public bool isVisible;
    public bool simulateTouch;
    private int simulateId = 0;

    // Dimensions of ctive area (mm)
    public float width;
    public float height;


    void Start()
    {
        EnhancedTouchSupport.Enable();
        Input.simulateMouseWithTouches = false;

        touchFrame.transform.localScale = new Vector3(width/100.0f, height/100.0f, 1.0f);
        // width = touchFrame.GetComponent<TouchFrameCustom>().width;
        // height = touchFrame.GetComponent<TouchFrameCustom>().height;

        touchPoints = new Dictionary<int, GameObject>();
    }


    void Update()
    {
        if (simulateTouch) {
            // Debug.Log(SimulateTouchCoord());
            if (SimulateTouchCoord(Input.mousePosition).x > 0.0f && SimulateTouchCoord(Input.mousePosition).x < width && 
                SimulateTouchCoord(Input.mousePosition).y > 0.0f && SimulateTouchCoord(Input.mousePosition).y < height) {
                
                if (Input.GetMouseButtonUp(0)) {
                    simulateId++;
                    Debug.Log(simulateId);
                }

                if (Input.GetMouseButtonDown(0)) {
                    if (!touchPoints.ContainsKey(simulateId)) {
                        touchPoints.Add(simulateId, GameObject.Instantiate(prefab, transform));
                        touchPoints[simulateId].name = $"{simulateId}";
                    }
                    Vector2 worldTouchCoord = SimulateTouchCoord(Input.mousePosition);
                    Vector3 worldPosition = new Vector3(0.01f * (worldTouchCoord.x - width/2), 0.0f, 0.01f * (worldTouchCoord.y-height/2));
                    touchPoints[simulateId].transform.localPosition = worldPosition;
                    if (isVisible)  touchPoints[simulateId].SetActive(true);
                }

                if (Input.GetMouseButton(0)) {
                    Vector2 worldTouchCoord = SimulateTouchCoord(Input.mousePosition);
                    Vector3 worldPosition = new Vector3(0.01f * (worldTouchCoord.x - width/2), 0.0f, 0.01f * (worldTouchCoord.y-height/2));
                    touchPoints[simulateId].transform.localPosition = worldPosition;
                }

                touchPoints[simulateId].GetComponent<TouchPointInfo>().id = simulateId;
                touchPoints[simulateId].GetComponent<TouchPointInfo>().phase = "Simulated";
                touchPoints[simulateId].GetComponent<TouchPointInfo>().pos = SimulateTouchCoord(Input.mousePosition); 

                
                if(Input.GetMouseButtonUp(1)) {

                }
            }
        }
        else {
            Debug.Log($"Number of Touches: {Touch.activeTouches.Count}");
            if (Touch.activeFingers.Count > 0 || Touch.activeTouches.Count > 0) {
                for (int i=0; i<Touch.activeFingers.Count; i++) {
                    Touch touch = Touch.activeFingers[i].currentTouch;

                    if (touch.phase == TouchPhase.Began) {
                        if (!touchPoints.ContainsKey(touch.touchId)) {
                            touchPoints.Add(touch.touchId, GameObject.Instantiate(prefab, transform));
                            touchPoints[touch.touchId].name = $"{touch.touchId}";
                        }
                        Vector2 worldTouchCoord = TransformTouchCoord(touch.screenPosition);
                        Vector3 worldPosition = new Vector3(0.01f * (worldTouchCoord.x - width/2), 0.0f, 0.01f * (worldTouchCoord.y-height/2));
                        touchPoints[touch.touchId].transform.localPosition = worldPosition;
                        if (isVisible)  touchPoints[touch.touchId].SetActive(true);
                    }
                    else if (touch.phase == TouchPhase.Moved) {
                        Vector2 worldTouchCoord = TransformTouchCoord(touch.screenPosition);
                        Vector3 worldPosition = new Vector3(0.01f * (worldTouchCoord.x - width/2), 0.0f, 0.01f * (worldTouchCoord.y-height/2));
                        touchPoints[touch.touchId].transform.localPosition = worldPosition;
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                        touchPoints[touch.touchId].SetActive(false);

                        Destroy(touchPoints[touch.touchId]);
                        touchPoints.Remove(touch.touchId);
                    }

                    touchPoints[touch.touchId].GetComponent<TouchPointInfo>().id = touch.touchId;
                    touchPoints[touch.touchId].GetComponent<TouchPointInfo>().phase = touch.phase.ToString();
                    touchPoints[touch.touchId].GetComponent<TouchPointInfo>().pos = TransformTouchCoord(touch.screenPosition);      
                }
            }
        }
        
    }

    
    private Vector2 SimulateTouchCoord(Vector2 pos) {
        Vector2 simulatedCoord = Vector2.zero;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider == touchFrame.GetComponent<MeshCollider>()) {
                Vector2 uv = hit.textureCoord;
                simulatedCoord = new Vector2(width * uv.x, height * uv.y);  //mm
            }
        }
        return simulatedCoord;
    }

    private Vector2 TransformTouchCoord(Vector2 pos) {
        Vector2 worldCoord = new Vector2(pos.x * width / (float)Screen.width, pos.y * height / (float)Screen.height);
        return worldCoord;
    }

}
