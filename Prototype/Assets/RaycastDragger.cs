using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class RaycastDragger : MonoBehaviour
{
    private bool IsDragging = false;
    private bool IsSnapping = false;
    private float t = 0.0f;
    private Vector3 GoalPosition = Vector3.zero;
    private Vector3 LetGoPosition = Vector3.zero;
    private Transform DraggingObject = null;
    private Vector3 InitialPosition = Vector3.zero;
    public Camera camera;
    int layerMask;
    public AnimationCurve easeCurve;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = gameObject.transform.position; 

    }

    // Update is called once per frame
    void Update()
    {
        

        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if(hit.collider.gameObject == gameObject)
                {
                    DraggingObject = hit.transform;
                    DraggingObject.position = ray.GetPoint(ray.origin.z * -1);
                    IsDragging = true;
                }
            } 
        }

        
        if (IsDragging)
        {
            DraggingObject.position = ray.GetPoint(ray.origin.z * -1);
        }

        if (IsSnapping)
        {
            t += 0.34f;

            DraggingObject.position = Vector3.Lerp(LetGoPosition, GoalPosition, t);

            if (t >= 1)
            {
                t = 0;
                IsSnapping = false;
                DraggingObject = null;
            }

            
        }


        if (Input.GetMouseButtonUp(0) && IsDragging)
        {
            IsDragging = false;
            

            //new ray starting from the dragging objects to the plane

            layerMask = 1 << LayerMask.NameToLayer("Grid Plane");

            Ray gridRay = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(gridRay, out hit, 100, layerMask))
            {
                //if it hits one of the squares, it snaps to that position
                IsSnapping = true;

                LetGoPosition = DraggingObject.position;
                GoalPosition = hit.transform.position;
                
                 
            }
            else
            {
                //DraggingObject.gameObject.SetActive(false);
                IsSnapping = true;
                LetGoPosition = DraggingObject.position;
                GoalPosition = InitialPosition;
            }
        }
    }

}