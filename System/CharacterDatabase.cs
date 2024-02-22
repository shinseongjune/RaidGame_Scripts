using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterDatabase : MonoBehaviour
{
    #region Singleton
    static CharacterDatabase _instance;
    static readonly object _lock = new object();

    public static CharacterDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<CharacterDatabase>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<CharacterDatabase>();
                            singletonObject.name = typeof(CharacterDatabase).ToString() + " (Singletone)";

                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton

    public List<CharacterBaseData> players = new List<CharacterBaseData>();
    public List<CharacterBaseData> bosses = new List<CharacterBaseData>();

    bool isLoadedPlayer = false;
    bool isLoadedBoss = false;

    void Start()
    {
        Addressables.LoadAssetsAsync<CharacterBaseData>("Character_Player", null).Completed += (AsyncOperationHandle<IList<CharacterBaseData>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    players.Add(item);
                }
                players.Sort();

                isLoadedPlayer = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<CharacterBaseData>("Character_Boss", null).Completed += (AsyncOperationHandle<IList<CharacterBaseData>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    bosses.Add(item);
                }
                bosses.Sort();

                isLoadedBoss = true;
                LoadComplete();
            }
        };
    }

    void LoadComplete()
    {
        if (isLoadedPlayer && isLoadedBoss)
        {
            TitleManager.Instance.isLoadedCharacter = true;
            TitleManager.Instance.CheckDataLoaded();
        }
    }
}
