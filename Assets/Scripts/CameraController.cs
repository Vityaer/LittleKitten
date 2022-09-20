using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour{
	public CinemachineVirtualCamera CameraComponent;
	void Awake(){
		instance = this;
	}
	private static CameraController instance;
	public static CameraController Instance{get => instance;}
}
