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
    public GameObject Hero;
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
    public bool isChasing;
    eState state = eState.Patrol;
	// Use this for initialization
	void Start () {
		Hero = GameObject.Find("Hero");
        mWayPointIndex = sWayPoints.GetInitWayIndex();
        sr = gameObject.GetComponent<SpriteRenderer>();
        t = 0f;
        isChasing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == eState.Chase)
		{
			isChasing = true;
		}
		else
		{
			isChasing = false;
		}

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
            if (state == eState.Chase)
            {
            	Hero.GetComponent<HeroBehavior>().TouchedEnemy();
            	ThisEnemyIsHit();
            }
            else
            {
            	t = 0f;
            	//Color c = GetComponent<Renderer>().material.color;
            	GetComponent<Renderer>().material.color = new Color(1,0,0,1);
            	state = eState.CCW;
        	}

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
    		transform.RotateAround(transform.position, transform.forward, 90f*Time.smoothDeltaTime);

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
    	if (state == eState.CCW)
    	{

    		t += Time.smoothDeltaTime;
    		transform.RotateAround(transform.position, transform.forward, 90f*Time.smoothDeltaTime);
    		if (t >= 1f)
    		{
    			t = 0f;
    			state = eState.CW;
    		}
    	}
    	if (state == eState.CW)
    	{
    		t += Time.smoothDeltaTime;
    		transform.RotateAround(transform.position, transform.forward, -90f*Time.smoothDeltaTime);
    		if (t >= 1f)
    		{
    			t = 0f;
    			state = eState.Chase;
    		}
    	}
    	if (state==eState.Chase)
    	{
    		if ((transform.position - Hero.transform.position).magnitude <= 40f)
    		{
    			PointAtPosition((Hero.transform.position), kTurnRate);
	    		transform.position += (kSpeed * Time.smoothDeltaTime) * transform.up;
	    	}
	    	else
	    	{
	    		t = 0f;
	    		state=eState.Enlarge;
	    	}
    	}
    	if (state == eState.Enlarge)
    	{
    		t += Time.smoothDeltaTime;
    		if (t <= 1f) {
    			var startingScale = transform.localScale;
    			var endingScale = new Vector3(10f,10f,2f);
    			transform.localScale = Vector3.Lerp(startingScale, endingScale, t/60f);
    		}
    		else
    		{
    			t = 0f;
    			state = eState.Shrink;
    		}
    	}
    	if (state == eState.Shrink)
    	{
    		t += Time.smoothDeltaTime;
    		if (t <= 1f) {
    			var startingScale = transform.localScale;
    			var endingScale = new Vector3(5f,5f,2f);
    			transform.localScale = Vector3.Lerp(startingScale, endingScale, t/60f);
    		}
    		else
    		{
    			t = 0f;
    			GetComponent<Renderer>().material.color = new Color(1,1,1,1);
    			state = eState.Patrol;
    		}
    	}
    }
    #endregion
}
