using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public GameObject playerIndicator; // Obiekt reprezentuj¹cy pozycjê gracza na minimapie
    public GameObject enemyIndicatorPrefab; // Prefab wskaŸnika przeciwnika
    public float indicatorHeight = 10f; // Wysokoœæ, na której ma znajdowaæ siê wskaŸnik gracza na minimapie

    private GameObject[] enemyIndicators; // Tablica wskaŸników przeciwników

    void Start()
    {
        // Inicjalizacja tablicy wskaŸników przeciwników
        enemyIndicators = new GameObject[0];
    }

    void LateUpdate()
    {
        // Obróæ kamerê minimapy tak, aby patrzy³a z góry na scenê
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        // Oblicz pozycjê gracza na minimapie
        Vector3 playerPositionOnMap = new Vector3(player.position.x, indicatorHeight, player.position.z);

        // Ustaw pozycjê obiektu reprezentuj¹cego gracza na minimapie
        playerIndicator.transform.position = playerPositionOnMap;

        // SprawdŸ wszystkie obiekty z tagiem "Enemy" i zaktualizuj ich wskaŸniki na minimapie
        UpdateEnemyIndicators();
    }

    void UpdateEnemyIndicators()
    {
        // ZnajdŸ wszystkie obiekty z tagiem "Enemy" w scenie
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Jeœli liczba wskaŸników przeciwników jest ró¿na od liczby przeciwników, zaktualizuj tablicê wskaŸników
        if (enemyIndicators.Length != enemies.Length)
        {
            // Zniszcz istniej¹ce wskaŸniki przeciwników
            foreach (GameObject indicator in enemyIndicators)
            {
                Destroy(indicator);
            }

            // Zaktualizuj tablicê wskaŸników przeciwników
            enemyIndicators = new GameObject[enemies.Length];
        }

        // Ustaw wskaŸniki przeciwników na minimapie
        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 enemyPositionOnMap = new Vector3(enemies[i].transform.position.x, indicatorHeight, enemies[i].transform.position.z);

            // Jeœli wskaŸnik przeciwnika nie istnieje, utwórz nowy
            if (enemyIndicators[i] == null)
            {
                enemyIndicators[i] = Instantiate(enemyIndicatorPrefab, enemyPositionOnMap, Quaternion.identity);
            }
            else // W przeciwnym razie zaktualizuj jego pozycjê
            {
                enemyIndicators[i].transform.position = enemyPositionOnMap;
            }
        }
    }
}
