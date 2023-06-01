using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePanel : MonoBehaviour
{
    /*[SerializeField] private GameObject objectToDeactivate1;
    [SerializeField] private GameObject objectToDeactivate2;
    [SerializeField] private GameObject objectToActivate1;
    [SerializeField] private GameObject objectToActivate2;*/

    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;



    public void TransitionToNextPanel()
    {

        foreach (var obj in objectsToActivate)
        {
            obj.SetActive(true);
        }

        foreach (var obj in objectsToDeactivate)
        {
            obj.SetActive(false);
        }


        /*objectToDeactivate1.SetActive(false);
        objectToDeactivate2.SetActive(false);
        objectToActivate1.SetActive(true);
        objectToActivate2.SetActive(true);*/
    }
}
