using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    [SerializeField] private string fileName;
    [SerializeField] private bool encryptData;
    private GameData gameData;
    private List<ISaveManager> saveManagers;
    private FileDataHandle dataHandle;

    private void Awake()
    {
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    [ContextMenu("Delete save file")]
    public void DeleteSavedData()
    {
        dataHandle = new FileDataHandle(Application.persistentDataPath, fileName, encryptData);
        dataHandle.Delete();
    }

    private void Start()
    {
        dataHandle = new FileDataHandle(Application.persistentDataPath, fileName, encryptData);

        saveManagers = FindAllSaveManagers();

        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        gameData = dataHandle.Load();

        if(this.gameData == null)
        {
            NewGame();
        }

        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        foreach(ISaveManager saveManager in saveManagers)
        {
            saveManager.SaveData(ref gameData);
        }

        dataHandle.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame(); 
    }

    private List<ISaveManager> FindAllSaveManagers()
    {
        IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();

        return new List<ISaveManager>(saveManagers);
    }

    public bool HasSaveData()
    {
        if(dataHandle.Load() != null)
        {
            return true;
        }

        return false;
    }

}
