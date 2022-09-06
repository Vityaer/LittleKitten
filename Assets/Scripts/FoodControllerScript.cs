using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FoodControllerScript : MonoBehaviour{
	public Image hunderImage;
	public float maxFood = 100f, speedDeacrese = 1f;
	private float currentFood = 100f;
	private bool work = false;
	void Awake(){
		instance = this;
	}
	void Start(){
		currentFood = maxFood;
		StartHunger();
	}
	Coroutine coroutineHunger;
	public void StartHunger(){
		coroutineHunger = StartCoroutine(IHungerProcess());
		work = true;

	}
	public void IncreaseFood(float amount){
		currentFood += amount;
		if(currentFood > maxFood) currentFood = maxFood;
		UpdateUI();
	}
	public void DeacreaseFood(float amount){
		currentFood -= amount;
		if(currentFood < 0f) currentFood = 0f;
		UpdateUI();
	}
	void UpdateUI(){
		hunderImage.sprite = listSpriteAmountFood.GetSprite((int) (currentFood / maxFood * 100f));
	}
	IEnumerator IHungerProcess(){
		while(true){
			if(work) DeacreaseFood(speedDeacrese * Time.deltaTime);
			yield return null;
		}
	}
	public ListSpriteDependFromCount listSpriteAmountFood;
	private static FoodControllerScript instance;
	public static FoodControllerScript Instance{get => instance;}
}