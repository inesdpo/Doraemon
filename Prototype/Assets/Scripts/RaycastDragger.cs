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


    public Vector3 WorldScale;
    public Quaternion WorldRotation;


    private Vector3 InitialPosition = Vector3.zero;
    private Quaternion InitialRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    private Vector3 PivotInitialScale = Vector3.zero;
    private Vector3 ObjectInitialScale = Vector3.zero;

    private float rotationSpeed = 180.0f; // Adjust the rotation speed as desired



    int layerMask;
    public AnimationCurve easeCurve;

    private bool justLetGo = true;
    private bool firstTouch = false;

    public TextMeshProUGUI notificationText;
    public GameObject notificationBox;
    public Image imageComponent;
    public Sprite spriteToChange;

    void Start()
    {
        InitialPosition = gameObject.transform.parent.transform.localPosition;
        InitialRotation = gameObject.transform.parent.transform.localRotation;
        PivotInitialScale = gameObject.transform.parent.localScale;
        ObjectInitialScale = gameObject.transform.localScale;

    }

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

                        notificationText.SetText("While dragging, use two fingers to rotate");
                        imageComponent.sprite = spriteToChange;

                    }
                }

            }

            firstTouch = true;


            if (IsDragging)
            {

                // zero out the rotation to align the object with the box 

                rotationT += 0.1f;
                if (rotationT >= 1) { rotationT = 1.0f; };

                animationTime = easeCurve.Evaluate(rotationT);

                Pivot.transform.localRotation = Quaternion.Lerp(InitialRotation, WorldRotation, animationTime);


                // the object gets dragged around according to the finger's position

                Pivot.transform.position = ray.GetPoint(ray.origin.z * -2.0f);


                // a new reay gets shot, if it hits the grid, the object snaps to that position

                layerMask = 1 << LayerMask.NameToLayer("Grid Plane");

                Ray gridRay = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(gridRay, out hit, 100, layerMask))
                {
                    Pivot.position = hit.transform.position + new Vector3(0, 0.5f, 0);
                    DraggingObject.localScale = WorldScale;
                } else { DraggingObject.localScale = new Vector3(1, 1, 1); }

                // Check if there are two touches
                if (Input.touchCount >= 2)
                {
                    // Get the positions of both touches
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);

                    // Check if both touches are moving
                    if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
                    {
                        // Calculate the previous and current positions of both touches
                        Vector2 prevTouch1Pos = touch1.position - touch1.deltaPosition;
                        Vector2 prevTouch2Pos = touch2.position - touch2.deltaPosition;
                        Vector2 touch1Delta = touch1.position - prevTouch1Pos;
                        Vector2 touch2Delta = touch2.position - prevTouch2Pos;

                        // Calculate the angle between the previous and current positions of both touches
                        float rotationAngle = Vector2.Angle(touch1Delta, touch2Delta);

                        // Cross product to determine the direction of rotation
                        Vector3 cross = Vector3.Cross(touch1Delta, touch2Delta);

                        // Adjust the rotation angle based on the direction of rotation
                        if (cross.z > 0)
                            rotationAngle = -rotationAngle;

                        // Restrict the rotation to the Y-axis
                        rotationAngle *= Mathf.Sign(Vector3.Dot(Vector3.up, cross));

                        // Apply the rotation to the dragged object around the Y-axis
                        DraggingObject.Rotate(Vector3.up, rotationAngle, Space.World);

                    }
                }

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
                    Debug.Log("I'm inside of the box's bounds");

                    layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                    RaycastHit hitObject;

                    Vector3 extendsScaled = new Vector3
                        (
                            Pivot.localScale.x,
                            Pivot.localScale.y,
                            Pivot.localScale.z
                        );


                    if (!Physics.BoxCast(DraggingObject.position, extendsScaled * 0.4f, new Vector3(0, -1, 0), out hitObject, Quaternion.identity, 10, layerMask))
                    {

                        Debug.Log("There are no objects underneath me, I'm snapping to the grid");

                        IsSnapping = true;


                        Pivot.parent = boxScene.transform;

                        LetGoPosition = Pivot.localPosition;
                        LetGoRotation = Pivot.localRotation;
                        LetGoScale = Pivot.localScale;

                        GoalPosition = LetGoPosition - new Vector3(0, 0.4f, 0);
                        GoalScale = LetGoScale;
                        GoalRotation = LetGoRotation;

                        DraggingObject = null;

                        notificationBox.SetActive(false);

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

    }


}