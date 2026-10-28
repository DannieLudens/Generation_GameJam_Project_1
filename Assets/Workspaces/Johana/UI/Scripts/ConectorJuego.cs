using UnityEngine;

public class ConectorJuego : MonoBehaviour
{
    public PlayerController jugador;
    public HUDJuego hud;
    public AvisoMemorando aviso;
    public PantallaGameOver gameOver;

    private float metros = 0;
    private float tiempoAnterior;
    private int diaAnterior = 0;
    private int memosAnterior = 0;
    private bool termino = false;

    void Awake()
    {
        // dejo todo como al empezar (si cambian estos valores en PlayerController, cambiarlos aca tambien)
        PlayerController.gameOver = false;
        PlayerController.speed = 12f;
        PlayerController.obstacleDistance = 30f;
        PlayerController.clockProbability = 40f;
        Physics.gravity = new Vector3(0, -9.81f, 0);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tiempoAnterior = jugador.timer;
        hud.MostrarDia(1);
        hud.MostrarMemorandos(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (termino)
        {
            return;
        }

        // metros de toda la partida
        if (!PlayerController.gameOver)
        {
            metros += PlayerController.speed * Time.deltaTime;
        }
        hud.MostrarRecorrido(metros);
        hud.MostrarTiempo(jugador.timer);

        // cambio de dia
        if (jugador.cicle > diaAnterior)
        {
            diaAnterior = jugador.cicle;
            hud.MostrarDia(jugador.cicle + 1);
            hud.MostrarATiempo(jugador.cicle + 1);
        }
        // memorando
        else if (jugador.memos > memosAnterior)
        {
            memosAnterior = jugador.memos;
            hud.MostrarMemorandos(jugador.memos);

            if (jugador.memos >= 3)
            {
                termino = true;
                gameOver.PonerDatos((int)metros, jugador.cicle + 1);
            }

            aviso.MostrarMemorando(jugador.memos);
        }
        // reloj recogido o choque
        else
        {
            float cambio = jugador.timer - tiempoAnterior + Time.deltaTime;

            if (cambio >= 0.5f)
            {
                hud.MostrarPopupTiempo(Mathf.RoundToInt(cambio));
            }
            else if (cambio <= -0.5f)
            {
                hud.MostrarPopupTiempo(Mathf.RoundToInt(cambio));
            }
        }

        tiempoAnterior = jugador.timer;
    }
}
