using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    private Vector3 mInitPosition = Vector3.zero;
    private int mHitCount = 0;
    private const int kHitLimit = 3;
    private const float kRepositionRange = 15f; // +- this value
    private Color mNormalColor = Color.white;
    private int stimer = 0;
    private float magnitude = 1f;
    private Vector3 startingPos;
    public GameObject waypointCam;
    enum wpState
    {
    	Rest,
    	Shake1,
    	Shake2,
    	Shake3
    }
    private wpState state = wpState.Rest;
    
    // Start is called before the first frame update
    void Start()
    {
    	//waypointCam = GameObject.Find("WayPoint Camera");
        mInitPosition = transform.position;
        startingPos = transform.position;
    }


    private void Reposition() 
    {
    	stimer = 0;
    	waypointCam.GetComponent<WayPointCam>().isActive = false;
        Vector3 p = mInitPosition;
        p += new Vector3(Random.Range(-kRepositionRange, kRepositionRange),
                         Random.Range(-kRepositionRange, kRepositionRange),
                         0f);
        transform.position = p;
        transform.rotation = new Quaternion (0,0,0,0);
        GetComponent<SpriteRenderer>().color = mNormalColor;
        startingPos = transform.position;
    }

    void FixedUpdate() {
    	if (stimer > 1) {
    		stimer-=1;
    		Shake();
    	}
    	if (stimer ==1) {
    		waypointCam.GetComponent<WayPointCam>().isActive = false;
    		stimer = 0;
    		state = wpState.Rest;
    		transform.position = startingPos;
    	}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Egg(Clone)")
        {
            mHitCount++;
            Color c = mNormalColor * (float)(kHitLimit - mHitCount + 1) / (float)(kHitLimit + 1);
            GetComponent<SpriteRenderer>().color = c;
            if (mHitCount > kHitLimit)
            {
                mHitCount = 0;
                Reposition();
            }
            else {
            	waypointCam.GetComponent<WayPointCam>().TryGrabCamera(gameObject);
            	if (mHitCount == 1)
            	{
            		state = wpState.Shake1;
            		stimer = 60;
            	}
            	else if (mHitCount == 2)
            	{
            		state = wpState.Shake2;
            		stimer = 120;
            	}
            	else if (mHitCount == 3)
            	{
            		state = wpState.Shake3;
            		stimer = 180;
            	}
            }
        }
    }

    private void Shake() {
    	if (state == wpState.Shake1)
    	{
    		ShakeCall(1f);
    	}
    	if (state == wpState.Shake2)
    	{
    		ShakeCall(2f);
    	}
    	if (state == wpState.Shake3)
    	{
    		ShakeCall(3f);;
    	}
    }

    private void ShakeCall(float strength) {
    	float x = startingPos.x + Mathf.Sin(Time.time * 40f) * magnitude * strength * (float) (stimer/(60f*strength)); // get random positions.
    	//float y = startingPos.y + Mathf.Sin(Time.time * 20f) * magnitude * strength;
    	float y = transform.position.y;
    	float z = transform.position.z;
    	transform.position = new Vector3(x,y,z);
    }
}
