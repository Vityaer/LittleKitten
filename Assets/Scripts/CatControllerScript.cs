using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatControllerScript : MonoBehaviour{
	bool inGame = true;
	private Transform tr;
	private Rigidbody2D rb;
	private Animator anim;
    [Header("Move")]
    public float SpeedWalk = 1.5f;
    public float timeForRun = 1f;	
    public float SpeedRun = 3f;
	public float SpeedOnLadder = 1f;
    public float SpeedVerticalSwim = 1f;
	public float PowerJump = 9f;
    public Transform groundCheck;
    
    private bool grounded = true;
	private bool isFacingRight = true;
    private float groundRadius = 0.05f;  
    public LayerMask whatIsGround;
    Vector2 jumpVector = Vector2.zero, moveVector = Vector2.zero;
    private bool speedFlag = false;
    [SerializeField] private bool swimFlag = false; 

	private float horizontalSpeed = 0f,verticalSpeed = 0f;
    private Vector3 upLadder, downLadder, ladderPos;
    private bool isLadder;
    public Vector3 GetPosition{get => tr.position;}
    void Awake(){
        if(instance == null){ instance = this; } else{Destroy(gameObject);}
		anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();	
    	jumpVector.y = PowerJump;
        horizontalSpeed = SpeedWalk;
    }
    void Start(){
        coroutineInaction  = StartCoroutine(IChechInaction());
        timerForRun = timeForRun;
    	coroutineWalkToRun = StartCoroutine(IChechRun());
    }
    void Update(){
    	GroundCheck();
    	if(Input.GetKeyDown(KeyCode.A)){
			if(isFacingRight == true) Flip();
            speedFlag = false;
    	}
    	if(Input.GetKeyDown(KeyCode.D)){
			if(isFacingRight == false) Flip();
            speedFlag = false;
    	}
    	if(Input.GetKey(KeyCode.A)){
            moveVector.x = -horizontalSpeed;
            speedFlag = true;
		}
        if(Input.GetKey(KeyCode.D)){
            moveVector.x = horizontalSpeed;
            speedFlag = true;
        }
    	if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)){
            moveVector.x = 0f;
        }
        wMove();
        if ((grounded || swimFlag) && Input.GetKeyDown(KeyCode.Space)){
            rb.velocity = jumpVector * ((swimFlag == false) ? 1f : 0.75f);
        }
        if(isLadder){
            if(Input.GetKey(KeyCode.W)){
                verticalSpeed = SpeedOnLadder;
            }
            if(Input.GetKey(KeyCode.S)){
                verticalSpeed = -SpeedOnLadder;
            }
            if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)){
                verticalSpeed = 0f;
            }
            if (isLadder) {
                LadderMode(verticalSpeed, moveVector.x);
                if(!grounded){
                    anim.SetBool("IsLadder", true);
                    if(verticalSpeed != 0){
                        anim.speed = 1;
                    }else{
                        anim.speed = 0;
                    }
                }else{
                    anim.speed = 1;
                    anim.SetBool("IsLadder", false);
                }
            }else{
                anim.speed = 1;
                anim.SetBool("IsLadder", false);
            } 
        }
        if(((speedFlag == true) || (grounded == false)) && (statusInaction == true)){
        	anim.SetBool("Inaction", false);
        }
   
    }
    void wMove(){
        anim.SetBool("Speed", speedFlag);
        moveVector.y = rb.velocity.y;
        rb.velocity = moveVector;
	}
    void GroundCheck(){
		grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Grounded", grounded);
	}
	private void Flip(){
		isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
//Swim in water
    Coroutine coroutineControllerWater = null;
    private WaterAreaScript waterArea;
    public void StartSwim(WaterAreaScript waterArea){
    	if(swimFlag == false){
            this.waterArea = waterArea;
            waterArea.StartSurfaceLevel(tr.position);
	        anim.SetBool("Swim", true);
			swimFlag = true;
            coroutineControllerWater = StartCoroutine(ISwim());
			anim.Play("Swim");
    	}
    }
    public void FinishSwim(){
		swimFlag = false;
        anim.SetBool("Swim", false);
        if(coroutineControllerWater != null){
            StopCoroutine(coroutineControllerWater);
            coroutineControllerWater = null;
        }
    }
    IEnumerator ISwim(){
        while(swimFlag){
            if(Input.GetKey(KeyCode.LeftShift)){
                if(Input.GetKey(KeyCode.S)){
                    moveVector.y = -SpeedVerticalSwim;
                    rb.velocity = moveVector;
                }
                if(Input.GetKey(KeyCode.W)){
                    moveVector.y = SpeedVerticalSwim;
                    rb.velocity = moveVector;
                }
            }
            yield return null;
        }
    }
//Inaction	
	[Header("Inaction")]
	public float timeForAnimInaction = 3f;
    Coroutine coroutineInaction = null;
	bool statusInaction = false;
	void StartStatusInaction(){
        if(coroutineChangeSpeed != null)
            StopCoroutine(coroutineChangeSpeed);
        coroutineChangeSpeed = null;
        speedFlag = false;
		statusInaction = true;
		anim.SetBool("Inaction", true);
		anim.Play("Sit");
	}
	float timerInaction;
	IEnumerator IChechInaction(){
		while(inGame){
			if((speedFlag == false) && (grounded == true)){
				timerInaction -= Time.deltaTime;
				if(timerInaction <= 0f) StartStatusInaction(); 
			}else{
				timerInaction = timeForAnimInaction;
			}
			yield return null;
		}
	}
	private bool HorizontalStop = true;
    void OnTriggerStay2D(Collider2D other){
        if (other.CompareTag("Ladder") && !isLadder){
            if(verticalSpeed != 0){
                rb.velocity = new Vector2(rb.velocity.x,0f);
                Ladder ladder = other.GetComponent<Ladder>();
                upLadder = ladder.up.position;
                downLadder = ladder.down.position;
                ladderPos = other.transform.position;
                HorizontalStop = ladder.HorizontalStop;
                if(grounded){
                    HorizontalStop = false;
                }
                isLadder = true;
            }

        }
        if(other.CompareTag("Ladder") && isLadder){
            if(other.GetComponent<Ladder>().HorizontalStop){
                if(!grounded){
                    HorizontalStop = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Ladder")){
            rb.gravityScale = 1;
            isLadder = false;
            rb.isKinematic = false;
            anim.SetBool("IsLadder", false);
            anim.speed = 1;
        }
    }
    void LadderMode(float vertical, float horizontal){
        if(transform.position.y < upLadder.y && vertical > 0){
            rb.isKinematic = true;
        }
        else if(transform.position.y > downLadder.y && vertical < 0 && transform.position.y > upLadder.y){
            rb.isKinematic = true;
        }
        else if(vertical < 0 && grounded && transform.position.y <= upLadder.y){
            rb.isKinematic = false;
        }
        if(vertical == 0 && horizontal == 0){
            rb.velocity = new Vector2(0f,0f);
        }
        else if(vertical != 0 && horizontal == 0){
            rb.velocity = new Vector2(0f,rb.velocity.y);
        }
        else if(vertical == 0 && horizontal != 0){
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
        if(grounded){
            HorizontalStop = false;
        }            
        if(rb.isKinematic && Mathf.Abs(vertical) > 0){
            rb.gravityScale = 0;
            transform.Translate(new Vector2(0, 2f * vertical * Time.deltaTime)); // движение по лестнице
            float xPos = Mathf.Lerp(transform.position.x, ladderPos.x, 10 * Time.deltaTime);
            if(Mathf.Abs(transform.position.x - ladderPos.x) < 0.5f){
                if(rb.velocity.y != 0){
                    transform.position = new Vector2(ladderPos.x, transform.position.y);
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            transform.position = new Vector2(xPos, transform.position.y); // плавное выравнивание по центру лестницы
        }
    }
    float timerForRun;
    Coroutine coroutineWalkToRun = null, coroutineChangeSpeed = null;
    IEnumerator IChechRun(){
        while(inGame){
            if((speedFlag == true) && (grounded == true)){
                if(timerForRun > 0){
                    timerForRun -= Time.deltaTime;
                    if(timerForRun <= 0f)
                        coroutineChangeSpeed = StartCoroutine(IChangeSpeedRun());
                }
            }else{
                timerForRun = timeForRun;
                horizontalSpeed = SpeedWalk;
            }
            yield return null;
        }
    }
    public float TimeForChangeSpeedFromWalkToRun = 0.5f;
    IEnumerator IChangeSpeedRun(){
        if(TimeForChangeSpeedFromWalkToRun == 0f) TimeForChangeSpeedFromWalkToRun = 0.5f;
        float t = 0f;
        while(t <= 1f){
            horizontalSpeed = Mathf.Lerp(SpeedWalk, SpeedRun, t); 
            t += Time.deltaTime * (1f/TimeForChangeSpeedFromWalkToRun);
            yield return null;
        }
        horizontalSpeed = SpeedRun;
    }
    private static CatControllerScript instance;
    public static CatControllerScript Instance{get => instance;}
}
