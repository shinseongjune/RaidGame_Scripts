using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    #region Singleton
    static TitleManager _instance;
    static readonly object _lock = new object();

    public static TitleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<TitleManager>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<TitleManager>();
                            singletonObject.name = typeof(TitleManager).ToString() + " (Singletone)";
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
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton

    public bool isLoadedItem;
    public bool isLoadedSkill;
    public bool isLoadedCharacter;

    public GameObject panel_LoadingBlocker;

    public void StartGame()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void CheckDataLoaded()
    {
        if (isLoadedCharacter && isLoadedSkill && isLoadedItem)
        {
            panel_LoadingBlocker.SetActive(false);
        }
    }
}
