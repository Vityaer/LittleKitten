using UnityEngine;
using BansheeGz.BGSpline.Components;

[RequireComponent(typeof(BGCcMath))]
public class PathContoller : MonoBehaviour{
   	private BGCcMath mathComponent;
  	void Awake(){
   		mathComponent = GetComponent<BGCcMath>();
   	}
   	void Start(){ OnChangeMath(); }
   	float PathLength = 0f;
   	public Vector3 GetPosition(float alfa){ return mathComponent.CalcPositionByDistance(PathLength * alfa); }
	public void OnChangeMath(){ PathLength = mathComponent.GetDistance(); }
}