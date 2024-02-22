using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SkillDatabase : MonoBehaviour
{
    #region Singleton
    static SkillDatabase _instance;
    static readonly object _lock = new object();

    public static SkillDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<SkillDatabase>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<SkillDatabase>();
                            singletonObject.name = typeof(SkillDatabase).ToString() + " (Singletone)";

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

    public List<Skill> warriorBasic = new();
    public List<Skill> priestBasic = new();
    //public List<Skill> archerBasic = new();
    
    public List<Skill> warriorSkill = new();
    public List<Skill> priestSkill = new();
    //public List<Skill> archerSkill = new();

    bool isLoadedWarriorBasic = false;
    bool isLoadedWarriorSkill = false;
    bool isLoadedPriestBasic = false;
    bool isLoadedPriestSkill = false;
    //bool isLoadedArcherBasic = false;
    //bool isLoadedArcherSkill = false;

    private void Start()
    {
        #region warrior
        Addressables.LoadAssetsAsync<Skill>("Skill_Warrior_Basic", null).Completed += (AsyncOperationHandle<IList<Skill>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var skill in handle.Result)
                {
                    warriorBasic.Add(skill);
                }
                warriorBasic.Sort();

                isLoadedWarriorBasic = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<Skill>("Skill_Warrior_Skill", null).Completed += (AsyncOperationHandle<IList<Skill>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var skill in handle.Result)
                {
                    warriorSkill.Add(skill);
                }
                warriorSkill.Sort();

                isLoadedWarriorSkill = true;
                LoadComplete();
            }
        };
        #endregion warrior

        #region priest
        Addressables.LoadAssetsAsync<Skill>("Skill_Priest_Basic", null).Completed += (AsyncOperationHandle<IList<Skill>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var skill in handle.Result)
                {
                    priestBasic.Add(skill);
                }
                priestBasic.Sort();

                isLoadedPriestBasic = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<Skill>("Skill_Priest_Skill", null).Completed += (AsyncOperationHandle<IList<Skill>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var skill in handle.Result)
                {
                    priestSkill.Add(skill);
                }
                priestSkill.Sort();

                isLoadedPriestSkill = true;
                LoadComplete();
            }
        };
        #endregion priest

        #region archer
        /*
        Addressables.LoadAssetsAsync<Skill>("Skill_Archer_Basic", null).Completed += (AsyncOperationHandle<IList<Skill>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var skill in handle.Result)
                {
                    archerBasic.Add(skill);
                }
                archerBasic.Sort();

                isLoadedArcherBasic = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<Skill>("Skill_Archer_Skill", null).Completed += (AsyncOperationHandle<IList<Skill>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var skill in handle.Result)
                {
                    archerSkill.Add(skill);
                }
                archerSkill.Sort();

                isLoadedArcherSkill = true;
                LoadComplete();
            }
        };
        */
        #endregion archer
    }

    void LoadComplete()
    {
        //TODO: 다른 직업 체크 포함
        if (isLoadedWarriorBasic && isLoadedWarriorSkill && isLoadedPriestBasic && isLoadedPriestSkill)
        {
            TitleManager.Instance.isLoadedSkill = true;
            TitleManager.Instance.CheckDataLoaded();
        }
    }
}
