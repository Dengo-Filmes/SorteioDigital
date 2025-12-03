using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class NumberDrawSystem : MonoBehaviour
{
    [Header("Loader de Jogadores")]
    [SerializeField] private SorteioLoader loader;

    [Header("UI References")]
    [SerializeField] private TMP_Text numberDisplayText;
    [SerializeField] private TMP_Text winnerNameText;
    [SerializeField] private Button drawButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rollingSound;
    [SerializeField] private AudioClip finishSound;

    private bool isDrawing = false;
    private int finalNumber;
    private Coroutine drawCoroutine;

    void Start()
    {
        if (drawButton != null)
            drawButton.onClick.AddListener(StartDraw);

        numberDisplayText.text = "000";
        winnerNameText.text = "";
    }

    public void StartDraw()
    {
        if (isDrawing) return;

        if (loader.allPlayers.Count == 0)
        {
            Debug.LogWarning("Nenhum jogador encontrado no SorteioLoader!");
            return;
        }

        PlayerData p = loader.GetRandomPlayer();
        finalNumber = p.luckyNumber;

        Debug.Log($"Número sorteado REAL: {finalNumber:000}");

        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        drawCoroutine = StartCoroutine(DrawAnimation(p));
    }

    private IEnumerator DrawAnimation(PlayerData winner)
    {
        isDrawing = true;
        drawButton.interactable = false;

        // Som rolando
        if (audioSource != null && rollingSound != null)
        {
            audioSource.clip = rollingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // =====================================================================
        // 1) FASE RÁPIDA — 4 segundos, velocidade perceptível (1 - 999)
        // =====================================================================
        float fastDuration = 4f;
        float fastElapsed = 0f;

        while (fastElapsed < fastDuration)
        {
            float t = fastElapsed / fastDuration;

            // Velocidade ajustada para ficar perceptível (entre 8 ms - 20 ms)
            float speed = Mathf.Lerp(0.008f, 0.020f, Mathf.Pow(t, 1.7f));

            int n = Random.Range(1, 1000);
            numberDisplayText.text = n.ToString("000");

            fastElapsed += speed;
            yield return new WaitForSeconds(speed);
        }

        // =====================================================================
        // 2) FASE LENTA / SUSPENSE — 6 segundos com desaceleração exponencial
        // =====================================================================
        float slowDuration = 6f;
        float slowElapsed = 0f;

        while (slowElapsed < slowDuration)
        {
            float t = slowElapsed / slowDuration;

            // Atrasos crescentes (suspense): 20 ms - 180 ms
            float speed = Mathf.Lerp(0.020f, 0.180f, Mathf.Pow(t, 2.3f));

            // Número aleatório caminhando exponencialmente até o final
            int n = Random.Range(1, 1000);
            float blend = Mathf.Pow(t, 3f); // aproximação final suave
            n = (int)Mathf.Lerp(n, finalNumber, blend);

            numberDisplayText.text = n.ToString("000");

            slowElapsed += speed;
            yield return new WaitForSeconds(speed);
        }

        // =====================================================================
        // NÚMERO FINAL
        // =====================================================================
        numberDisplayText.text = finalNumber.ToString("000");

        if (audioSource != null)
        {
            audioSource.Stop();
            if (finishSound != null)
                audioSource.PlayOneShot(finishSound);
        }

        // =====================================================================
        // MOSTRAR GANHADOR
        // =====================================================================
        winnerNameText.text =
            $"GANHADOR:\n{winner.playerName}\nNÚMERO: {winner.luckyNumber:000}";

        isDrawing = false;
        drawButton.interactable = true;
    }

    void OnDestroy()
    {
        if (drawButton != null)
            drawButton.onClick.RemoveListener(StartDraw);
    }
}
