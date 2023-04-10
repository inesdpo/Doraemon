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
    public List<Vector3> ObjectsInitialPosition = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {

        var objects = GameObject.FindGameObjectsWithTag("Draggable Object");

        Debug.Log(objects[0].transform.position);

        foreach (var obj in objects)
        {
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


                //new ray starting from the dragging objects to the plane

                layerMask = 1 << LayerMask.NameToLayer("Grid Plane");

                Ray gridRay = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(gridRay, out hit, 100, layerMask))
                {
                    //if it hits one of the squares, it snaps to that position
                    DraggingObject.position = hit.transform.position;

                } else
                {
                    
                }
            }        


            //objectpos = campos + ( rayforward * distancefromscreen )

            // move the object to this position
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsDragging = false;
            //DraggingObject = null;
        }
    }

}