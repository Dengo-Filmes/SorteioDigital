
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public string phoneNumber;
    public int luckyNumber;
    public DateTime registrationDate;

    public PlayerData(string name, string phone, int number)
    {
        playerName = name;
        phoneNumber = phone;
        luckyNumber = number;
        registrationDate = DateTime.Now;
    }
}

public class CadastroUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField inputNome;
    public TMP_InputField inputTelefone;
    public Button btnCadastrar;
    public TMP_Text txtNumeroSorte;

    [Header("Settings")]
    [SerializeField] private int minNumber = 0;
    [SerializeField] private int maxNumber = 999;
    [SerializeField] private float tempoExibicaoNumero = 7f;

    private List<PlayerData> registeredPlayers = new List<PlayerData>();
    private HashSet<int> usedNumbers = new HashSet<int>();
    private string dataFolderPath;
    private bool cadastrando = false;

    // NOVO: evita formatar enquanto reseta o input
    private bool podeFormatarTelefone = true;

    private void Start()
    {
        SetupDataFolder();
        LoadAllData();

        btnCadastrar.onClick.AddListener(OnCadastrar);

        inputTelefone.onValueChanged.AddListener(FormatarTelefone);
        inputNome.onValueChanged.AddListener(ValidarCampos);
        inputTelefone.onValueChanged.AddListener(ValidarCampos);

        ResetUI();
    }

    private void SetupDataFolder()
    {
#if UNITY_EDITOR
        dataFolderPath = Path.Combine(Application.dataPath, "DADOS_SALVOS");
#elif UNITY_STANDALONE_WIN
        dataFolderPath = Path.Combine(Application.dataPath, "..", "DADOS_SALVOS");
#else
        dataFolderPath = Path.Combine(Application.persistentDataPath, "DADOS_SALVOS");
#endif

        if (!Directory.Exists(dataFolderPath))
            Directory.CreateDirectory(dataFolderPath);
    }

    private void LoadAllData()
    {
        if (!Directory.Exists(dataFolderPath)) return;

        string[] files = Directory.GetFiles(dataFolderPath, "*.json");

        foreach (string file in files)
        {
            try
            {
                string jsonData = File.ReadAllText(file);
                PlayerData player = JsonUtility.FromJson<PlayerData>(jsonData);

                if (player != null)
                {
                    registeredPlayers.Add(player);
                    usedNumbers.Add(player.luckyNumber);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Erro ao carregar arquivo {file}: {ex.Message}");
            }
        }

        Debug.Log($"Carregados {registeredPlayers.Count} jogadores");
    }

    private void OnCadastrar()
    {
        if (cadastrando) return;

        cadastrando = true;

        string nome = inputNome.text.Trim();
        string telefone = ExtrairApenasNumeros(inputTelefone.text);

        if (string.IsNullOrEmpty(nome) || nome.Length < 3)
        {
            Debug.LogWarning("Nome deve ter pelo menos 3 caracteres!");
            cadastrando = false;
            return;
        }

        if (telefone.Length < 10 || telefone.Length > 11)
        {
            Debug.LogWarning("Telefone inválido! Digite DDD + número");
            cadastrando = false;
            return;
        }

        if (registeredPlayers.Any(p => p.phoneNumber == telefone))
        {
            PlayerData existente = registeredPlayers.First(p => p.phoneNumber == telefone);
            Debug.LogWarning($"Este telefone já está cadastrado! Número: {existente.luckyNumber:000}");
            cadastrando = false;
            return;
        }

        int luckyNumber = GerarNumeroSorteUnico();

        if (luckyNumber == -1)
        {
            Debug.LogWarning("Não há mais números disponíveis!");
            cadastrando = false;
            return;
        }

        PlayerData novoJogador = new PlayerData(nome, telefone, luckyNumber);
        registeredPlayers.Add(novoJogador);
        usedNumbers.Add(luckyNumber);

        SalvarJogador(novoJogador);

        MostrarNumeroSorte(luckyNumber);

        PlayerPrefs.SetString("UltimoNome", nome);
        PlayerPrefs.SetString("UltimoTelefone", telefone);
        PlayerPrefs.SetInt("UltimoNumero", luckyNumber);
        PlayerPrefs.Save();

        DesabilitarUI();

        Invoke("FinalizarCadastro", tempoExibicaoNumero);
    }

    private int GerarNumeroSorteUnico()
    {
        if (usedNumbers.Count >= 1000) return -1;

        int tentativas = 0;
        int novoNumero;

        do
        {
            novoNumero = UnityEngine.Random.Range(minNumber, maxNumber + 1);
            tentativas++;

            if (tentativas > 1000)
            {
                for (int i = minNumber; i <= maxNumber; i++)
                {
                    if (!usedNumbers.Contains(i))
                        return i;
                }
                return -1;
            }
        } while (usedNumbers.Contains(novoNumero));

        return novoNumero;
    }

    private void SalvarJogador(PlayerData jogador)
    {
        try
        {
            string fileName = $"player_{jogador.phoneNumber}_{jogador.luckyNumber:000}.json";
            string filePath = Path.Combine(dataFolderPath, fileName);

            string jsonData = JsonUtility.ToJson(jogador, true);
            File.WriteAllText(filePath, jsonData);

            SalvarNoArquivoConsolidado(jogador);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao salvar jogador: {ex.Message}");
        }
    }

    private void SalvarNoArquivoConsolidado(PlayerData jogador)
    {
        try
        {
            string fileName = $"todos_cadastros.txt";
            string filePath = Path.Combine(dataFolderPath, fileName);

            string linha = $"{DateTime.Now:dd/MM/yyyy HH:mm} | Nome: {jogador.playerName} | Telefone: {jogador.phoneNumber} | Número: {jogador.luckyNumber:000}";

            using (StreamWriter sw = new StreamWriter(filePath, true))
                sw.WriteLine(linha);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erro ao salvar no consolidado: {ex.Message}");
        }
    }

    private void FormatarTelefone(string texto)
    {
        if (!podeFormatarTelefone) return;

        string apenasNumeros = ExtrairApenasNumeros(texto);

        if (apenasNumeros.Length > 11)
            apenasNumeros = apenasNumeros.Substring(0, 11);

        string formatado;

        if (apenasNumeros.Length <= 2)
            formatado = $"({apenasNumeros}";
        else if (apenasNumeros.Length <= 6)
            formatado = $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2)}";
        else if (apenasNumeros.Length <= 10)
            formatado = $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2, 4)}-{apenasNumeros.Substring(6)}";
        else
            formatado = $"({apenasNumeros.Substring(0, 2)}) {apenasNumeros.Substring(2, 5)}-{apenasNumeros.Substring(7)}";

        if (inputTelefone.text != formatado)
        {
            inputTelefone.text = formatado;
            inputTelefone.caretPosition = formatado.Length;
        }
    }

    private string ExtrairApenasNumeros(string texto)
    {
        return new string(texto.Where(char.IsDigit).ToArray());
    }

    private void ValidarCampos(string _)
    {
        string nome = inputNome.text.Trim();
        string telefone = ExtrairApenasNumeros(inputTelefone.text);

        bool nomeValido = !string.IsNullOrEmpty(nome) && nome.Length >= 3;
        bool telefoneValido = telefone.Length >= 10 && telefone.Length <= 11;

        btnCadastrar.interactable = nomeValido && telefoneValido;
    }

    private void MostrarNumeroSorte(int numero)
    {
        if (txtNumeroSorte != null)
        {
            txtNumeroSorte.text = numero.ToString("000");
            txtNumeroSorte.gameObject.SetActive(true);
            StartCoroutine(AnimacaoNumeroSorte());
        }
    }

    private System.Collections.IEnumerator AnimacaoNumeroSorte()
    {
        float tempoDecorrido = 0f;
        Color corOriginal = txtNumeroSorte.color;
        Vector3 escalaOriginal = txtNumeroSorte.transform.localScale;

        while (tempoDecorrido < 6f)
        {
            float t = Mathf.PingPong(tempoDecorrido * 10f, 1f);
            txtNumeroSorte.color = Color.Lerp(Color.white, Color.green, t);

            float escala = Mathf.Lerp(1f, 1.2f, Mathf.Sin(tempoDecorrido * 10f) * 0.5f + 0.5f);
            txtNumeroSorte.transform.localScale = escalaOriginal * escala;

            tempoDecorrido += Time.deltaTime;
            yield return null;
        }

        float tempoFade = 0f;
        float duracaoFade = tempoExibicaoNumero - 3f;

        while (tempoFade < duracaoFade)
        {
            float alpha = Mathf.Lerp(1f, 0f, tempoFade / duracaoFade);
            Color corFade = txtNumeroSorte.color;
            corFade.a = alpha;
            txtNumeroSorte.color = corFade;

            float escalaFade = Mathf.Lerp(1f, 0.8f, tempoFade / duracaoFade);
            txtNumeroSorte.transform.localScale = escalaOriginal * escalaFade;

            tempoFade += Time.deltaTime;
            yield return null;
        }

        txtNumeroSorte.gameObject.SetActive(false);
        txtNumeroSorte.color = corOriginal;
        txtNumeroSorte.transform.localScale = escalaOriginal;
    }

    private void DesabilitarUI()
    {
        inputNome.interactable = false;
        inputTelefone.interactable = false;
        btnCadastrar.interactable = false;

        if (inputNome.image != null)
            inputNome.image.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);

        if (inputTelefone.image != null)
            inputTelefone.image.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);

        ColorBlock coresBtn = btnCadastrar.colors;
        coresBtn.disabledColor = new Color(0.6f, 0.6f, 0.6f, 0.5f);
        btnCadastrar.colors = coresBtn;
    }

    private void FinalizarCadastro()
    {
        ResetUI();
        cadastrando = false;
    }

    private void ResetUI()
    {
        podeFormatarTelefone = false;

        inputNome.text = "";
        inputTelefone.text = "";

        podeFormatarTelefone = true;

        if (txtNumeroSorte != null)
        {
            txtNumeroSorte.gameObject.SetActive(false);
            txtNumeroSorte.color = Color.white;
            txtNumeroSorte.transform.localScale = Vector3.one;
        }

        inputNome.interactable = true;
        inputTelefone.interactable = true;
        btnCadastrar.interactable = false;

        if (inputNome.image != null)
            inputNome.image.color = Color.white;

        if (inputTelefone.image != null)
            inputTelefone.image.color = Color.white;

        inputNome.Select();
    }

    public List<PlayerData> GetRegisteredPlayers()
    {
        return new List<PlayerData>(registeredPlayers);
    }

    public int GetPlayerLuckyNumber(string telefone)
    {
        var player = registeredPlayers.FirstOrDefault(p => p.phoneNumber == telefone);
        return player != null ? player.luckyNumber : -1;
    }

    void OnDestroy()
    {
        if (btnCadastrar != null)
            btnCadastrar.onClick.RemoveListener(OnCadastrar);

        if (inputTelefone != null)
            inputTelefone.onValueChanged.RemoveListener(FormatarTelefone);

        if (inputNome != null)
            inputNome.onValueChanged.RemoveListener(ValidarCampos);

        if (inputTelefone != null)
            inputTelefone.onValueChanged.RemoveListener(ValidarCampos);
    }
}
