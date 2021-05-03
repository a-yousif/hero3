using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCamera : MonoBehaviour
{
    public Vector3 startingPos;
    public Vector3 destinationPos;
    public GameObject Hero;
    private TimedLerp tlerp;

    // Start is called before the first frame update
    void Start()
    {
        Hero = GameObject.Find("Hero");
        startingPos = transform.position;
        destinationPos = Hero.transform.position;
        tlerp = new TimedLerp(0.5f, 8);
        tlerp.BeginLerp(startingPos, destinationPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (tlerp.LerpIsActive())
        {
            Vector3 s = tlerp.UpdateLerp();
            float z = transform.position.z;
            transform.position = new Vector3(s.x, s.y, z);
        }
        else
        {
            Start();
        }
    }
}
