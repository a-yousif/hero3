using UnityEngine;
using System.Collections;

public partial class EnemyBehavior : MonoBehaviour {

    // All instances of Enemy shares this one WayPoint and EnemySystem
    static private WayPointSystem sWayPoints = null;
    static private EnemySpawnSystem sEnemySystem = null;
    static public void InitializeEnemySystem(EnemySpawnSystem s, WayPointSystem w) { sEnemySystem = s; sWayPoints = w; }
    public SpriteRenderer sr;
    private const float kSpeed = 5f;
    private int mWayPointIndex = 0;
    public Vector3 destinationPos;
    public Vector3 startingPos;
    private const float kTurnRate = 0.03f/60f;
    private int mNumHit = 0;
    private const int kHitsToDestroy = 4;
    private const float kEnemyEnergyLost = 0.8f;
    public Sprite Default;
    public Sprite Stun;
    public Sprite Egg;
    public float t;
    private TimedLerp tlerp;
    enum eState
    {
    	Patrol,
    	Chase,
    	CCW,
    	CW,
    	Enlarge,
    	Shrink,
    	Stunned,
    	Egg
    }
    eState state = eState.Patrol;
	// Use this for initialization
	void Start () {
        mWayPointIndex = sWayPoints.GetInitWayIndex();
        sr = gameObject.GetComponent<SpriteRenderer>();
        t = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == eState.Patrol) {
	       sWayPoints.CheckNextWayPoint(transform.position, ref mWayPointIndex);
	       PointAtPosition(sWayPoints.WayPoint(mWayPointIndex), kTurnRate);
	       transform.position += (kSpeed * Time.smoothDeltaTime) * transform.up;
   		} else {
   			StateHandler();
   		}
    }

    private void PointAtPosition(Vector3 p, float r)
    {
        Vector3 v = p - transform.position;
        transform.up = Vector3.LerpUnclamped(transform.up, v, r);
    }

    #region Trigger into chase or die
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Emeny OnTriggerEnter");
        TriggerCheck(collision.gameObject);
    }

    private void TriggerCheck(GameObject g)
    {
        if (g.name == "Hero")
        {
            ThisEnemyIsHit();

        } else if (g.name == "Egg(Clone)")
        {
            mNumHit++;
            if (state != eState.Egg && state != eState.Stunned)
            {
            	
            	startingPos = transform.position;
            	destinationPos = transform.position + 4f*(g.transform.up.normalized);
            	tlerp = new TimedLerp(2, 2);
            	tlerp.BeginLerp(startingPos, destinationPos);
            	state = eState.Stunned;
                //Color c = GetComponent<Renderer>().material.color;
                //c.a = c.a * kEnemyEnergyLost;
                //GetComponent<Renderer>().material.color = c;
            }
            else if (state == eState.Stunned)
            {
            	state = eState.Egg;
            	startingPos = transform.position;
            	destinationPos = transform.position + 8f*(g.transform.up.normalized);
            	tlerp = new TimedLerp(2, 2);
            	tlerp.BeginLerp(startingPos, destinationPos);
            }
            else
            {
                ThisEnemyIsHit();
            }
        }
    }

    private void ThisEnemyIsHit()
    {
        sEnemySystem.OneEnemyDestroyed();
        Destroy(gameObject);
    }

    private void StateHandler() {
    	if (state == eState.Patrol)
    	{

    	}
    	if (state == eState.Stunned)
    	{
    		sr.sprite = Stun;
    		if (tlerp.LerpIsActive()) {
    			Vector3 s = tlerp.UpdateLerp();
    			float z = transform.position.z;
    			transform.position = new Vector3(s.x, s.y, z);
    		}
    		transform.RotateAround(transform.position, transform.forward, -90f*Time.smoothDeltaTime);

    	}
    	if (state == eState.Egg)
    	{
    		sr.sprite = Egg;
    		if (tlerp.LerpIsActive()) {
    			Vector3 s = tlerp.UpdateLerp();
    			float z = transform.position.z;
    			transform.position = new Vector3(s.x, s.y, z);
    		}
    		//transform.RotateAround(transform.position, transform.forward, -90f*Time.smoothDeltaTime);
    	}
    }
    #endregion
}
