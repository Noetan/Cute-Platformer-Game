using UnityEngine;
using System.Collections;

//this is a utility class. It holds collision information about this trigger, so another script can access that.
//for example: put this on an enemies "field of vision cone", and then the AIscript gets information like: has the player entered the field of vision?
public class TriggerParent : MonoBehaviour 
{
	public string[] tagsToCheck;			//if left empty, trigger will check collisions from everything. Othewise, it will only check these tags
	
	[HideInInspector]
	public bool collided, colliding;
	[HideInInspector]
	public GameObject hitObject;
	
	void Awake()
	{
		if(!GetComponent<Collider>() || (GetComponent<Collider>() && !GetComponent<Collider>().isTrigger))
			Debug.LogError ("'TriggerParent' script attached to object which does not have a trigger collider", transform);
	}
	
	//see if anything entered trigger, filter by tag, store the object
	void OnTriggerEnter (Collider other)
	{
		if (tagsToCheck.Length > 0 && !collided)
		{
			foreach (string tag in tagsToCheck)
			{
				if (other.tag == tag )
				{
					collided = true;
					hitObject = other.gameObject;
					break;
				}
					
			}
		}
		else
			collided = true;
			hitObject = other.gameObject;
	}
	
	//see if anything is constantly colliding with this trigger, filter by tag, store the object
	void OnTriggerStay (Collider other)
	{
		if (tagsToCheck.Length > 0)
		{
			foreach (string tag in tagsToCheck)
			{
				if (other.tag == tag )
				{
					hitObject = other.gameObject;
					colliding = true;
					break;
				}
			}
		}
		else
		{
			hitObject = other.gameObject;
			colliding = true;
		}
	}

    // Added OnTriggerExit to fix Enemy AI 'flickering' movement when not using VSync, as it was turned on and off every update
    void OnTriggerExit(Collider other)
    {
        if (tagsToCheck.Length > 0)
        {
            foreach (string tag in tagsToCheck)
            {
                if (other.tag == tag)
                {
                    colliding = false;
                    hitObject = null;
                    break;
                }
            }
        }

        else
            return;
    }

    //this runs after the main code, and resets the info to false
    //so we know when something is no longer colliding with this trigger
    void LateUpdate()
	{
        // Colliding bool here causes Enemy AI to move very slowly if VSync is off, as EnemyAI 'flickers' on and  off
        // Fixed by moving this to OnTriggerExit
		//colliding = false;
		collided = false;
		hitObject = null;
	}
}