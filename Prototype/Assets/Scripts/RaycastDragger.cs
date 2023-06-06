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
    private bool RotationState = false;
    private float t = 0.0f;
    private float rotationT = 0.0f;
    private float animationTime = 0.0f;

    public GameObject box;
    public GameObject ObjectsList;
    public Transform DraggingObject = null;
    public Transform Pivot = null;
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

    int layerMask;
    public AnimationCurve easeCurve;

    private bool justLetGo = true;
    private bool firstTouch = false;

    //public TextMeshProUGUI notificationText;
    public GameObject notificationBox;
    public Image imageComponent;
    public Sprite spriteToChange;
    public Button SaveButton;
    public Button RotateButton;

    private bool rayHitsBox = false;
    private bool insideBounds = false;
    private bool canPlaceObject;
    private Vector3 extendsScaled = Vector3.zero;
    public GameObject BoxCast;

    public Material TransparentRed;
    public Material TransparentGreen;

    void Start()
    {
        InitialPosition = gameObject.transform.parent.localPosition;
        InitialRotation = gameObject.transform.parent.localRotation;
        PivotInitialScale = gameObject.transform.parent.localScale;
        ObjectInitialScale = gameObject.transform.localScale;


        RotateButton.onClick.AddListener(rotateObject);
    }

    void Update()
    {

        if (Input.touchCount > 0 && !RotationState)
        {

            justLetGo = false;

            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;


            if (!firstTouch)
            {

                layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                if (Physics.Raycast(ray, out hit, 100, layerMask))
                {

                    if (hit.collider.gameObject == gameObject)
                    {
                        DraggingObject = hit.transform;
                        Pivot = DraggingObject.parent;


                        IsDragging = true;

                        notificationBox.SetActive(false);

                    }
                }

            }


            if (IsDragging && ObjectsList.GetComponent<ObjectsListScroll>().VerticalScroll)
            {


                Pivot.parent = boxScene.transform;

                DraggingObject.localScale = WorldScale / 4;

                // zero out the rotation to align the object with the box 

                rotationT += 0.1f;
                if (rotationT >= 1) { rotationT = 1.0f; };

                animationTime = easeCurve.Evaluate(rotationT);

                Pivot.transform.localRotation = Quaternion.Lerp(InitialRotation, WorldRotation, animationTime);


                // the object gets dragged around according to the finger's position

                Pivot.transform.position = ray.GetPoint(ray.origin.z * -2.0f);


                // a new ray gets shot, if it hits the grid, the object snaps to that position

                layerMask = 1 << LayerMask.NameToLayer("Box");

                Ray gridRay = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(gridRay, out hit, 100, layerMask))
                {
                    if (!BoxCast.activeSelf) { BoxCast.SetActive(true); }
                    Pivot.position = hit.point + new Vector3(0, 0.5f, 0);
                    DraggingObject.localScale = WorldScale;
                    rayHitsBox = true;

                    Debug.Log("I'm hitting the box with the ray");

                }
                else { DraggingObject.localScale = WorldScale / 4; if (BoxCast.activeSelf) { BoxCast.SetActive(false); rayHitsBox = false; } }

            }


        }
        else
        {

            firstTouch = false;

            if (justLetGo == false && IsDragging == true)
            {
                IsDragging = false;

                if (insideBounds && rayHitsBox) { RotationState = true; rayHitsBox = false; }
                else { placeObject(); }


            }

            justLetGo = true;
        }


        if (RotationState)
        {
            if (SaveButton.gameObject.activeSelf)
            {
                SaveButton.gameObject.SetActive(false);
                RotateButton.gameObject.SetActive(true);
                ObjectsList.SetActive(false);

            }

            if (Input.touchCount > 0)
            {

                if (!firstTouch)
                {

                    Touch touch1 = Input.GetTouch(0);

                    Ray confirmationRay = Camera.main.ScreenPointToRay(touch1.position);

                    RaycastHit confirmationHit;

                    if (Physics.Raycast(confirmationRay, out confirmationHit, 100) && confirmationHit.collider.name == DraggingObject.name)
                    {
                        placeObject();
                    }
                }
            }


        }

        //check if the object can be placed

        if (DraggingObject && rayHitsBox)
        {

            //Check if the object is inside the bounds

            canPlaceObject = true;
            BoxCast.GetComponent<Renderer>().material = TransparentGreen;

            objectMin = DraggingObject.gameObject.GetComponent<BoxCollider>().bounds.min;
            objectMax = DraggingObject.gameObject.GetComponent<BoxCollider>().bounds.max;
            boxMin = box.GetComponent<BoxCollider>().bounds.min;
            boxMax = box.GetComponent<BoxCollider>().bounds.max;


            if (objectMin.x > boxMin.x && objectMin.z > boxMin.z && objectMax.x < boxMax.x && objectMax.z < boxMax.z)
            {

                insideBounds = true;

                Debug.Log("I'm inside of the box's bounds");

                layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                extendsScaled = new Vector3
                    (
                        DraggingObject.GetComponent<BoxCollider>().size.x * DraggingObject.localScale.x * Pivot.localScale.x,
                        DraggingObject.GetComponent<BoxCollider>().size.y * DraggingObject.localScale.y * Pivot.localScale.y,
                        DraggingObject.GetComponent<BoxCollider>().size.z * DraggingObject.localScale.z * Pivot.localScale.z
                    );



                //Check if the object has other objects underneath

                RaycastHit[] AllHits = Physics.BoxCastAll(DraggingObject.position, extendsScaled * 0.5f, new Vector3(0, -1.0f, 0), Pivot.localRotation, 10.0f, layerMask);


                foreach (var objectHit in AllHits)
                {
                    if (objectHit.collider.name != gameObject.name)
                    {
                        canPlaceObject = false;
                        BoxCast.GetComponent<Renderer>().material = TransparentRed;

                        Debug.Log("I have something underneath");
                    }
                }

            }
            else
            {
                canPlaceObject = false;
                BoxCast.GetComponent<Renderer>().material = TransparentRed;
                insideBounds = false;

                //Debug.Log("I'm outside of the box");
            }
        }


        if (IsSnapping)
        {
            t += 0.1f;

            animationTime = easeCurve.Evaluate(t);

            Pivot.localPosition = Vector3.Lerp(LetGoPosition, GoalPosition, animationTime);
            Pivot.localRotation = Quaternion.Lerp(LetGoRotation, GoalRotation, animationTime);
            DraggingObject.localScale = Vector3.Lerp(LetGoScale, GoalScale, animationTime);

            if (t >= 1)
            {
                t = 0;
                IsSnapping = false;
                DraggingObject = null;
                Pivot = null;
            }
        }


        //sets first touch

        if (Input.touchCount > 0)
        {

            firstTouch = true;

        }

    }


    //rotate the object with the rotate button
    void rotateObject()
    {
        if (Pivot)
        {

            Pivot.Rotate(Vector3.up, 90.0f, Space.World);

        }

    }


    //place object inside of the box or back to the original position
    void placeObject()
    {
        BoxCast.SetActive(false);
        RotationState = false;

        SaveButton.gameObject.SetActive(true);
        RotateButton.gameObject.SetActive(false);
        ObjectsList.SetActive(true);

        if (canPlaceObject)
        {

            IsSnapping = true;


            Pivot.parent = boxScene.transform;

            LetGoPosition = Pivot.localPosition;
            LetGoRotation = Pivot.localRotation;
            LetGoScale = DraggingObject.localScale;

            GoalPosition = LetGoPosition - new Vector3(0, 0.4f, 0);
            GoalScale = LetGoScale;
            GoalRotation = LetGoRotation;


            return;

        }

        Pivot.parent = ObjectsList.transform;

        IsSnapping = true;

        LetGoPosition = Pivot.localPosition;
        LetGoRotation = Pivot.localRotation;
        LetGoScale = DraggingObject.localScale;

        GoalPosition = InitialPosition;
        GoalScale = ObjectInitialScale;
        GoalRotation = InitialRotation;
    }
}