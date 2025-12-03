using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private UnityEvent OnTimerEnd = new();
    [SerializeField] private float holdTime = 2f;

    private float timer;
    private bool holding = false;
    private bool clicked = false;

    [Header("UI")]
    [SerializeField] private Image roundDial;

    [Header("Panels")]
    [SerializeField] private GameObject panelCadastro;
    [SerializeField] private GameObject panelSorteio;

    void Update()
    {
        HandleTimer();
        UpdateDialUI();
    }

    void UpdateDialUI()
    {
        if (roundDial != null)
            roundDial.fillAmount = timer / holdTime;
    }

    public void SetHoldButton(bool hold)
    {
        holding = hold;
    }

    void HandleTimer()
    {
        timer = holding ? timer + Time.deltaTime : 0;

        if (timer >= holdTime)
        {
            timer = holdTime;

            if (!clicked)
            {
                clicked = true;
                OnTimerEnd.Invoke();
                AbrirPainelSorteio();
            }
        }
        else
        {
            clicked = false;
        }
    }

    private void AbrirPainelSorteio()
    {
        if (panelCadastro != null)
            panelCadastro.SetActive(false);

        if (panelSorteio != null)
            panelSorteio.SetActive(true);

        Debug.Log("Painel de sorteio aberto!");
    }
}
