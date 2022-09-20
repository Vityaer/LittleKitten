using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BirdControllerScript : MonoBehaviour{
	Coroutine coroutineMoveOnCycle = null;
	private Rigidbody2D rb;
	private Transform tr;
	public float speedMove = 1f;
	public PathContoller pathController;
	void Awake(){
		tr = base.transform;
		rb = GetComponent<Rigidbody2D>();
	}
	void Start(){
		if(pathController != null){
			tr.position = pathController.GetPosition(0);
			MoveToNextPoint();
		}else{
			Debug.Log("this bird not have path", gameObject);
		}
	}

	void MoveToNextPoint(){
		rb.DOMove(GetNextPoint(), 1f/speedMove).OnComplete(MoveToNextPoint);
	}
	public float t = 0f, step = 0.01f;
    Vector3 GetNextPoint(){
    	t += step;
    	if(t > 1) t = 0f;
    	return pathController.GetPosition(t);
    }
}