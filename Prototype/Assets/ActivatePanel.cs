using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePanel : MonoBehaviour
{
    [SerializeField] private GameObject objectToDeactivate;
    [SerializeField] private GameObject objectToActivate1;
    [SerializeField] private GameObject objectToActivate2;

    public void TransitionToNextPanel()
    {
        objectToDeactivate.SetActive(false);
        objectToActivate1.SetActive(true);
        objectToActivate2.SetActive(true);
    }
}
