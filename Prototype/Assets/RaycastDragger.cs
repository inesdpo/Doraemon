using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDragger : MonoBehaviour
{
    private bool IsDragging = false;
    private Transform DraggingObject = null;
    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 20))
            {
                DraggingObject = hit.transform;
                DraggingObject.position = ray.GetPoint(ray.origin.z * - 1);
                IsDragging = true;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (IsDragging)
            {
                DraggingObject.position = ray.GetPoint(ray.origin.z * -1);
                

                //new ray starting from the dragging objects to the plane

                Ray gridRay = new Ray(DraggingObject.transform.position, Input.mousePosition);
                
                if (Physics.Raycast(gridRay, out hit, 20))
                {   
                    //if it hit one of the squares, it snaps to that position
                    DraggingObject.position = hit.transform.position;
                    //hit.transform.position = ray.GetPoint(ray.origin.z * -1); 
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsDragging = false;
        }



        //objectpos = campos + ( rayforward * distancefromscreen )

        // move the object to this position
    }
}