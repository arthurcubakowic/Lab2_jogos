using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
                                    // Tile Revelada se tornou uma variavel inutil depois de algumas modificacoes, por isso eu a deletei
    public Sprite originalCarta;
    public Sprite backCarta;


    void Start()
    {
        EsconderCarta();
    }

    private void OnMouseDown()
    {
        GameObject.Find("gameManager").GetComponent<ManageCartas>().CartaSelecionada(gameObject);
    }

    public void EsconderCarta()
    {
        GetComponent<SpriteRenderer>().sprite = backCarta;
    }

    public void RevelaCarta()
    {
        GetComponent<SpriteRenderer>().sprite = originalCarta;
    }

    public void setCartaOriginal(Sprite novaCarta)
    {
        originalCarta = novaCarta;
    }


}
