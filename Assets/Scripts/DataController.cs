using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class DataController : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] TMP_InputField _nameField;
    [SerializeField] TMP_InputField _IDField;
    [SerializeField] TMP_Dropdown _turnDropdown;
    [SerializeField] GameObject _continueButton;

    [Header("UI")]
    [SerializeField] TMP_Text _connectionStatusText;

    [Header("Data")]
    string _data;

    readonly string webAppUrl = "https://script.google.com/macros/s/AKfycbx4xEGyuTXXptNK-zRjxdtf40UbKhgej71QhvwwHMS3E1bVgihJ8968bc1CzltVdgw2/exec";

    const string folderName = "ACHE";
    const string onlineFileName = "ache_data.txt";
    const string offlineFileName = "ache_data_offline.txt";

    [Space(20)]
    bool _isWindows = true;
    bool _connected = false;
    bool _retry = false;

    bool _saveLocally = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                _isWindows = true;
                break;
            case RuntimePlatform.WebGLPlayer:
                _isWindows = false;
                break;
            default:
                _isWindows = true;
                break;
        }

        LoadUserData();
    }

    // Update is called once per frame
    void Update()
    {
        if (_retry)
        {
            _connectionStatusText.text = "Falha na conexão. Modo offline ativado";
        }
        else
        {
            if (!_connected) _connectionStatusText.text = "Carregando dados online...";
            else _connectionStatusText.text = "";
        }

        if (_nameField.text != string.Empty && _turnDropdown.value != 0 && _IDField.text != string.Empty) _continueButton.SetActive(true);
        else _continueButton.SetActive(false);
    }

    public void StartGame()
    {
        //if (_retry)
        //{
        //    LoadUserData();
        //    _retry = false;
        //    return;
        //}

        if(_connected || _saveLocally)
        {

        }
        else
        {
            print("<color=red>Trying to connect");
        }
    }

    public void SaveUserData()
    {
        string newData = _nameField.text + "," + _IDField.text + "," + _turnDropdown.value.ToString() + "\n";
        _data += newData;

        if(_saveLocally)
            File.WriteAllText(Application.dataPath + offlineFileName, _data);

        StartCoroutine(UploadCoroutine(_data));
    }

    public void LoadUserData()
    {
        if(_saveLocally && File.Exists(Application.dataPath + offlineFileName))
            _data += File.ReadAllText(Application.dataPath + offlineFileName);

        StartCoroutine(DownloadCoroutine());
    }

    IEnumerator UploadCoroutine(string content)
    {
        WWWForm form = new WWWForm();
        form.AddField("folder", folderName);
        form.AddField("file", _isWindows ? offlineFileName : onlineFileName);
        form.AddField("data", content);

        using (UnityWebRequest www = UnityWebRequest.Post(webAppUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro: " + www.error);
                File.WriteAllText(Application.dataPath + offlineFileName, content);
            }
            else
            {
                Debug.Log("Resposta: " + www.downloadHandler.text);
            }
        }
    }

    IEnumerator DownloadCoroutine()
    {
        string url = webAppUrl
        + "?folder=" + UnityWebRequest.EscapeURL(folderName)
        + "&file=" + UnityWebRequest.EscapeURL(_isWindows ? offlineFileName : onlineFileName);

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro: " + www.error);
                _retry = true;
                _saveLocally = true;
                if (_saveLocally && File.Exists(Application.dataPath + offlineFileName))
                    _data = File.ReadAllText(Application.dataPath + offlineFileName);
            }
            else
            {
                Debug.Log("<color=green>CONNECTED");

                _data = www.downloadHandler.text;
                _connected = true;
            }
        }
    }
}

public class UserData
{
    public string name;
    public string ID;
    public int turn;

    public UserData(string name, string ID, int turn)
    {
        this.name = name;
        this.ID = ID;
        this.turn = turn;
    }
}
