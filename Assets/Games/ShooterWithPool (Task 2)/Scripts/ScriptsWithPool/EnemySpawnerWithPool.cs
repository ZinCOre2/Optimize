using UnityEngine;

public class EnemySpawnerWithPool : MonoBehaviour
{
	[Header("Enemy Spawn Info")]
	public bool spawnEnemies = true;
	public float enemySpawnRadius = 10f;
	public ObjectPoolType enemyType;

	[Header("Enemy Spawn Timing")]
	[Range(1, 100)] public int spawnsPerInterval = 1;
	[Range(.1f, 2f)] public float spawnInterval = 1f;
	
	float cooldown;


	void Start()
	{

	}

	void Update()
    {
		if (!spawnEnemies || Settings.IsPlayerDead())
			return;

		cooldown -= Time.deltaTime;

		if (cooldown <= 0f)
		{
			cooldown += spawnInterval;
			Spawn();
		}
    }

	void Spawn()
	{
		for (int i = 0; i < spawnsPerInterval; i++)
		{
			Vector3 pos = Settings.GetPositionAroundPlayer(enemySpawnRadius);
			var enemy = ObjectPooler.Instance.GetObjectFromPool(enemyType);

			enemy.transform.position = pos;
			enemy.transform.rotation = Quaternion.identity;
		}
	}
}
