using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private float angulo = 0;
	private RigidbodyFPSWalker playerCtrl;

	void Awake () {
		playerCtrl = GetComponentInParent<RigidbodyFPSWalker> ();
	}

	void FixedUpdate () {
		angulo -= Input.GetAxis ("Mouse Y") * playerCtrl.MouseSensitivity * Time.deltaTime; //pego o eixo Y do Mouse, dou uma intensidade para ele, desvinculo do framerate e armazeno na var
		angulo = Mathf.Clamp (angulo, -80, 80); //eu determino o min e max de rotacao
		Vector3 rotacao = transform.localEulerAngles; //crio um vector3 e armazeno nele a rotacao atual do jogador.
		rotacao.x = angulo; //eu associo meu angulo ao eixo X do vector3. Isso pq o mouse Y deve girar a camera para cima e para baixo, o que acontece ao rotacionar a visao no eixo X.
		transform.localEulerAngles = rotacao; //eu faço a rotacao atual do jogador ser igual ao novo valor de Vector3.
	}
}
