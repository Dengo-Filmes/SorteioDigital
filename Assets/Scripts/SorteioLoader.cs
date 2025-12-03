using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SorteioLoader : MonoBehaviour
{
    public List<PlayerData> allPlayers = new List<PlayerData>();
    private string dataFolderPath;

    void Awake()
    {
        SetupDataFolder();
        LoadAllPlayers();
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
    }

    private void LoadAllPlayers()
    {
        allPlayers.Clear();

        if (!Directory.Exists(dataFolderPath))
            return;

        string[] files = Directory.GetFiles(dataFolderPath, "*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            PlayerData p = JsonUtility.FromJson<PlayerData>(json);

            if (p != null)
                allPlayers.Add(p);
        }

        Debug.Log($"[SORTEIO] Jogadores carregados: {allPlayers.Count}");
    }

    public PlayerData GetRandomPlayer()
    {
        if (allPlayers.Count == 0)
            return null;

        int r = Random.Range(0, allPlayers.Count);
        return allPlayers[r];
    }
}
