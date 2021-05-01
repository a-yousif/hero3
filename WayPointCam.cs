using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointCam : MonoBehaviour
{
	public bool isActive = false;
	public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        cam.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	if (!isActive) {
    		cam.enabled = false;
    		//gameObject.SetActive(false);
    	} else {
    		cam.enabled = true;
    		
    	}
    }

    public void TryGrabCamera(GameObject g)
    {
    	gameObject.SetActive(true);
    	if (!isActive) {
    		float x = g.transform.position.x;
    		float y = g.transform.position.y;
    		float z = transform.position.z;
    		transform.position = new Vector3(x,y,z);
    		isActive = true;
    	}
    }
}
