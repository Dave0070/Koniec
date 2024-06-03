using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public GameObject minionPrefab;
    public Transform[] spawnPoints;
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;
    public int minMinionsToSpawn = 1;
    public int maxMinionsToSpawn = 3;
    public float orbitSpeed = 10f;
    public float orbitRadius = 10f;
    public Transform player;
    public GameObject shieldPrefab;
    public float minShieldInterval = 10f;
    public float maxShieldInterval = 20f;

    private bool isShieldActive = false;

    void Start()
    {
        StartCoroutine(SpawnMinions());
        StartCoroutine(OrbitAroundPlayer());
        StartCoroutine(ActivateShield());
    }

    IEnumerator SpawnMinions()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            int numMinions = Random.Range(minMinionsToSpawn, maxMinionsToSpawn + 1);
            for (int i = 0; i < numMinions; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(minionPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
    }

    IEnumerator OrbitAroundPlayer()
    {
        while (true)
        {
            Vector3 orbitPosition = player.position + Random.onUnitSphere * orbitRadius;
            Vector3 directionToOrbit = orbitPosition - transform.position;

            Quaternion lookRotation = Quaternion.LookRotation(directionToOrbit);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * orbitSpeed);

            transform.position = Vector3.MoveTowards(transform.position, orbitPosition, Time.deltaTime * orbitSpeed);

            yield return null;
        }
    }

    IEnumerator ActivateShield()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minShieldInterval, maxShieldInterval));

            isShieldActive = true;
            shieldPrefab.SetActive(true);

            yield return new WaitForSeconds(Random.Range(2f, 5f)); // Shield duration

            isShieldActive = false;
            shieldPrefab.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet") && !isShieldActive)
        {
            TakeDamage();
        }
    }

    void TakeDamage()
    {
        // Implement your damage logic here
    }
}
