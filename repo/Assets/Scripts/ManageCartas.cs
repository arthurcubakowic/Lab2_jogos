using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageCartas : MonoBehaviour
{
    public GameObject carta;
    private bool primeiraCartaSelecionada, segundaCartaSelecionada;
    private GameObject carta1, carta2;

    bool timerAcionado; // O jogo funciona bem se timerPausado for igual a !timerAcionado, entao deletei essa variavel
    float timer;

    int numTentativas;
    int numAcertos;

    AudioSource somOK;

    public GameObject botoes; // tem que referenciar os botoes para poder desativar e ativar eles, se ficar por Find dara erro


    void Start()
    {
        if (PlayerPrefs.GetInt("Jogadas") < 26)     // se o jogo nao tiver record cria um record facil de bater
            PlayerPrefs.SetInt("Jogadas", 99999);   // se der End Game antes do jogo acabar tbm resetara o record

        botoes.SetActive(false); // desativa os botoes de reset e credit no inicio do jogo

        GameObject.Find("Recorde").GetComponent<Text>().text = "Recorde = " + PlayerPrefs.GetInt("Jogadas"); // atualiza o recorde do jogo


        numTentativas = 0;
        numAcertos = 0;
        MostraCartas();
        updateNumeroTentativas();
        somOK = GetComponent<AudioSource>();

        print(PlayerPrefs.GetInt("Jogadas"));
    }


    void Update()
    {
        if (timerAcionado)
        {
            timer += Time.deltaTime;
            print(timer);
            if (timer > 1)
            {
                timerAcionado = false;
                if (carta1.tag == carta2.tag)
                {
                    Destroy(carta1);
                    Destroy(carta2);
                    numAcertos++;
                    somOK.Play();

                    if (numAcertos == 26) // se voce acertar todas as cartas chama a funcao que termina o jogo
                        EndGame();
                }
                else
                {
                    carta1.GetComponent<Tile>().EsconderCarta();
                    carta2.GetComponent<Tile>().EsconderCarta();
                }
                primeiraCartaSelecionada = false;
                segundaCartaSelecionada = false;
                carta1 = null;
                carta2 = null;
                timer = 0;
            }
        }
    }

    public void MostraCartas() // Agora essa funcao cria 4 linhas, cada linha um Nipe
    {
        int[] arrayEmbaralhado = criaArrayEmbaralhado();
        int[] arrayEmbaralhado2 = criaArrayEmbaralhado();
        int[] arrayEmbaralhado3 = criaArrayEmbaralhado();
        int[] arrayEmbaralhado4 = criaArrayEmbaralhado();

        for (int i = 0; i < 13; i++)
        {
            AddUmaCarta(0, i, arrayEmbaralhado[i]);
            AddUmaCarta(1, i, arrayEmbaralhado2[i]);
            AddUmaCarta(2, i, arrayEmbaralhado3[i]);
            AddUmaCarta(3, i, arrayEmbaralhado4[i]);
        }
    }

    void AddUmaCarta(int linha, int rank, int valor)
    {
        GameObject centro = GameObject.Find("centroDaTela");
        float escalaCartaOriginal = carta.transform.localScale.x;
        float fatorEscalaX = (650 * escalaCartaOriginal) / 110.0f;
        float fatorEscalaY = (945 * escalaCartaOriginal) / 110.0f;
        Vector3 novaPosicao = new Vector3(centro.transform.position.x + ((rank - 13 / 2) * fatorEscalaX),
                                          centro.transform.position.y + ((linha - 4 / 2) * fatorEscalaY),
                                          centro.transform.position.z);
        GameObject c = (GameObject)(Instantiate(carta, novaPosicao, Quaternion.identity));
        c.tag = "" + (valor + 1);
        c.name = "" + linha + "_" + valor;

        string numeroCarta; // retirei a redundancia
        string nomeDaCarta; // retirei a redundancia

        if (valor == 0) numeroCarta = "Ace";
        else if (valor == 10) numeroCarta = "jack";
        else if (valor == 11) numeroCarta = "queen";
        else if (valor == 12) numeroCarta = "king";
        else numeroCarta = "" + (valor + 1);

        switch (linha) {
            case 0:
                nomeDaCarta = numeroCarta + "_of_clubs";    // se a linha for 0 o nipe é paus
                break;
            case 1:
                nomeDaCarta = numeroCarta + "_of_hearts";   // se a linha for 1 o nipe é copas
                break;
            case 2:
                nomeDaCarta = numeroCarta + "_of_spades";   // se a linha for 2 o nipe é espadas
                break;
            default:
                nomeDaCarta = numeroCarta + "_of_diamonds"; // se a linha for 3 o nipe é ouros
                break;
        }

        Sprite s1 = (Sprite)(Resources.Load<Sprite>(nomeDaCarta));
        print("S1: " + s1);
        GameObject.Find("" + linha + "_" + valor).GetComponent<Tile>().setCartaOriginal(s1);
    }

    public int[] criaArrayEmbaralhado()
    {
        int[] novoArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        int temp;
        for (int t=0; t<13; t++)
        {
            temp = novoArray[t];
            int r = Random.Range(0, 13);
            novoArray[t] = novoArray[r];
            novoArray[r] = temp;
        }
        return novoArray;
    }

    public void CartaSelecionada(GameObject carta)
    {
        if (!primeiraCartaSelecionada)
        {
            primeiraCartaSelecionada = true;
            carta1 = carta;
            carta1.GetComponent<Tile>().RevelaCarta();
        }
        else if (primeiraCartaSelecionada && !segundaCartaSelecionada)
        {
            segundaCartaSelecionada = true;
            carta2 = carta;
            carta2.GetComponent<Tile>().RevelaCarta();
            VerificaCartas();
        }
    }

    public void VerificaCartas()
    {
        DisparaTimer();
        numTentativas++;
        updateNumeroTentativas();
    }

    public void DisparaTimer()
    {
        timerAcionado = true;
    }

    public void updateNumeroTentativas()
    {
        GameObject.Find("numTentativas").GetComponent<Text>().text = "Tentativas = " + numTentativas;
    }

    public void EndGame()
    {
        if (PlayerPrefs.GetInt("Jogadas") >= numTentativas)                    // se o record anterior for maior que o atual, atualiza o record
        {
            PlayerPrefs.SetInt("Jogadas", numTentativas);
            GameObject.Find("Recorde").GetComponent<Text>().text = "Recorde = " + PlayerPrefs.GetInt("Jogadas"); // atualiza o recorde do jogo
            GameObject.Find("Main Camera").GetComponent<AudioSource>().Play(); // se o jogador bater o record anterior toca uma musica de parabenizacao localizada no GameObject Main Camera
        }

        botoes.SetActive(true); // ativa os botoes de reset e credit

    }

    public void Restart() // botao Restart reinicia o jogo
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Creditos() // botao que leva para os Creditos
    {
        SceneManager.LoadScene("Creditos");
    }

}
