using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; // Gracz, którego bêdziemy œledziæ
    public float followSpeed = 5f; // Prêdkoœæ poruszania siê obiektu za graczem
    public float distanceFromPlayer = 5f; // Odleg³oœæ, jak¹ obiekt bêdzie utrzymywa³ od gracza
    public float driftAmount = 1f; // Wartoœæ okreœlaj¹ca si³ê "slidu"

    void Update()
    {
        if (player != null) // Sprawdzamy, czy gracz istnieje
        {
            // Obliczamy kierunek od obiektu do gracza
            Vector3 direction = player.position - transform.position;
            // Obliczamy docelow¹ pozycjê, która jest odleg³a od gracza o wartoœæ `distanceFromPlayer`
            Vector3 targetPosition = player.position - direction.normalized * distanceFromPlayer;
            // Uaktualniamy pozycjê obiektu, u¿ywaj¹c interpolacji liniowej, aby œledziæ gracza z zadan¹ prêdkoœci¹
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

            // Dodajemy "slid" do kierunku ruchu
            Vector3 drift = Random.insideUnitSphere * driftAmount;
            // Aktualizujemy kierunek ruchu, dodaj¹c "slid"
            direction += drift;
            // Normalizujemy kierunek, aby zachowaæ sta³¹ prêdkoœæ
            direction.Normalize();
            // Przesuwamy obiekt w zmodyfikowanym kierunku
            transform.position += direction * followSpeed * Time.deltaTime;
        }
    }
}
