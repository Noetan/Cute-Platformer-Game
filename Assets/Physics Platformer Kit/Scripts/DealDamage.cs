using UnityEngine;

//attach this to any object which needs to deal damage to another object
public class DealDamage : MonoBehaviour 
{
	Health m_health;
    Rigidbody m_rigidBody;
	
	//remove health from object and push it
	public void Attack(GameObject victim, int dmg, float pushHeight, float pushForce)
	{
		m_health = victim.GetComponent<Health>();
        m_rigidBody = victim.GetComponent<Rigidbody>();	
		//push
		Vector3 pushDir = (victim.transform.position - transform.position);
		pushDir.y = 0f;
		pushDir.y = pushHeight * 0.1f;
		if (m_rigidBody && !m_rigidBody.isKinematic)
		{
            m_rigidBody.velocity = new Vector3(0, 0, 0);
            m_rigidBody.AddForce (pushDir.normalized * pushForce, ForceMode.VelocityChange);
            m_rigidBody.AddForce (Vector3.up * pushHeight, ForceMode.VelocityChange);
		}
		//deal dmg
		if(m_health && !m_health.flashing)
			m_health.currentHealth -= dmg;
	}
}

/* NOTE: if you just want to push objects you could use this script but set damage to 0. (ie: a bouncepad)
 * if you want to restore an objects health, set the damage to a negative number (ie: a healing bouncepad!) */