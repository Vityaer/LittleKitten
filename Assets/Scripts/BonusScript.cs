using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelpFuction;
public class BonusScript : MonoBehaviour {

	public float amountFood = 1f;
    public float timeFly = 0.75f;
    public float deltaY = 0.1f;
	private Rigidbody2D rb;
    private Transform tr;
    private bool MoveUp;
    GameTimer timerMove;
    private bool work = false;
	// Use this for initialization
	void Awake(){
		tr = GetComponent<Transform>();
		rb = GetComponent<Rigidbody2D>();
		work = (UnityEngine.Random.Range(0f, 100f) < 50f);
	}
	void Start(){
        timerMove = TimerScript.Timer.StartLoopTimer(timeFly, ChangeDirection);
	}
	Vector2 direction = Vector2.zero;
	public void ChangeDirection(){
		MoveUp      = !MoveUp;
		direction.y =  MoveUp ? deltaY : -deltaY;
		rb.velocity = direction;  
	}
	void OnDestroy(){
		if(timerMove != null) TimerScript.Timer.StopTimer(timerMove);
	}
	bool isComplete = false;
	void OnTriggerEnter2D(Collider2D other){
		if(isComplete == false){
			if(other.gameObject.CompareTag("Cat")){
				isComplete = true;
				FoodControllerScript.Instance.IncreaseFood(amountFood);
				Destroy(gameObject);
			}
		}
	}
}
