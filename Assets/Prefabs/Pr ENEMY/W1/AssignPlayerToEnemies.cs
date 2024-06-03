using UnityEngine;

public class AssignPlayerToEnemies : MonoBehaviour
{
    public GameObject player; // Gracz, kt�ry ma by� przypisany do przeciwnik�w
    public GameObject[] enemyPrefabs; // Lista prefabi przeciwnik�w, do kt�rych chcemy przypisa� gracza

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
