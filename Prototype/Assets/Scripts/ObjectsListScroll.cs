using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsListScroll : MonoBehaviour
{

    private bool firstTouch = false;
    private bool justLetGo = true;

    private Vector3 InitialTouchPosition = Vector3.zero;
    private Vector3 PrevTouchPosition = Vector3.zero;
    private Vector3 TouchPosition = Vector3.zero;

    private float TouchDiff = 0;
    private float easing = 0.05f;

    private float rightLimit = 0;
    private float leftLimit = 0;

    private bool IsScrolling = false;

    private int VerticalScrollConfirmation = 0;

    private bool MayWantToGrab = false;
    private bool HorizontalScroll = false;
    public bool VerticalScroll = false;

    private Transform DraggingObject = null;
    private Transform Pivot = null;

    private float newListPositionX = 0;
    private float GoalListPositionX = 0;

    private Transform ObjectsList = null;


    // Start is called before the first frame update
    void Start()
    {
        ObjectsList = gameObject.transform;
        newListPositionX = ObjectsList.localPosition.x;

        rightLimit = gameObject.transform.position.x - gameObject.GetComponent<BoxCollider>().size.x + 0.36f;
        leftLimit = gameObject.transform.position.x;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount == 1)
        {

            justLetGo = false;

            Touch touch = Input.GetTouch(0);


            //understand if the person is trying to touch the menu

            if (!firstTouch)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit[] hits = Physics.RaycastAll(ray, 100);

                Debug.Log("first touch");


                foreach (var hit in hits)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        InitialTouchPosition = touch.position;
                        TouchPosition = InitialTouchPosition;

                        HorizontalScroll = true;
                        VerticalScroll = false;
                        IsScrolling = false;
                    }
                    else if (hit.collider.gameObject.tag == "Draggable Object")
                    {
                        MayWantToGrab = true;
                        DraggingObject = hit.collider.gameObject.transform;
                        Pivot = DraggingObject.parent;
                    }
                }
            }


            if (touch.phase == TouchPhase.Moved && HorizontalScroll)
            {
                PrevTouchPosition = TouchPosition;
                TouchPosition = touch.position;

                TouchDiff = TouchPosition.x - PrevTouchPosition.x;

                newListPositionX = ObjectsList.localPosition.x + TouchDiff / 2500;

                if (newListPositionX < rightLimit) { newListPositionX = rightLimit; }
                else if (newListPositionX > leftLimit) { newListPositionX = leftLimit; }


                ObjectsList.localPosition = new Vector3(newListPositionX, ObjectsList.localPosition.y, ObjectsList.localPosition.z);

            }


            if (MayWantToGrab)
            {

                Vector3 TouchDiffFromStart = TouchPosition - InitialTouchPosition;

                if (Mathf.Abs(TouchDiffFromStart.y) > Mathf.Abs(TouchDiffFromStart.x))
                {

                    VerticalScrollConfirmation++;

                    if (VerticalScrollConfirmation > 5)
                    {

                        HorizontalScroll = false;
                        MayWantToGrab = false;
                        VerticalScroll = true;

                        DraggingObject.GetComponent<RaycastDragger>().DraggingObject = DraggingObject;
                        DraggingObject.GetComponent<RaycastDragger>().Pivot = Pivot;

                        Debug.Log("Dragging an Object");

                    }
                }


            }


            firstTouch = true;

        }

        else if (Input.touchCount == 0)
        {
            if (!justLetGo)
            {
                IsScrolling = true;

                GoalListPositionX = newListPositionX + TouchDiff / 75;

                if (GoalListPositionX < rightLimit) { GoalListPositionX = rightLimit; }
                else if (GoalListPositionX > leftLimit) { GoalListPositionX = leftLimit; }

                Debug.Log("current position: " + newListPositionX + "; difference: " + TouchDiff + "; goal position: " + GoalListPositionX);

            }

            justLetGo = true;


            firstTouch = false;
            MayWantToGrab = false;
            HorizontalScroll = false;
            VerticalScroll = false;
        }

        if (IsScrolling)
        {
            float currentPos = ObjectsList.localPosition.x;

            if (GoalListPositionX - currentPos < 0.001)
            {
                ObjectsList.localPosition = new Vector3(GoalListPositionX, ObjectsList.localPosition.y, ObjectsList.localPosition.z); IsScrolling = false;
            }

            ObjectsList.localPosition = new Vector3(currentPos + (GoalListPositionX - currentPos) * easing, ObjectsList.localPosition.y, ObjectsList.localPosition.z);

        }

    }
}
