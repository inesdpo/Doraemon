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

    int layerMask;
    public AnimationCurve easeCurve;

    private bool justLetGo = true;
    private bool firstTouch = false;

    public TextMeshProUGUI notificationText;
    public GameObject notificationBox;
    public Image imageComponent;
    public Sprite spriteToChange;


    private bool ObjectsUnderneath = false;
    private Vector3 extendsScaled = Vector3.zero;

    void Start()
    {
        InitialPosition = gameObject.transform.parent.localPosition;
        InitialRotation = gameObject.transform.parent.localRotation;
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

                        DraggingObject.localScale = WorldScale / 4;

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

                layerMask = 1 << LayerMask.NameToLayer("Box");

                Ray gridRay = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(gridRay, out hit, 100, layerMask))
                {

                    Pivot.position = hit.point + new Vector3(0, 0.5f, 0);
                    DraggingObject.localScale = WorldScale;

                }
                else { DraggingObject.localScale = WorldScale / 4; }



                layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

                ObjectsUnderneath = false;

                extendsScaled = new Vector3
                    (
                        DraggingObject.GetComponent<BoxCollider>().size.x * DraggingObject.localScale.x * Pivot.localScale.x,
                        DraggingObject.GetComponent<BoxCollider>().size.y * DraggingObject.localScale.y * Pivot.localScale.y,
                        DraggingObject.GetComponent<BoxCollider>().size.z * DraggingObject.localScale.z * Pivot.localScale.z
                    );


                RaycastHit[] AllHits = Physics.BoxCastAll(DraggingObject.position, extendsScaled * 0.5f, new Vector3(0, -1.0f, 0), Pivot.localRotation, 10.0f, layerMask);


                foreach (var objectHit in AllHits)
                {
                    if (objectHit.collider.name != gameObject.name)
                    {
                        ObjectsUnderneath = true;
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



                    if (!ObjectsUnderneath)
                    {

                        IsSnapping = true;


                        Pivot.parent = boxScene.transform;

                        LetGoPosition = Pivot.localPosition;
                        LetGoRotation = Pivot.localRotation;
                        LetGoScale = DraggingObject.localScale;

                        GoalPosition = LetGoPosition - new Vector3(0, 0.4f, 0);
                        GoalScale = LetGoScale;
                        GoalRotation = LetGoRotation;

                        notificationBox.SetActive(false);

                        return;

                    }
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

            justLetGo = true;
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
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (IsDragging)
        {
            if (ObjectsUnderneath) { Gizmos.color = Color.red; }
            else { Gizmos.color = Color.green; }

            Gizmos.DrawWireCube(DraggingObject.position - new Vector3(0, 5.0f, 0), extendsScaled - new Vector3(0, extendsScaled.y, 0) + new Vector3(0, 10.0f, 0));
        }
    }
}