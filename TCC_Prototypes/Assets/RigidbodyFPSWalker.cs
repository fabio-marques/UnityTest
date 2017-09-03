using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class RigidbodyFPSWalker : MonoBehaviour {

	public GameObject wings;

	public Cinemachine.CinemachineVirtualCamera flightCam;

	public float speed = 10.0f;
	public float flightSpeed = 20.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	[Range (1, 10)]
	public float jumpHeight = 4.0f;
	[Range (20, 100)]
	public float MouseSensitivity = 60.0f;
	[HideInInspector]
	public bool playerInCtrl = true;

	public float fallMultiplier = 2.5f;
	public float lowJumpMultiplier = 2f;
	[Range (0.4f, 1)]
	public float glideStrength = 1f;

	private Rigidbody rb;
	private Transform myT;
	public CameraControl camCtrl;

	private bool grounded = false;
	private float GroundHeight = 1.15f;

	private bool jumped = false;
	[SerializeField]
	private bool wannaFly = false;
	private float flightHeight = 3f;


	private float flyRotSpeed = 2f;
	private float pitch, yaw = 0f;

	//Awake roda uma única vez, antes do Start, mesmo se o Script estiver desabilitado
	void Awake () { 
		rb = GetComponent<Rigidbody>();
		myT = GetComponent<Transform> ();
		rb.freezeRotation = true;
		rb.useGravity = false;

		flightCam.Priority = 6;
	}

	void Update(){
		// Jump ==============================================================================================================================
		if ((grounded && Input.GetButtonDown("Jump"))) { //Se o jogador pular enquanto estiver no chão
			jumped = true;
		}
		if ((!grounded && Input.GetKeyDown(KeyCode.LeftShift))) { //Se o jogador pular enquanto estiver no chão
			wannaFly = !wannaFly;
			camCtrl.enabled = !wannaFly;
			ResetFlight (!wannaFly);
			rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
		}

		if (!grounded && Input.GetButton ("Jump"))
			wings.SetActive (true);
		else
			wings.SetActive (false);
	}

	//FixedUpdate é preferível quando estiver mexendo com simulações físicas (e.g. Rigidbody)
	void FixedUpdate () {

		//Controla a rotação horizontal do jogador.
		//Mover o mouse no eixo X rotaciona o Player no Y e, consequentemente, a camera.
		float cameraY = Input.GetAxis ("Mouse X") * MouseSensitivity * Time.deltaTime; 

		if (playerInCtrl) {
			
			if (wannaFly) {
				if (FlightMode (cameraY))
					return;
			} else
				flightCam.Priority = 6;
			
			myT.Rotate (0, cameraY, 0);

			#region Normal Movement
			//A lógica por trás disso é poder aumentar a velocidade no rigidBody do jogador até alcançar a velocidade alvo.

			//Primeiro armazenamos a direção em que o jogador deseja andar, em relação ao eixo Global;
			//Em seguida, transformamos essa direção para o eixo Local do gameObject;
			//Com a direção definida, multiplicamos pela velocidade para descobrir a magnitude.
			//Assim, targetVelocity passa a indicar para onde o gameObject deve andar, e com que velocidade
			Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			targetVelocity = myT.TransformDirection (targetVelocity); 
			targetVelocity *= speed; 

			//movControl é a var que define quanto controle o jogador terá sobre o movimento. 1 é 100%.
			float movControl = 1.0f;
			if (grounded)
				movControl = 1.0f;
			else
				movControl = 0.3f;

			// Aplica uma força que tenta alcançar a targetVelocity ==========================================================================
			// Basicamente, aqui controlamos a aceleração.
			Vector3 velocity = rb.velocity;
			Vector3 velocityChange = (targetVelocity - velocity); //armazena a diferença entre a velocidade alvo e a velocidade atual.
			//Isso indica quanto a velocidade do jogador deve aumentar até alcançar a velocidade alvo.
			velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange); //limita o movimento ao maxVelocityChange
			velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange); 
			velocityChange.y = 0; 

			rb.AddForce (velocityChange * movControl, ForceMode.VelocityChange); //ForceMode.VelocityChange adiciona a força no rigidbody sem levar em conta a massa do objeto

			CheckGrounded (); //Checa se o jogador está no chão ou não

			if (jumped) {
				rb.velocity = new Vector3 (velocity.x, jumpHeight, velocity.z);
				jumped = false;
			}

			if (rb.velocity.y < 0 && !Input.GetButton ("Jump")) {	//Queda normal
				rb.velocity += Vector3.up * -gravity * (fallMultiplier - 1) * Time.deltaTime;
			} else if (rb.velocity.y < 0 && Input.GetButton ("Jump")) {	//Queda com Glide
				rb.velocity += Vector3.up * gravity * glideStrength * Time.deltaTime;
			} else if (rb.velocity.y > 0 && !Input.GetButton ("Jump")) {	//Pulo baixo (o pulo alto é o default)
				rb.velocity += Vector3.up * -gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
			}

			//Aplicando a Gravidade
			rb.AddForce (new Vector3 (0, -gravity * rb.mass, 0));

			#endregion

		} else {
			rb.AddForce (new Vector3 (0, -gravity * rb.mass, 0));
		}
	}

	void CheckGrounded () {
		bool ray1 = Physics.Raycast(myT.position, (Vector3.down * gravity).normalized, GroundHeight);
		Debug.DrawRay (myT.position, (Vector3.down * gravity).normalized * GroundHeight);
		bool ray2 = Physics.Raycast(myT.position + myT.forward/2, (Vector3.down * gravity).normalized, GroundHeight);
		Debug.DrawRay (myT.position + myT.forward/2, (Vector3.down * gravity).normalized * GroundHeight);
		bool ray3 = Physics.Raycast(myT.position - myT.forward/2, (Vector3.down * gravity).normalized, GroundHeight);
		Debug.DrawRay (myT.position - myT.forward/2, (Vector3.down * gravity).normalized * GroundHeight);

		grounded = false;
		if (ray1 || ray2 || ray3)
			grounded = true;
	}

	void ResetFlight(bool stopFlying){
		myT.localEulerAngles = new Vector3 (0, myT.localEulerAngles.y, 0);
		//yaw = pitch = roll = 0;
		if(stopFlying){
			flightCam.Priority = 6;
			camCtrl.enabled = true;
			wannaFly = false;
		}
	}

	bool FlightMode(float cameraY){
		Debug.DrawRay (myT.position, Vector3.down * flightHeight);
		if(grounded || Physics.Raycast(myT.position, Vector3.down, flightHeight)){
			ResetFlight (true);
			return false;
		}

		flightCam.Priority = 20;

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		myT.Rotate(0, h * flyRotSpeed, 0);

		pitch = v;
		yaw = h;

		myT.localEulerAngles = new Vector3 (pitch*30, myT.localEulerAngles.y, -yaw*20);

		rb.velocity = myT.forward * flightSpeed;

		return true;
	}

}