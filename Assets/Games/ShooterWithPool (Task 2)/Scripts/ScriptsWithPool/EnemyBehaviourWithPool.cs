using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyBehaviourWithPool : MonoBehaviour
{
	[Header("Movement")]
	public float speed = 2f;

	[Header("Life Settings")]
	public float enemyHealth = 1f;

	[Header("Bullet Impact")]
	public ObjectPoolType bulletHitType;

	void Update()
	{
		if(!Settings.IsPlayerDead())
			transform.LookAt(Settings.PlayerPosition);
		Vector3 movement = transform.forward * speed * Time.deltaTime;
		this.GetComponent<Rigidbody>().MovePosition(transform.position + movement);
	}

	//Enemy Collision
	void OnTriggerEnter(Collider theCollider)
	{
		if (theCollider.tag != "Bullet")
			return;

		enemyHealth--;

		if(enemyHealth <= 0)
		{
			ObjectPooler.Instance.GetObjectFromPool(bulletHitType);
			gameObject.SetActive(false);
		}
	}

}
