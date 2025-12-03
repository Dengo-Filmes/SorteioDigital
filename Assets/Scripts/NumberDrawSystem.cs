using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class NumberDrawSystem : MonoBehaviour
{
    [Header("Loader de Jogadores")]
    [SerializeField] private SorteioLoader loader;

    [Header("UI References")]
    [SerializeField] private TMP_Text numberDisplayText;
    [SerializeField] private TMP_Text winnerNameText;
    [SerializeField] private TMP_Text winnerNumberText;
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
        winnerNumberText.text = "";
    }

    public void StartDraw()
    {
        if (isDrawing) return;

        if (loader.allPlayers.Count == 0)
        {
            Debug.LogWarning("Nenhum jogador carregado! Verifique o loader.");
            return;
        }

        PlayerData p = loader.GetRandomPlayer();
        finalNumber = p.luckyNumber;

        if (drawCoroutine != null)
            StopCoroutine(drawCoroutine);

        drawCoroutine = StartCoroutine(DrawAnimation(p));
    }

    private IEnumerator DrawAnimation(PlayerData winner)
    {
        isDrawing = true;
        drawButton.interactable = false;

        if (audioSource != null && rollingSound != null)
        {
            audioSource.clip = rollingSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        float fastDuration = 4f;
        float fastElapsed = 0f;

        while (fastElapsed < fastDuration)
        {
            float t = fastElapsed / fastDuration;
            float speed = Mathf.Lerp(0.008f, 0.020f, Mathf.Pow(t, 1.7f));

            int n = Random.Range(1, 1000);
            numberDisplayText.text = n.ToString("000");

            fastElapsed += speed;
            yield return new WaitForSeconds(speed);
        }

        float slowDuration = 6f;
        float slowElapsed = 0f;

        while (slowElapsed < slowDuration)
        {
            float t = slowElapsed / slowDuration;
            float speed = Mathf.Lerp(0.020f, 0.180f, Mathf.Pow(t, 2.3f));

            int n = Random.Range(1, 1000);
            float blend = Mathf.Pow(t, 3f);
            n = (int)Mathf.Lerp(n, finalNumber, blend);

            numberDisplayText.text = n.ToString("000");

            slowElapsed += speed;
            yield return new WaitForSeconds(speed);
        }

        numberDisplayText.text = finalNumber.ToString("000");

        if (audioSource != null)
        {
            audioSource.Stop();
            if (finishSound != null)
                audioSource.PlayOneShot(finishSound);
        }

        winnerNameText.text = winner.playerName;
        winnerNumberText.text = winner.luckyNumber.ToString("000");

        isDrawing = false;
        drawButton.interactable = true;
    }
}
