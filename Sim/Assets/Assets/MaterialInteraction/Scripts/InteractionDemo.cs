using UnityEngine;
public class InteractionDemo : MonoBehaviour {

	void OnCollisionEnter(Collision collision)
	{
		// Ask the system to spawn an effect based on the collision and the collider's physic material.
		MaterialInteractionSystem.SpawnEffect(collision, GetComponent<Collider>().sharedMaterial);
	}
}