using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Box : MonoBehaviour
{
    public Camera camera;
    int layerMask;

   // public bool homeScreen = true;
    public GameObject notificationBox;
    //public TextMeshProUGUI notificationText;
    public GameObject saveButton;
    public bool boxClosed = true;
    public Image imageComponent;
    public Sprite spriteToChange;
    public GameObject box2;
    public GameObject box1;

    public GameObject boxFrame;

    private bool IsOpening;

    public GameObject[] LeftSide;
    public GameObject[] RightSide;

    private Vector3 RotationLeftX = new Vector3(-90, 0, 0);
    private Vector3 RotationRightX = new Vector3(-90, 0, 0);

    private Vector3 StartingRotationLeft;
    private Vector3 StartingRotationRight;

    private Vector3 GoalRotationLeft;
    private Vector3 GoalRotationRight;

    private float t = 0.0f;

    public AnimationCurve easeCurve;


    private bool firstTouch = false;

    //public GameObject newIcon;

    public GameObject plane;

    public GameObject[] draggableObjects;


    //public Animator mAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        box1.SetActive(true);


        foreach (var obj in LeftSide)
        {
            StartingRotationLeft = obj.transform.localEulerAngles;
        }

        foreach (var obj in RightSide)
        {
            StartingRotationRight = obj.transform.localEulerAngles;
        }


        GoalRotationLeft = StartingRotationLeft + RotationLeftX;
        GoalRotationRight = StartingRotationRight + RotationRightX;


        Debug.Log(GoalRotationLeft);
        Debug.Log(GoalRotationRight);


        // mAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        if (!boxFrame.activeSelf && boxClosed) { notificationBox.SetActive(true); }
        if (boxFrame.activeSelf) { saveButton.SetActive(false); }

        if (Input.touchCount > 0 && firstTouch == false)
        {

            RaycastHit hit;

            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            layerMask = 1 << LayerMask.NameToLayer("Box");

            
            //transform.eulerAngles

            //Quaternion.Euler()

            if (Physics.Raycast(ray, out hit, 100, layerMask) && boxClosed)
            {
                //notificationBox.SetActive(true);
                //notificationText.SetText("Drag and drop the objects into the box");
                imageComponent.sprite = spriteToChange;
                Debug.Log("Open Box");
                saveButton.SetActive(true);
                boxClosed = false;
                plane.SetActive(true);
                box1.SetActive(false);
                box2.SetActive(true);

                IsOpening = true;
                

                //newIcon.SetActive(false);

                foreach (var obj in draggableObjects)
                {
                    obj.SetActive(true);
                }

                firstTouch = true;

            }
        }

        else
        {
            firstTouch = false;
        }

        if(IsOpening)
        {

            t += Time.deltaTime / 2;
                        

            float animationProgress = easeCurve.Evaluate(t);

            

            foreach( var obj in LeftSide )
            {
                Vector3 LeftSideRotation = Vector3.Lerp(StartingRotationLeft, GoalRotationLeft, animationProgress);

                obj.transform.localEulerAngles= LeftSideRotation;

            }

            foreach (var obj in RightSide)
            {
                Vector3 RightSideRotation = Vector3.Lerp(StartingRotationRight, GoalRotationRight, animationProgress);

                obj.transform.localEulerAngles = RightSideRotation;

            }

            if (t >= 1) { IsOpening = false; }

        }
             
    }
}
