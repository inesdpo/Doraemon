using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;

    // Start is called before the first frame update
    void Start()
    {
        // Disable the second camera
        camera2.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCamera();
        }
    }

    public void SwitchCamera()
    {
        if (camera1.activeSelf)
        {
            camera1.gameObject.SetActive(false);
            camera2.gameObject.SetActive(true);

        }
        else
        {
            camera2.gameObject.SetActive(false);
            camera1.gameObject.SetActive(true);
        }
    }
}

    