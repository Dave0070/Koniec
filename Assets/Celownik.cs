using UnityEngine;
using UnityEngine.UI;

public class Celownik : MonoBehaviour
{
    public float szerokoscObszaru = 200f;
    public float wysokoscObszaru = 200f;
    public Canvas canvas;
    public Image celownikImage;
    public float predkoscPrzesuwania = 5f;

    private Vector3 poczatkowaPozycja;
    private GameObject przeciwnik;

    void Start()
    {
        // Oblicz pocz¹tkow¹ pozycjê celownika na œrodku canvasu
        poczatkowaPozycja = new Vector3(canvas.pixelRect.width / 2f, canvas.pixelRect.height / 2f, 0f);
        celownikImage.rectTransform.position = poczatkowaPozycja; // Ustaw pocz¹tkow¹ pozycjê celownika
    }

    void Update()
    {
        // Kod aktualizacji celownika bez zmian
        Rect obszarWykrywania = new Rect(
            poczatkowaPozycja.x - szerokoscObszaru / 2f,
            poczatkowaPozycja.y - wysokoscObszaru / 2f,
            szerokoscObszaru,
            wysokoscObszaru
        );

        GameObject[] przeciwnicy = GameObject.FindGameObjectsWithTag("Enemy");

        przeciwnik = null;
        float najmniejszaOdleglosc = Mathf.Infinity;
        foreach (GameObject przeciwnikObiekt in przeciwnicy)
        {
            Vector3 pozycjaPrzeciwnikaNaCanvasie = Camera.main.WorldToScreenPoint(przeciwnikObiekt.transform.position);
            float odleglosc = Vector3.Distance(poczatkowaPozycja, pozycjaPrzeciwnikaNaCanvasie);
            if (odleglosc < najmniejszaOdleglosc && obszarWykrywania.Contains(pozycjaPrzeciwnikaNaCanvasie))
            {
                przeciwnik = przeciwnikObiekt;
                najmniejszaOdleglosc = odleglosc;
            }
        }

        if (przeciwnik != null)
        {
            Vector3 pozycjaPrzeciwnikaNaCanvasie = Camera.main.WorldToScreenPoint(przeciwnik.transform.position);
            Vector3 nowaPozycja = Vector3.MoveTowards(celownikImage.rectTransform.position, pozycjaPrzeciwnikaNaCanvasie, predkoscPrzesuwania * Time.deltaTime);
            celownikImage.rectTransform.position = nowaPozycja;
        }
        else
        {
            Vector3 nowaPozycja = Vector3.MoveTowards(celownikImage.rectTransform.position, poczatkowaPozycja, predkoscPrzesuwania * Time.deltaTime);
            celownikImage.rectTransform.position = nowaPozycja;
        }
    }

    public Vector3 GetPozycjaPrzeciwnika()
    {
        if (przeciwnik != null)
        {
            return przeciwnik.transform.position;
        }
        else
        {
            return transform.position;
        }
    }

    public bool IsPrzeciwnikWZasiegu()
    {
        return przeciwnik != null;
    }
}
