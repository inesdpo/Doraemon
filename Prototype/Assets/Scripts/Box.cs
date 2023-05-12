using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Box : MonoBehaviour
{
    public Camera camera;
    int layerMask;

    public GameObject notificationBox;
    public TextMeshProUGUI notificationText;
    public GameObject buttons;
    public bool boxClosed = true;
    public Image imageComponent;
    public Sprite spriteToChange;

    //public GameObject newIcon;

    public GameObject plane;

    public GameObject[] draggableObjects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);


        if (boxClosed) {notificationBox.SetActive(true);}


        if (Input.GetMouseButtonDown(0))
        {
            layerMask = 1 << LayerMask.NameToLayer("Box");

            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if (hit.collider.gameObject == gameObject && boxClosed)
                {
                    notificationText.SetText("Drag and drop the objects into the box");
                    imageComponent.sprite = spriteToChange;
                    Debug.Log("Open Box");
                    buttons.SetActive(true);
                    boxClosed = false;
                    plane.SetActive(true);
                    //newIcon.SetActive(false);


                    foreach (var obj in draggableObjects)
                    {
                        obj.SetActive(true);
                    }
                }
                
            }
           
        }

    }
}
