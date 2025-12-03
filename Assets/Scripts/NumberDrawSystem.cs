using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NumberDrawSystem : MonoBehaviour
{
    [Header("Cadastro Reference")]
    [SerializeField] private CadastroUI cadastroUI; 
    [Header("UI References")]
    [SerializeField] private TMP_Text numberDisplayText;
    [SerializeField] private TMP_Text winnerNameText; 
    [SerializeField] private Button drawButton;

    [Header("Animation Settings")]
    [SerializeField] private float initialSpeed = 0.05f;
    [SerializeField] private float finalSpeed = 0.5f;
    [SerializeField] private float animationDuration = 3f;
    [SerializeField] private float slowDownStartTime = 2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rollingSound;
    [SerializeField] private AudioClip finishSound;

    private bool isDrawing = false;
    private int finalNumber;
    private Coroutine drawCoroutine;

    private List<PlayerData> jogadores = new List<PlayerData>();


    void Start()
    {
        if (drawButton != null)
            drawButton.onClick.AddListener(StartDraw);

        if (numberDisplayText != null)
            numberDisplayText.text = "000";

        if (winnerNameText != null)
            winnerNameText.text = "";

        CarregarJogadoresDoCadastro();
    }

    private void CarregarJogadoresDoCadastro()
    {
        if (cadastroUI == null)
        {
            Debug.LogError(" cadastroUI não atribuído no inspector!");
            return;
        }

        jogadores = cadastroUI.GetRegisteredPlayers();

        Debug.Log($" Sorteio carregou {jogadores.Count} jogadores válidos.");
    }


    public void StartDraw()
    {
        if (isDrawing) return;

        CarregarJogadoresDoCadastro();

        if (jogadores.Count == 0)
        {
            Debug.LogWarning(" Nenhum jogador cadastrado para sortear!");
            return;
        }

        int index = Random.Range(0, jogadores.Count);
        finalNumber = jogadores[index].luckyNumber;

        Debug.Log($" Número sorteado REAL: {finalNumber:000}");

        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        drawCoroutine = StartCoroutine(DrawAnimation());
    }


    private IEnumerator DrawAnimation()
    {
        isDrawing = true;
        if (drawButton != null)
            drawButton.interactable = false;

        float elapsedTime = 0f;

        if (audioSource != null && rollingSound != null)
        {
            audioSource.clip = rollingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;

            float currentSpeed = elapsedTime < slowDownStartTime ?
                initialSpeed :
                Mathf.Lerp(initialSpeed, finalSpeed, (elapsedTime - slowDownStartTime) / (animationDuration - slowDownStartTime));

            int currentNumber = Random.Range(0, 1000);

            if (elapsedTime > animationDuration - 0.5f)
            {
                float t = (elapsedTime - (animationDuration - 0.5f)) / 0.5f;
                currentNumber = (int)Mathf.Lerp(currentNumber, finalNumber, t);
            }

            if (numberDisplayText != null)
                numberDisplayText.text = currentNumber.ToString("000");

            yield return new WaitForSeconds(currentSpeed);
            elapsedTime += currentSpeed;
        }

        numberDisplayText.text = finalNumber.ToString("000");

        if (audioSource != null)
        {
            audioSource.Stop();
            if (finishSound != null)
                audioSource.PlayOneShot(finishSound);
        }

        MostrarNomeDoGanhador(finalNumber);

        isDrawing = false;
        drawButton.interactable = true;
    }


    private void MostrarNomeDoGanhador(int numero)
    {
        if (winnerNameText == null) return;

        var ganhador = jogadores.Find(p => p.luckyNumber == numero);

        if (ganhador != null)
            winnerNameText.text = $"GANHADOR:\n{ganhador.playerName}\nNÚMERO: {ganhador.luckyNumber:000}";
        else
            winnerNameText.text = "Ninguém encontrado?";

    }


    void OnDestroy()
    {
        if (drawButton != null)
            drawButton.onClick.RemoveListener(StartDraw);
    }
}
