using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveObject : MonoBehaviour
{
    public GameObject[] Objects;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

        foreach (var obj in Objects)
        {
            if (obj.GetComponent<RaycastDragger>().IsDragging)
            {
                foreach(var otherobjects in Objects)
                {
                    if (otherobjects != obj)
                    {
                       //set script to false 
                    }
                }
            }
        }


    }
}
