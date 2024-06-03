using UnityEngine;

public class KulaPrzedGraczem : MonoBehaviour
{
    public Transform celownik; // Referencja do transformacji celownika
    public Transform gracz; // Referencja do transformacji gracza
    public float odlegloscOdGraczaCelowanie = 2f; // Odleg³oœæ kuli od gracza podczas celowania
    public float odlegloscOdGraczaBezCelowania = 5f; // Odleg³oœæ kuli od gracza bez celowania

    void Update()
    {
        if (celownik != null && gracz != null)
        {
            // SprawdŸ, czy przeciwnik jest obecny w celowniku
            bool czyPrzeciwnikWZasiegu = celownik.GetComponent<Celownik>().IsPrzeciwnikWZasiegu();

            if (czyPrzeciwnikWZasiegu)
            {
                // Pobierz pozycjê przeciwnika, na którym skupiony jest celownik
                Vector3 pozycjaPrzeciwnika = celownik.GetComponent<Celownik>().GetPozycjaPrzeciwnika();

                // Oblicz kierunek od gracza do celownika
                Vector3 kierunek = (pozycjaPrzeciwnika - gracz.position).normalized;

                // Oblicz pozycjê kuli na podstawie pozycji przeciwnika i odleg³oœci od niego podczas celowania
                Vector3 nowaPozycja = pozycjaPrzeciwnika - kierunek * odlegloscOdGraczaCelowanie;

                // Ustaw pozycjê kuli
                transform.position = nowaPozycja;
            }
            else
            {
                // Ustaw kulkê na ustalon¹ odleg³oœæ przed graczem, gdy celownik nie wskazuje na przeciwnika
                transform.position = gracz.position + gracz.forward * odlegloscOdGraczaBezCelowania;
            }
        }
    }
}