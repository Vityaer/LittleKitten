using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
public class FadeInOut : MonoBehaviour {
	
	public float fadeSpeed = 0.75f;
	public static string nextLevel;
	private Image _image;
	private static FadeInOut instance;
	public static FadeInOut Instance{get => instance;}
	void Awake (){
		instance = this;
		_image = GetComponent<Image>();
	}
	void Start(){
		StartScene();
	}
	public void RestartLevel(){
		EndScene(SceneManager.GetActiveScene().name);
	}

	Coroutine coroutineFade;
	public void StartScene (){
		if(coroutineFade != null){
			StopCoroutine(coroutineFade);
			coroutineFade = null;
		}
		coroutineFade = StartCoroutine(IFadeInOut(Color.clear));
	}

	public void EndScene (string nameScene){
		nextLevel = nameScene;
		if(coroutineFade != null){
			StopCoroutine(coroutineFade);
			coroutineFade = null;
		}
		coroutineFade = StartCoroutine(IFadeInOut(Color.black));
	}
	IEnumerator IFadeInOut(Color targetColor){
		float t = 0f;
		_image.enabled = true;
		while(t <= 1f){
			_image.color = Color.Lerp(_image.color, targetColor, t);
			t += fadeSpeed * Time.deltaTime;
			yield return null;
		}
		if(targetColor == Color.clear){
			_image.enabled = false;
		}else if(targetColor == Color.black){
			SceneManager.LoadScene(nextLevel);
		}	
	}
}