using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public class RaycastDragger : MonoBehaviour
{
    private bool IsDragging = false;
    private bool IsSnapping = false;
    private float t = 0.0f;
    private float rotationT = 0.0f;
    private float animationTime = 0.0f;

    public GameObject box;
    public GameObject ObjectsList;
    private Transform DraggingObject = null;
    private Transform Pivot = null;
    public Camera camera;

    private Vector3 objectMin = Vector3.zero;
    private Vector3 objectMax = Vector3.zero;
    private Vector3 boxMin = Vector3.zero;
    private Vector3 boxMax = Vector3.zero;

    public GameObject boxScene = null;

    private Vector3 GoalPosition = Vector3.zero;
    private Quaternion GoalRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    private Vector3 GoalScale = Vector3.zero;
    
    private Vector3 LetGoPosition = Vector3.zero;
    private Quaternion LetGoRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    private Vector3 LetGoScale = Vector3.zero;

    private Quaternion ZeroRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

 

    private Vector3 InitialPosition = Vector3.zero;
    private Quaternion InitialRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    private Vector3 PivotInitialScale = Vector3.zero;
    private Vector3 ObjectInitialScale = Vector3.zero;
    

  
    int layerMask;
    public AnimationCurve easeCurve;

    private bool justLetGo = true;
    private bool firstTouch = false;

    public TextMeshProUGUI notificationText;
    public GameObject notificationBox;
    public Image imageComponent;
    public Sprite spriteToChange;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = gameObject.transform.parent.transform.localPosition;
        InitialRotation = gameObject.transform.parent.transform.localRotation;
        PivotInitialScale = gameObject.transform.parent.localScale;
        ObjectInitialScale = gameObject.transform.localScale;

        Debug.Log(InitialPosition);
        Debug.Log(InitialRotation);
        Debug.Log(PivotInitialScale);
        Debug.Log(ObjectInitialScale);

    }

    // Update is called once per frame
    void Update()
    {

        

        if (Input.touchCount > 0)
        {

            justLetGo = false;

            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;


            if (firstTouch == false)
            {

                layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                if (Physics.Raycast(ray, out hit, 100, layerMask))
                {

                    if (hit.collider.gameObject == gameObject)
                    {
                        DraggingObject = hit.transform;
                        Pivot = DraggingObject.parent;

                        Pivot.parent = boxScene.transform;
                        Pivot.localScale = PivotInitialScale;
                        DraggingObject.localScale = ObjectInitialScale;

                        IsDragging = true;

                        /*notificationText.SetText("Two fingers to rotate");
                        imageComponent.sprite = spriteToChange;*/

                    }
                }

            }

            firstTouch = true;


            if (IsDragging)
            {

                // zero out the rotation to align the object with the box 

                rotationT += 0.25f;
                if (rotationT >= 1) { rotationT = 1.0f; };

                animationTime = easeCurve.Evaluate(rotationT);

                Pivot.transform.localRotation = Quaternion.Lerp(InitialRotation, ZeroRotation, animationTime);


                // the object gets dragged around according to the finger's position

                Pivot.transform.position = ray.GetPoint(ray.origin.z * -2.0f);


                // a new reay gets shot, if it hits the grid, the object snaps to that position

                layerMask = 1 << LayerMask.NameToLayer("Grid Plane");

                Ray gridRay = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(gridRay, out hit, 100, layerMask))
                {
                    Pivot.position = hit.transform.position + new Vector3(0, 0.5f, 0);
                };
            }



        }
        else
        {
            
            firstTouch = false;

            if (justLetGo == false && IsDragging == true)
            {
                IsDragging = false;

                objectMin = DraggingObject.gameObject.GetComponent<BoxCollider>().bounds.min;
                objectMax = DraggingObject.gameObject.GetComponent<BoxCollider>().bounds.max;
                boxMin = box.GetComponent<BoxCollider>().bounds.min;
                boxMax = box.GetComponent<BoxCollider>().bounds.max;

                if (objectMin.x > boxMin.x && objectMin.z > boxMin.z && objectMax.x < boxMax.x && objectMax.z < boxMax.z)
                {
                    Debug.Log("I'm inside of the bounds");

                    layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                    RaycastHit hitObject;

                    Vector3 extendsScaled = new Vector3
                        (
                            Pivot.localScale.x,
                            Pivot.localScale.y,
                            Pivot.localScale.z
                        );

                    Debug.Log(extendsScaled);


                    if (!Physics.BoxCast(DraggingObject.position, extendsScaled * 0.4f, new Vector3(0, -1, 0), out hitObject, Quaternion.identity, 10, layerMask))
                    {

                        Debug.Log("I didn't hit any object");

                        IsSnapping = true;


                        Pivot.parent = boxScene.transform;

                        LetGoPosition = Pivot.localPosition;
                        LetGoRotation = Pivot.localRotation;
                        LetGoScale = Pivot.localScale;

                        GoalPosition = LetGoPosition - new Vector3(0, 0.4f, 0);
                        GoalScale = LetGoScale;
                        GoalRotation = LetGoRotation;

                        DraggingObject = null;

                        return;

                    }
                }

                Debug.Log("snapping back to my orginal position");


                Pivot.parent = ObjectsList.transform;


                IsSnapping = true;

                LetGoPosition = Pivot.localPosition;
                LetGoRotation = Pivot.localRotation;
                LetGoScale = Pivot.localScale;

                GoalPosition = InitialPosition;
                GoalScale = PivotInitialScale;
                GoalRotation = InitialRotation;

            }

            justLetGo = true;
        }

        if (IsSnapping)
        {
            t += 0.1f;

            animationTime = easeCurve.Evaluate(t);

            Pivot.localPosition = Vector3.Lerp(LetGoPosition, GoalPosition, animationTime);
            Pivot.localRotation = Quaternion.Lerp(LetGoRotation, GoalRotation, animationTime);
            Pivot.localScale = Vector3.Lerp(LetGoScale, GoalScale, animationTime);

            if (t >= 1)
            {   
                t = 0;
                IsSnapping = false;
                DraggingObject = null;

            }
        }



        


        /*else
        {

            

            if (justLetGo == false)
            {
                justLetGo = true;

                Debug.Log("touch ended");

                //function that runs once when I let go

                IsDragging = false;
                rotationT = 0.0f;

                objectMin = gameObject.GetComponent<BoxCollider>().bounds.min;
                objectMax = gameObject.GetComponent<BoxCollider>().bounds.max;
                boxMin = box.GetComponent<BoxCollider>().bounds.min;
                boxMax = box.GetComponent<BoxCollider>().bounds.max;


                if (objectMin.x > boxMin.x && objectMin.z > boxMin.z && objectMax.x < boxMax.x && objectMax.z < boxMax.z)
                {
                    layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                    Debug.Log("I'm inside the box bouds!");

                    RaycastHit hitObject;

                    Vector3 extendsScaled = new Vector3
                        (
                            DraggingObject.GetComponent<BoxCollider>().size.x * DraggingObject.localScale.x,
                            DraggingObject.GetComponent<BoxCollider>().size.y * DraggingObject.localScale.y,
                            DraggingObject.GetComponent<BoxCollider>().size.z * DraggingObject.localScale.z
                        );



                    if (!Physics.BoxCast(DraggingObject.position, extendsScaled * 0.5f, new Vector3(0, -1, 0), out hitObject, Quaternion.identity, 10, layerMask))
                    {
                        Debug.Log("There is no object underneath me");

                        IsSnapping = true;

                        LetGoPosition = DraggingObject.position;
                        LetGoRotation = DraggingObject.rotation;
                        //GoalPosition = hit.transform.position;
                        GoalRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

                        DraggingObject.parent.parent = box.transform;

                        notificationBox.SetActive(false);
                        return;

                    };

                }

                //DraggingObject.gameObject.SetActive(false);
                IsSnapping = true;
                LetGoPosition = DraggingObject.position;
                LetGoRotation = DraggingObject.rotation;
                GoalPosition = InitialPosition;
                GoalRotation = InitialRotation;

                //DraggingObject.parent.parent = GameObject.Find("DraggableObjects").transform;

                Debug.Log("I'm snapping back to my original position");

            }

        }

        if (IsSnapping)
        {
            t += 0.25f;

            Debug.Log("I'm snapping");

            animationTime = easeCurve.Evaluate(t);

            DraggingObject.parent.transform.position = Vector3.Lerp(LetGoPosition, GoalPosition, animationTime);
            DraggingObject.parent.transform.rotation = Quaternion.Lerp(LetGoRotation, GoalRotation, animationTime);

            if (t >= 1)
            {
                t = 0;
                IsSnapping = false;
                DraggingObject = null;
            }

        } */

    }


}