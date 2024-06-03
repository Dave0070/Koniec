using UnityEngine;
using UnityEngine.UI; // Dodajemy dla obs³ugi interfejsu u¿ytkownika
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public List<GameObject> enemies; // Lista przeciwników wystêpuj¹cych w danej rundzie
    public int numberOfEnemies; // Iloœæ przeciwników w danej rundzie
    public float timeBetweenWaves; // Czas miêdzy rundami
}

public class EnemyManager : MonoBehaviour
{
    public List<Wave> waves; // Lista rund
    public Transform[] spawnPoints; // Miejsca, w których mog¹ pojawiæ siê przeciwnicy
    public GameObject player; // Gracz

    public Image waveStartImage; // Dodajemy obrazek do wyœwietlania na starcie fali
    public float waveStartDisplayTime = 3f; // Czas wyœwietlania obrazka na starcie fali

    private int currentWave = 0; // Aktualna runda
    private int enemiesRemaining; // Pozosta³a iloœæ przeciwników w danej rundzie

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < waves.Count)
        {
            // Wyœwietlenie obrazka na starcie fali
            waveStartImage.enabled = true;
            yield return new WaitForSeconds(waves[currentWave].timeBetweenWaves - waveStartDisplayTime);
            waveStartImage.enabled = false;

            yield return new WaitForSeconds(waveStartDisplayTime);

            SpawnWave(waves[currentWave]);
            yield return new WaitUntil(() => enemiesRemaining <= 0); // Czekaj, a¿ wszyscy przeciwnicy zostan¹ pokonani
            currentWave++;
        }
    }

    void SpawnWave(Wave wave)
    {
        enemiesRemaining = wave.numberOfEnemies;

        for (int i = 0; i < wave.numberOfEnemies; i++)
        {
            GameObject enemyPrefab = wave.enemies[Random.Range(0, wave.enemies.Count)]; // Losowy wybór prefabu przeciwnika
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // Losowy wybór punktu spawnu
            GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

            // Przypisanie gracza do przeciwnika
            if (newEnemy.CompareTag("Minion"))
            {
                Debug.Log("Przypisujê gracza do Minion: " + newEnemy.name);
                var enemyShip = newEnemy.GetComponent<EnemyShip>();
                if (enemyShip != null)
                {
                    enemyShip.player = player.transform;
                    Debug.Log("Gracz przypisany do " + newEnemy.name);
                }
                else
                {
                    Debug.LogError("EnemyShip component not found on " + newEnemy.name);
                }
            }

            var enemyHealth = newEnemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.OnDeath += OnEnemyDeath; // Subskrypcja zdarzenia œmierci przeciwnika
            }
            else
            {
                Debug.LogError("EnemyHealth component not found on " + newEnemy.name);
            }
        }
    }

    void OnEnemyDeath()
    {
        enemiesRemaining--;
    }
}
