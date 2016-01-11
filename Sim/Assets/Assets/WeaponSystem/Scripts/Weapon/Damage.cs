using UnityEngine;
using System.Collections;

public class Damage : DamageBase
{
    public bool Explosive;
    public float ExplosionRadius = 20;
    public float ExplosionForce = 1000;
	public bool HitedActive = true;
	public float TimeActive = 0;
	private float timetemp = 0;

    private void Start()
    {
        if (!Owner || !Owner.GetComponent<Collider>()) return;
        Physics.IgnoreCollision(GetComponent<Collider>(), Owner.GetComponent<Collider>());
		
		timetemp = Time.time;
    }

    private void Update()
    {
		if(!HitedActive){
			if(Time.time >= (timetemp + TimeActive)){
				Active();
			}
		}
    }

    public void Active()
    {
        if (Effect)
        {
            GameObject obj = (GameObject) Instantiate(Effect, transform.position, transform.rotation);
            Destroy(obj, 3);
        }

        if (Explosive)
            ExplosionDamage();


        Destroy(gameObject);
    }

    private void ExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ExplosionRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            Collider hit = hitColliders[i];
            if (!hit)
                continue;

            if (hit.gameObject.GetComponent<DamageManager>())
            {
                if (hit.gameObject.GetComponent<DamageManager>())
                {
                    hit.gameObject.GetComponent<DamageManager>().ApplyDamage(Damage);
                }
            }
            if (hit.GetComponent<Rigidbody>())
                hit.GetComponent<Rigidbody>().AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 3.0f);
        }

    }

    private void NormalDamage(Collision collision)
    {
        if (collision.gameObject.GetComponent<DamageManager>())
        {
            collision.gameObject.GetComponent<DamageManager>().ApplyDamage(Damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
		if(HitedActive){
        	if (collision.gameObject.tag != "Particle" && collision.gameObject.tag != this.gameObject.tag)
        	{
            	if (!Explosive)
                	NormalDamage(collision);
            	Active();
        	}
		}
    }
}
