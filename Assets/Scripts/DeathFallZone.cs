using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFallZone : MonoBehaviour{
	bool isComplete = false;
	void OnTriggerEnter2D(Collider2D other){
		if(isComplete == false){
			if(other.gameObject.GetComponent<CatControllerScript>()){
				isComplete = true;
				CameraController.Instance.CameraComponent.Follow = null;
				StartCoroutine(IDeathFall());
			}
		}
	}
	IEnumerator IDeathFall(){
		yield return new WaitForSeconds(2f);
		FadeInOut.Instance.RestartLevel();
	}
}
