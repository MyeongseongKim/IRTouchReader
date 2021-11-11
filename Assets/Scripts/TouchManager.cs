using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class TouchManager : TouchSimulation
{

    public GameObject prefab;    
    public Dictionary<int, GameObject> touchPoints;
    public bool isVisible;
    public bool simulateTouch;

    // Start is called before the first frame update
    void Start()
    {
        EnhancedTouchSupport.Enable();
       // if (simulateTouch)  TouchSimulation.Enable();
       // else TouchSimulation.Disable();

        touchPoints = new Dictionary<int, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Number of Touches: {Touch.activeTouches.Count}");
        if (Touch.activeFingers.Count > 0 || Touch.activeTouches.Count > 0) {
            for (int i=0; i<Touch.activeFingers.Count; i++) {
                Touch touch = Touch.activeFingers[i].currentTouch;

                if (touch.phase == TouchPhase.Began) {
                    if (!touchPoints.ContainsKey(touch.touchId)) {
                        touchPoints.Add(touch.touchId, GameObject.Instantiate(prefab, transform));
                        touchPoints[touch.touchId].name = $"{touch.touchId}";
                    }

                    if (isVisible)  touchPoints[touch.touchId].SetActive(true);
                }
                else if (touch.phase == TouchPhase.Moved) {
                    // Debug.Log(touch.screenPosition);
                }
                else if (touch.phase == TouchPhase.Ended) {
                    touchPoints[touch.touchId].SetActive(false);

                    Destroy(touchPoints[touch.touchId]);
                    touchPoints.Remove(touch.touchId);
                }
                else if (touch.phase == TouchPhase.Canceled) {
                    touchPoints[touch.touchId].SetActive(false);

                    Destroy(touchPoints[touch.touchId]);
                    touchPoints.Remove(touch.touchId);
                }

                touchPoints[touch.touchId].GetComponent<TouchPointInfo>().id = touch.touchId;
                touchPoints[touch.touchId].GetComponent<TouchPointInfo>().phase = touch.phase.ToString();
                touchPoints[touch.touchId].GetComponent<TouchPointInfo>().pos = touch.screenPosition;

                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.screenPosition.x, touch.screenPosition.y, Camera.main.transform.position.y));
                Debug.Log(worldPosition);


                touchPoints[touch.touchId].transform.localPosition = worldPosition;
            }
        }
    }


}
