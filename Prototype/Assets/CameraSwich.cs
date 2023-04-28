using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Disable the second camera
        camera2.enabled = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space));
    }
    public Camera camera1;
    public Camera camera2;
    public void SwichCamera()
    {
        if (camera1.enabled)
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

    