using UnityEngine;
using UnityEngine.UI; // Dodajemy dla obs�ugi interfejsu u�ytkownika
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Wave
{
    public List<GameObject> enemies; // Lista przeciwnik�w wyst�puj�cych w danej rundzie
    public int numberOfEnemies; // Ilo�� przeciwnik�w w danej rundzie
    public float timeBetweenWaves; // Czas mi�dzy rundami
}

public class EnemyManager : MonoBehaviour
{
    public List<Wave> waves; // Lista rund
    public Transform[] spawnPoints; // Miejsca, w kt�rych mog� pojawi� si� przeciwnicy
    public GameObject player; // Gracz

    public Image waveStartImage; // Dodajemy obrazek do wy�wietlania na starcie fali
    public float waveStartDisplayTime = 3f; // Czas wy�wietlania obrazka na starcie fali

    private int currentWave = 0; // Aktualna runda
    private int enemiesRemaining; // Pozosta�a ilo�� przeciwnik�w w danej rundzie

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < waves.Count)
        {
            // Wy�wietlenie obrazka na starcie fali
            waveStartImage.enabled = true;
            yield return new WaitForSeconds(waves[currentWave].timeBetweenWaves - waveStartDisplayTime);
            waveStartImage.enabled = false;

            yield return new WaitForSeconds(waveStartDisplayTime);

            SpawnWave(waves[currentWave]);
            yield return new WaitUntil(() => enemiesRemaining <= 0); // Czekaj, a� wszyscy przeciwnicy zostan� pokonani
            currentWave++;
        }
    }

    void SpawnWave(Wave wave)
    {
        enemiesRemaining = wave.numberOfEnemies;

        for (int i = 0; i < wave.numberOfEnemies; i++)
        {
            GameObject enemyPrefab = wave.enemies[Random.Range(0, wave.enemies.Count)]; // Losowy wyb�r prefabu przeciwnika
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)]; // Losowy wyb�r punktu spawnu
            GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

            // Przypisanie gracza do przeciwnika
            if (newEnemy.CompareTag("Minion"))
            {
                Debug.Log("Przypisuj� gracza do Minion: " + newEnemy.name);
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
                enemyHealth.OnDeath += OnEnemyDeath; // Subskrypcja zdarzenia �mierci przeciwnika
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
