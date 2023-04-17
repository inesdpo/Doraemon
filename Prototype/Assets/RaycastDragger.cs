using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class RaycastDragger : MonoBehaviour
{
    private bool IsDragging = false;
    private Transform DraggingObject = null;
    public Camera camera;
    int layerMask;
    public List<GameObject> Objects = new List<GameObject>();
    public List<Vector3> ObjectsInitialPosition = new List<Vector3>();
    //public AnimationCurve easeCurve;

    // Start is called before the first frame update
    void Start()
    {

        var objects = GameObject.FindGameObjectsWithTag("Draggable Object");


        foreach (var obj in objects)
        {
            Objects.Add(obj);
            ObjectsInitialPosition.Add(obj.transform.position);
        }


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
                DraggingObject = hit.transform;
                DraggingObject.position = ray.GetPoint(ray.origin.z * -1);
                IsDragging = true;
            } 
        }

        if (Input.GetMouseButton(0))
        {
            if (IsDragging)
            {
                DraggingObject.position = ray.GetPoint(ray.origin.z * -1);


               
            }        


            
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsDragging = false;
            

            //new ray starting from the dragging objects to the plane

            layerMask = 1 << LayerMask.NameToLayer("Grid Plane");

            Ray gridRay = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(gridRay, out hit, 100, layerMask))
            {
                //if it hits one of the squares, it snaps to that position
                DraggingObject.position = hit.transform.position;

            }
            else
            {
                Debug.Log("disappear");
                //DraggingObject.position = Vector3.Lerp(DraggingObject.position, ObjectsInitialPosition[0], easeCurve);
            }

            DraggingObject = null;
        }
    }

}