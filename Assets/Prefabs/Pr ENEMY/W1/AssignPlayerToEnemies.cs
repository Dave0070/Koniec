using UnityEngine;

public class AssignPlayerToEnemies : MonoBehaviour
{
    public GameObject player; // Gracz, który ma byæ przypisany do przeciwników
    public GameObject[] enemyPrefabs; // Lista prefabi przeciwników, do których chcemy przypisaæ gracza

    void Start()
    {
        AssignPlayerToAllEnemies();
    }

    void AssignPlayerToAllEnemies()
    {
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            if (enemyPrefab.CompareTag("Minion"))
            {
                var enemyShip = enemyPrefab.GetComponent<EnemyShip>();
                if (enemyShip != null)
                {
                    enemyShip.player = player.transform;
                }
                else
                {
                    Debug.LogError("EnemyShip component not found on " + enemyPrefab.name);
                }
            }
        }
    }
}
