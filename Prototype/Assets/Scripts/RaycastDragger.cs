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
    private float rotationTime = 0.0f;

    public GameObject box;

    private Vector3 objectMin = Vector3.zero;
    private Vector3 objectMax = Vector3.zero;
    private Vector3 boxMin = Vector3.zero;
    private Vector3 boxMax = Vector3.zero;

    private Vector3 GoalPosition = Vector3.zero;
    private Quaternion GoalRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    private Quaternion ZeroRotation = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
    private Vector3 LetGoPosition = Vector3.zero;
    private Quaternion LetGoRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    private Transform DraggingObject = null;
    private Vector3 InitialPosition = Vector3.zero;
    private Quaternion InitialRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    public Camera camera;
    int layerMask;
    public AnimationCurve easeCurve;

    private bool justLetGo = true;

    public TextMeshProUGUI notificationText;
    public GameObject notificationBox;
    public Image imageComponent;
    public Sprite spriteToChange;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = gameObject.transform.parent.transform.position;
        InitialRotation = gameObject.transform.parent.transform.rotation;

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

            layerMask = 1 << LayerMask.NameToLayer("Draggable Objects");

            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    DraggingObject = hit.transform;
                    IsDragging = true;
                    /*notificationText.SetText("Two fingers to rotate");
                    imageComponent.sprite = spriteToChange;*/

                }
            }
            

            if (IsDragging)
            {
                rotationT += 0.25f;
                if (rotationT >= 1) { rotationT = 1.0f;};

                animationTime = easeCurve.Evaluate(rotationT);

                DraggingObject.parent.transform.rotation = Quaternion.Lerp(InitialRotation, ZeroRotation, animationTime);

                DraggingObject.parent.transform.position = ray.GetPoint(ray.origin.z * -0.5f);

                layerMask = 1 << LayerMask.NameToLayer("Grid Plane");

                Ray gridRay = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(gridRay, out hit, 100, layerMask))
                {
                    DraggingObject.parent.transform.position = hit.transform.position + new Vector3(0, 0.5f, 0);
                };
            }

            

        } else
        {
            DraggingObject = null;
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