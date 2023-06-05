using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnap : MonoBehaviour
{   
    public Vector3 boxPos =  Vector3.zero; 
 
    float width = 0.0f;
    float height = 0.0f;
    float thickness = 0.0f;

    Vector3 topLeftCorner = Vector3.zero;

    float gridUnitWidth = 0.0f;
    float gridUnitHeight= 0.0f;

    Vector3 gridUnitCenter = Vector3.zero;

    public GameObject GridCube;

    public GameObject Grid;


        
    // Start is called before the first frame update
    void Start()
    { 
        
        boxPos = transform.position;
        Debug.Log(boxPos);

        width = transform.localScale.x;
        height = transform.localScale.z;
        thickness = transform.localScale.y;

        topLeftCorner = new Vector3(boxPos.x - width/2, boxPos.y + thickness/2, boxPos.z - height/2);

        gridUnitWidth = width / 10.0f;
        gridUnitHeight= height / 13.0f;

        gridUnitCenter = new Vector3( topLeftCorner.x + gridUnitWidth/2.0f , topLeftCorner.y , topLeftCorner.z + gridUnitHeight/2.0f );


        for(int x = 0; x < 10; x++) 
        {
            for(int z = 0; z < 13; z++) 
            {
                Vector3 gridSnapPoint = new Vector3(gridUnitCenter.x + x * gridUnitWidth , gridUnitCenter.y , gridUnitCenter.z + z * gridUnitHeight);

                GameObject gridUnit = Instantiate(GridCube, gridSnapPoint, new Quaternion());
                gridUnit.transform.localScale = new Vector3(gridUnitWidth, 0.001f, gridUnitHeight) * 0.99f;
                gridUnit.transform.parent = Grid.transform;


            }
        }
    }
        
   

    // Update is called once per frame
    void Update()
    {
    }
}
