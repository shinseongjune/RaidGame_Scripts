using Item;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ItemDatabase : MonoBehaviour
{
    #region Singleton
    static ItemDatabase _instance;
    static readonly object _lock = new object();

    public static ItemDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<ItemDatabase>();

                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<ItemDatabase>();
                            singletonObject.name = typeof(ItemDatabase).ToString() + " (Singletone)";

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

    public List<Equipable> weapons = new();
    public List<Equipable> helmets = new();
    public List<Equipable> armors = new();

    public List<Consumable> consumables = new();
    public List<ItemMaterial> materials = new();

    public List<MaterialRecipe> recipes = new();

    bool isLoadedWeapons = false;
    bool isLoadedHelmets = false;
    bool isLoadedArmors = false;
    bool isLoadedConsumables = false;
    bool isLoadedMaterials = false;
    bool isLoadedRecipes = false;

    void Start()
    {
        Addressables.LoadAssetsAsync<Equipable>("Equipable_Weapon", null).Completed += (AsyncOperationHandle<IList<Equipable>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    weapons.Add(item);
                }
                weapons.Sort();

                isLoadedWeapons = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<Equipable>("Equipable_Helmet", null).Completed += (AsyncOperationHandle<IList<Equipable>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    helmets.Add(item);
                }
                helmets.Sort();

                isLoadedHelmets = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<Equipable>("Equipable_Armor", null).Completed += (AsyncOperationHandle<IList<Equipable>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    armors.Add(item);
                }
                armors.Sort();

                isLoadedArmors = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<Consumable>("Consumable", null).Completed += (AsyncOperationHandle<IList<Consumable>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    consumables.Add(item);
                }
                consumables.Sort();

                isLoadedConsumables = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<ItemMaterial>("ItemMaterial", null).Completed += (AsyncOperationHandle<IList<ItemMaterial>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    materials.Add(item);
                }
                materials.Sort();

                isLoadedMaterials = true;
                LoadComplete();
            }
        };

        Addressables.LoadAssetsAsync<MaterialRecipe>("MaterialRecipe", null).Completed += (AsyncOperationHandle<IList<MaterialRecipe>> handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var item in handle.Result)
                {
                    recipes.Add(item);
                }
                recipes.Sort();

                isLoadedRecipes = true;
                LoadComplete();
            }
        };
    }

    void LoadComplete()
    {
        if (isLoadedWeapons && isLoadedHelmets && isLoadedArmors && isLoadedConsumables && isLoadedMaterials && isLoadedRecipes)
        {
            TitleManager.Instance.isLoadedItem = true;
            TitleManager.Instance.CheckDataLoaded();
        }
    }
}
