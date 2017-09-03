using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
	public int quantityToSpawn;
	public GameObject enemyBall;

	void Start ()
	{
		for (int i = 0; i < quantityToSpawn; i++)
		{
			Vector3 position = Random.insideUnitSphere;
			position = Camera.main.ViewportToWorldPoint(new Vector2(Mathf.Abs(position.x), Mathf.Abs (position.y)));
			position.z = 0;
			
			GameObject obj = Instantiate(enemyBall, position, Quaternion.identity) as GameObject;
			obj.transform.parent = transform;
		}
	}
}
