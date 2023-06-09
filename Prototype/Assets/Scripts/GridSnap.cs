using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnap : MonoBehaviour
{   
    public Vector3 boxPos =  Vector3.zero; 
 
    float width = 0.0f;
    float height = 0.0f;

    Vector3 topLeftCorner = Vector3.zero;

    float gridUnitWidth = 0.0f;
    float gridUnitHeight= 0.0f;

    Vector3 gridUnitCenter = Vector3.zero;

    public GameObject spherePrefab;

        
    // Start is called before the first frame update
    void Start()
    { //Vector3.Lerp(Start position, emdpositio, time (0 to 1);


        BoxCollider boxCollider = GetComponent<BoxCollider>();

        boxPos = transform.position;

        width = boxCollider.size.x;
        height = boxCollider.size.z;
        
        topLeftCorner = new Vector3( boxPos.x - width/2.0f , boxPos.y , boxPos.z - height/2.0f);

        gridUnitWidth = width / 10.0f;
        gridUnitHeight= height / 13.0f;

        gridUnitCenter = new Vector3( topLeftCorner.x + gridUnitWidth/2.0f , topLeftCorner.y , topLeftCorner.z + gridUnitHeight/2.0f );


        for(int x = 0; x < 10; x++) 
        {
            for(int z = 0; z < 13; z++) 
            {
                Vector3 gridSnapPoint = new Vector3(gridUnitCenter.x + x * gridUnitWidth , gridUnitCenter.y , gridUnitCenter.z + z * gridUnitHeight);

                GameObject gridUnit = Instantiate(spherePrefab, gridSnapPoint, new Quaternion());
                gridUnit.transform.localScale = new Vector3(gridUnitWidth * 0.95f, 0.001f, gridUnitHeight * 0.95f);


            }
        }
    }
        
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
