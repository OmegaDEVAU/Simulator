using UnityEngine;
using System.Collections;

public class ThrowABox : MonoBehaviour {
	public GameObject box;
	public float throwPower = 20;
	public bool drawGUI = false;
	
	void Update () {
		if(Input.GetButtonDown("Jump"))
		{
			Rigidbody newBody = ((GameObject)Instantiate(box, transform.position, Random.rotation)).GetComponent<Rigidbody>();
			newBody.AddForce(throwPower * transform.forward, ForceMode.Impulse);
			newBody.AddRelativeTorque(Random.insideUnitSphere, ForceMode.VelocityChange);
		}
	}
	
	void OnGUI()
	{
		if(drawGUI)
		{
			GUILayout.Space (30);
			GUILayout.Label("Press Space to test Interaction System!");
		}
	}
}
