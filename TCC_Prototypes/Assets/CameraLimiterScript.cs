using UnityEngine;
using System.Collections;

public class CameraLimiterScript : MonoBehaviour {

	public Transform target;

	private Transform playerCam;

	public static float cameraDistance = 4.0f;

	void Start(){
		playerCam = Camera.main.transform.parent.transform;
		UpdateCamerDistance (cameraDistance);
	}

	void Update () {
		transform.LookAt(target);

		RaycastHit hit;

		if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit)){
			if(hit.distance < cameraDistance){
				UpdateCamerDistance (hit.distance);
			}
		}else{
			UpdateCamerDistance (cameraDistance);
		}
	}

	void UpdateCamerDistance(float dist){
		Vector3 distancia = new Vector3 (playerCam.transform.localPosition.x, playerCam.transform.localPosition.y, -dist);
		playerCam.transform.localPosition = distancia;
	}
}
