using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static Unity.VisualScripting.Metadata;

public class FeedbackPopup : MonoBehaviour
{
    //public Image imageComponent;
    public GameObject spriteBad;
    public GameObject spriteGood;
    public GameObject spriteNormal;
    public Transform[] children;

    // Start is called before the first frame update
    void Start()
    {
       
       
    }

    // Update is called once per frame
    void Update()
    {
        //children = gameObject.GetComponentsInChildren<Transform>(); ;


        Transform parentTransform = transform;
        int children = parentTransform.childCount;
        if (children >= 6)
        {
            spriteGood.SetActive(true);
            spriteBad.SetActive(false);
            spriteNormal.SetActive(false);
        }
        else if (children <= 3)
        {
            spriteBad.SetActive(true);
            spriteGood.SetActive(false);
            spriteNormal.SetActive(false);
        }
        else
        {
            spriteNormal.SetActive(true);
            spriteBad.SetActive(false);
            spriteGood.SetActive(false);
        }
    }
}
