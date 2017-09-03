using UnityEngine;

public class WaypointControl : MonoBehaviour {
	static Color     linkColor     = Color.green; //define a cor da ligacao entre os Waypoints
	//var static - qdo vc muda o valor de uma copia, muda para todo mundo
	public Color     waypointColor = Color.cyan; //cor dos waypoints
	public float     radius        = 0.1F;
	public Transform next; //define qual o proximo, ou seja, qual waypoint esta ligado a ele

	public void OnDrawGizmos() { //funçao que permite desenhar informaçoes na aba game
		Gizmos.color = waypointColor; 
		Gizmos.DrawSphere(transform.position, radius); //na posicao do objeto que tem este script, desenha uma esfera com esse raio
		if (next != null) {
			Gizmos.color = linkColor;
			Gizmos.DrawLine(transform.position, next.position);
		}
	}
}

//A posicao Y do Waypoints (obj vazio criado na cena) deve ser um valor entre a posY do NPC e metade da posY do NPC