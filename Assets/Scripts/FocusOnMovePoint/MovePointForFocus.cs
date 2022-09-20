using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MovePointForFocus : MonoBehaviour{
	Coroutine coroutineMoveOnPath = null, coroutineWaitStartFocus = null;
	private Rigidbody2D rb;
	private Transform tr;
	public float speedMove = 1f;
	public PathContoller pathController;
	private Animator anim;
	void Awake(){
		tr = base.transform;
		rb = GetComponent<Rigidbody2D>();
		locScale = transform.localScale;
    	startPos = tr.position;
	}
	void Start(){
		if(pathController == null){
			Debug.Log("this point not have path", gameObject);
			this.enabled = false; 
		}else{
	    	targetPos = pathController.GetPosition(1f);
			coroutineWaitStartFocus = StartCoroutine(WaitStartFocus());
		}
	}

    bool startFollowMouse = false;
    Coroutine pressFocus = null;
    IEnumerator WaitStartFocus(){
    	while(startFollowMouse == false){
    		if(Input.GetKeyDown(KeyCode.F)){
    			Debug.Log("start focusing");
				pressFocus = StartCoroutine(IFocusing());
    		}
    		if(Input.GetKeyUp(KeyCode.F)){
    			if(pressFocus != null){
    				StopCoroutine(pressFocus);
    				pressFocus = null;
    			}
    		}
    		yield return null;
    	}
    }
    public float timeForFocusing = 2f;
    Vector3 startPos, targetPos;
    IEnumerator IFocusing(){
    	float time = timeForFocusing;
    	while(Input.GetKey(KeyCode.F) && startFollowMouse == false){
    		time -= Time.deltaTime;
    		tr.position = Vector2.Lerp(startPos, targetPos, 1f - time/timeForFocusing);
    		if(time < 0f){
    			startFollowMouse = true; 
    			yield return StartCoroutine(ReturnOnStart(1f));
    			coroutineMoveOnPath = StartCoroutine(MoveOnPath());
    		}
    		yield return null;
    	}
    }
    Vector3 posMouse;
    public float radiusCorrect, radiusWrong, deltaStep = 0.005f;
    IEnumerator MoveOnPath(){
    	Debug.Log("start move...");
    	float t = 0f, distance = 0f;
    	tr.position = pathController.GetPosition(0);
    	while(t <= 1f){
	    	posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	    	posMouse.z = tr.position.z;
	    	Debug.Log("posMouse: " + posMouse.ToString() +" and " + "tr.position: " + tr.position.ToString());
	    	distance = (tr.position - posMouse).sqrMagnitude;
	    	Debug.Log("distance: " + distance.ToString());
	    	if(distance <= radiusCorrect * radiusCorrect){
	    		t+= deltaStep;
	    		ChangeScale(maxScale);
	    		yield return StartCoroutine(MoveToPoint(t));
	    	}else if(distance >= radiusWrong * radiusWrong){
	    		Debug.Log("return...");
	    		ChangeScale(minScale);
	    		yield return StartCoroutine(ReturnOnStart(t));
	    		t = 0f;
	    	}else{
	    		ChangeScale(distance);
	    	}
	    	yield return null;
    	}
    	// anim.Play("Success");
    	Destroy(gameObject);
    }
    IEnumerator MoveToPoint(float value){
	    Debug.Log("move forward...");
    	Vector3 startPos = tr.position;
    	Vector3 targetPos = pathController.GetPosition(value);
    	float t = 0;
    	while(t <= 1f){
	    	tr.position = Vector2.Lerp(startPos, targetPos, t);
	    	t += Time.deltaTime * speedMove;
	    	yield return null;
    	}
	    Debug.Log("end move forward...");
    }
    IEnumerator ReturnOnStart(float t){
    	while(t >= 0f){
    		t -= deltaStep * 4;
    		yield return MoveToPoint(t);
    	}
    	yield return MoveToPoint(0f);
    }
    float scale = 1f;
	Vector3 locScale;
	public float minScale = 0.25f, maxScale = 1f;
    void ChangeScale(float distance){
    	scale = -0.375f * distance + 1.375f;
    	Mathf.Clamp(scale, minScale, maxScale); 
		locScale.x = scale;
		locScale.y = scale;
		locScale.z = scale;
		// transform.localScale = locScale;
    }
}
