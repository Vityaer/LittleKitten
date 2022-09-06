using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WaterAreaScript : MonoBehaviour{
	private static LayerMask whatIsWater;
	private BuoyancyEffector2D buoyancyEffector;
    void OnTriggerEnter2D(Collider2D other){
		if(other.gameObject.CompareTag("Cat")){
			CatControllerScript.Instance.StartSwim(this);
			if(coroutineCheckSurfaceLevel == null)
				coroutineCheckSurfaceLevel = StartCoroutine(ICheckSurfaceLevel());
		}
	}
    void OnTriggerExit2D(Collider2D other){
		if(other.gameObject.CompareTag("Cat")){
			CatControllerScript.Instance.FinishSwim();
			if(coroutineCheckSurfaceLevel != null){
				StopCoroutine(coroutineCheckSurfaceLevel);
				coroutineCheckSurfaceLevel = null;
			}
		}
	}
	Coroutine coroutineCheckSurfaceLevel = null;
	private static float waterCheckRadius = 0.05f; 
	private static Vector3 checkPoint = Vector3.zero;
	public float offset = -1.7f;
	public void StartSurfaceLevel(Vector3 currentPos){
		buoyancyEffector.surfaceLevel = 0.5f * currentPos.y + offset;
	}
	IEnumerator ICheckSurfaceLevel(){
		while(true){
			checkPoint = CatControllerScript.Instance.GetPosition;
			checkPoint.y += 0.5f;
			if(Physics2D.OverlapCircle(checkPoint, waterCheckRadius, whatIsWater)){
				if(TransformLevel(checkPoint.y) > buoyancyEffector.surfaceLevel){
					buoyancyEffector.surfaceLevel = TransformLevel(checkPoint.y);
				}
			}else{
				if(TransformLevel(checkPoint.y) < buoyancyEffector.surfaceLevel){
					buoyancyEffector.surfaceLevel = TransformLevel(checkPoint.y) + offset;
				}
			}
			yield return null;
		}
	} 
	float TransformLevel(float y){ return 0.5f * y - 1.245f; }
	void Awake(){
		whatIsWater = LayerMask.GetMask("Water");
		buoyancyEffector = GetComponent<BuoyancyEffector2D>();
	}
}