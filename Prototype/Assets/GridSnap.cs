using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSnap : MonoBehaviour
{   
    //[SerialiedField] public GameObject object ;
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
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();


        boxPos = transform.position;

        width = boxCollider.size.x;
        height = boxCollider.size.z;
        
        topLeftCorner = new Vector3( boxPos.x - width/2.0f , boxPos.y , boxPos.z - height/2.0f);

        


        gridUnitWidth = width / 5.0f;
        gridUnitHeight= height / 8.0f;

        gridUnitCenter = new Vector3( topLeftCorner.x + gridUnitWidth/2.0f , topLeftCorner.y , topLeftCorner.z + gridUnitHeight/2.0f );

        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.position = gridUnitCenter;

        for(int x = 0; x < 5; x++) 
        {
            for(int z = 0; z < 8; z++) 
            {
                Vector3 gridSnapPoint = new Vector3(gridUnitCenter.x + x * gridUnitWidth , gridUnitCenter.y , gridUnitCenter.z + z * gridUnitHeight);


                GameObject mySphere = Instantiate(spherePrefab, gridSnapPoint, new Quaternion());
                mySphere.transform.position = gridSnapPoint;
                //Debug.Log(mySphere.transform);
                //mySphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
        }
    }
        
   

    // Update is called once per frame
    void Update()
    {
        
    }
}
