using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RaycastDragger : MonoBehaviour
{
    private bool IsDragging = false;
    private bool IsSnapping = false;
    private float t = 0.0f;
    private float animationTime = 0.0f;

    public GameObject boxPlane; 

    private Vector3 objectMin = Vector3.zero;
    private Vector3 objectMax = Vector3.zero;
    private Vector3 boxMin = Vector3.zero;
    private Vector3 boxMax = Vector3.zero;

    private Vector3 GoalPosition = Vector3.zero;
    private Vector3 LetGoPosition = Vector3.zero;
    private Transform DraggingObject = null;
    private Vector3 InitialPosition = Vector3.zero;
    public Camera camera;
    int layerMask;
    public AnimationCurve easeCurve;

    public TextMeshProUGUI notificationText;
    public GameObject notificationBox;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = gameObject.transform.parent.transform.position;
        boxPlane = GameObject.FindGameObjectsWithTag("Box")[0];

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
                    IsDragging = true;
                    notificationText.SetText("Two fingers to rotate");
                };
            }

            
        }

        if (IsDragging)
        {
            DraggingObject.parent.transform.position = ray.GetPoint(ray.origin.z * - 0.3f);

            layerMask = 1 << LayerMask.NameToLayer("Grid Plane");

            Ray gridRay = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(gridRay, out hit, 100, layerMask))
            {   
                DraggingObject.parent.transform.position = hit.transform.position + new Vector3(0, 0.02f, 0);
            };
        }

        if (IsSnapping)
        {
            t += 0.25f;

            animationTime = easeCurve.Evaluate(t);

            DraggingObject.parent.transform.position = Vector3.Lerp(LetGoPosition, GoalPosition, animationTime);

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

                objectMin = gameObject.GetComponent<BoxCollider>().bounds.min;
                objectMax = gameObject.GetComponent<BoxCollider>().bounds.max;
                boxMin = boxPlane.GetComponent<BoxCollider>().bounds.min;
                boxMax = boxPlane.GetComponent<BoxCollider>().bounds.max;


                if (objectMin.x > boxMin.x && objectMin.z > boxMin.z && objectMax.x < boxMax.x && objectMax.z < boxMax.z)
                {
                    layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                    Ray checkSpaceRayMin = new Ray((objectMin + new Vector3(0.1f, 0, 0.1f)), (objectMin + new Vector3(0.1f, -2, 0.1f)));
                    Ray checkSpaceRayMax = new Ray((objectMax - new Vector3(0.1f, 0, 0.1f)), (objectMin - new Vector3(0.1f, 2, 0.1f)));

                    Debug.DrawRay((objectMin + new Vector3(0.1f, 0, 0.1f)), (objectMin + new Vector3(0.1f, -2, 0.1f)), Color.green);
                    Debug.DrawRay((objectMax - new Vector3(0.1f, 0, 0.1f)), (objectMin - new Vector3(0.1f, 2, 0.1f)), Color.green);

                    //if (Physics.Raycast(checkSpaceRayMin, 100, layerMask) || Physics.Raycast(checkSpaceRayMax, 100, layerMask)) {
                    //    return; 
                    //};


                    //Physics.BoxCast()

                    //if it hits one of the squares, it snaps to that position
                    IsSnapping = true;

                    LetGoPosition = DraggingObject.position;
                    GoalPosition = hit.transform.position;

                    notificationBox.SetActive(false);
                    return;
                }


            }

                //DraggingObject.gameObject.SetActive(false);
                IsSnapping = true;
                LetGoPosition = DraggingObject.position;
                GoalPosition = InitialPosition;

                //notificationText.SetText("Drag object with finger");
            
        }
    }

}