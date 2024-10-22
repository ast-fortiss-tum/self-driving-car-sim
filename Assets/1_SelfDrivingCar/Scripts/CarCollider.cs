using UnityEngine;

public class CarCollider : MonoBehaviour
{

	int obe;
	int collisions;

	private WayPointUpdate wayPointUpdate;

	void Start ()
	{
		wayPointUpdate = GetComponent<WayPointUpdate> ();
		obe = 0;
		collisions = 0;
	}

	// This method is needed for tracking OBE within JungleTrack
	void OnCollisionEnter (Collision collision)
	{
		if (collision.collider.name.Contains ("road-signs") || !collision.collider.name.Contains ("road")) {
			// collisions = collisions + 1;
			// Debug.Log ("Detected collision between " + gameObject.name + " and " + collision.collider.name);
			// Debug.Log ("Collision number " + collisions + " between " + gameObject.name + " and " + collision.collider.name);
			wayPointUpdate.registerCollision ();
		}
	}

	// This method is needed for tracking OBE within LakeTrack
	void OnTriggerEnter (Collider collider)
	{
		if (collider.name.Contains ("road-signs") || !collider.name.Contains ("road")) {
			// this.obe = this.obe + 1;
			// Debug.Log ("OUT OF BOUND Episode " + obe);
			wayPointUpdate.registerOutOfTrack ();
		}
	
	}

	public int getLastOBENumber ()
	{
		return this.obe;
	}

}
