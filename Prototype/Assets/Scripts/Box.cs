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

    private bool firstTouch = false;

    //public GameObject newIcon;

    public GameObject plane;

    public GameObject[] draggableObjects;

    //public Animator mAnimator;

    // Start is called before the first frame update
    void Start()
    {
        /*if (homeScreen)
        {
            notificationBox.SetActive(true);
            homeScreen = false;
        }*/

        box1.SetActive(true);
        

       // mAnimator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.touchCount > 0 && firstTouch == false)
        {

            RaycastHit hit;

            Touch touch = Input.GetTouch(0);

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            layerMask = 1 << LayerMask.NameToLayer("Box");

            

            if (Physics.Raycast(ray, out hit, 100, layerMask) && boxClosed)
            {
                //notificationText.SetText("Drag and drop the objects into the box");
                imageComponent.sprite = spriteToChange;
                Debug.Log("Open Box");
                saveButton.SetActive(true);
                boxClosed = false;
                plane.SetActive(true);
                box1.SetActive(false);
                box2.SetActive(true);


                //newIcon.SetActive(false);

                foreach (var obj in draggableObjects)
                {
                    obj.SetActive(true);
                }


            }
            else
            {
                firstTouch = false;
            }


            firstTouch = true;

          
        }

    }
}
