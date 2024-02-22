using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    GameObject characterModel;

    public GameObject itemPrefab;

    #region Character Settings
    public GameObject go_CharacterSettingsWindow;
    
    public Image image_HelmetSlot;
    public Image image_ArmorSlot;
    public Image image_WeaponSlot;
    
    public Image image_OneConsumableSlot;
    public Image image_TwoConsumableSlot;
    public Image image_ThreeConsumableSlot;
    public Image image_FourConsumableSlot;
    
    public Image image_QSkillSlot;
    public Image image_WSkillSlot;
    public Image image_ESkillSlot;
    
    public Transform transform_EquipsDropdownContent;
    public Transform transform_ConsumsDropdownContent;
    public Transform transform_SkillsDropdownContent;
    #endregion Character Settings

    #region Craft
    public Transform transform_RecipesDropdownContent;
    public Transform transform_MaterialsDropdownContent;
    
    public MaterialRecipe targetRecipe;
    public Image image_CraftTargetImage;
    public Transform transform_CraftTargetDropdownContent;

    public GameObject targetMaterialPrefab;
    #endregion Craft

    #region Save & Load
    public GameObject go_SaveWindow;
    public GameObject go_SaveSlotOne;
    public GameObject go_SaveSlotTwo;
    public GameObject go_SaveSlotThree;
    
    public GameObject go_LoadWindow;
    public GameObject go_LoadSlotOne;
    public GameObject go_LoadSlotTwo;
    public GameObject go_LoadSlotThree;
    #endregion Save & Load

    #region Selection Dropdowns
    public TMP_Dropdown dropdown_CharacterSelection;
    public TMP_Dropdown dropdown_BossSelection;
    #endregion Selection Dropdowns

    public GameObject go_DescWindow;

    void Start()
    {
        InitScene();
    }

    void InitScene()
    {
        dropdown_BossSelection.options.Clear();
        dropdown_CharacterSelection.options.Clear();

        var bosses = CharacterDatabase.Instance.bosses;
        int i = 0;
        StringBuilder stringBuilder = new StringBuilder();
        foreach (var boss in bosses)
        {
            stringBuilder.Clear();
            stringBuilder.Append(++i).Append(":").Append(boss.Name);
            dropdown_BossSelection.options.Add(new TMP_Dropdown.OptionData(stringBuilder.ToString()));
        }

        var playables = CharacterDatabase.Instance.players;
        foreach (var playable in playables)
        {
            dropdown_CharacterSelection.options.Add(new TMP_Dropdown.OptionData(playable.Name));
        }

        dropdown_BossSelection.value = LoginDataManager.Instance.currentPlayer.recentBoss;
        dropdown_CharacterSelection.value = LoginDataManager.Instance.currentPlayer.recentCharacter;

        SummonCharacterModel();
        UIUpdate_CharacterSettingsWindowUpdate();
        UIUpdate_CraftWindowUpdate();
        UIUpdate_SaveWindowUpdate();
        UIUpdate_LoadWindowUpdate();
    }

    public void BtnOnClick_ExitGame()
    {
        Application.Quit();
    }

    public void BtnOnClick_CharacterSettingsWindowOnOff()
    {
        if (go_CharacterSettingsWindow.activeSelf)
        {
            go_CharacterSettingsWindow.gameObject.SetActive(false);
        }
        else
        {
            UIUpdate_CharacterSettingsWindowUpdate();
            go_CharacterSettingsWindow.SetActive(true);
        }
    }

    public void BtnOnClick_SaveWindowOnOff()
    {
        if (go_SaveWindow.activeSelf)
        {
            go_SaveWindow.SetActive(false);
        }
        else
        {
            UIUpdate_SaveWindowUpdate();
            go_SaveWindow.SetActive(true);
            if (go_LoadWindow.activeSelf)
            {
                go_LoadWindow.SetActive(false);
            }
        }
    }

    public void BtnOnClick_Save(int index)
    {
        string folderPath = Path.Combine(Application.dataPath, "save");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"savedata_{index}.csv");

        PlayerData current = LoginDataManager.Instance.currentPlayer;

        SaveLoadManager.SavePlayerData(current, filePath);

        go_SaveWindow.SetActive(false);
        InitScene();
    }

    public void BtnOnClick_LoadWindowOnOff()
    {
        if (go_LoadWindow.activeSelf)
        {
            go_LoadWindow.SetActive(false);
        }
        else
        {
            UIUpdate_LoadWindowUpdate();
            go_LoadWindow.SetActive(true);
            if (go_SaveWindow.activeSelf)
            {
                go_SaveWindow.SetActive(false);
            }
        }
    }

    public void BtnOnClick_Load(int index)
    {
        string folderPath = Path.Combine(Application.dataPath, "save");
        
        if (!Directory.Exists(folderPath))
        {
            return;
        }

        string filePath = Path.Combine(folderPath, $"savedata_{index}.csv");
        
        if (!File.Exists(filePath))
        {
            return;
        }

        PlayerData loaded = SaveLoadManager.LoadPlayerData(filePath);

        LoginDataManager.Instance.currentPlayer = loaded;
        
        go_LoadWindow.SetActive(false);
        InitScene();
    }

    public void BtnOnClick_StartGame()
    {
        SceneManager.LoadScene("BossBattleScene");
    }

    public void DropdownOnValueChanged_SelectBoss()
    {
        int index = dropdown_BossSelection.value;

        LoginDataManager.Instance.currentPlayer.recentBoss = index;
    }

    public void DropdownOnValueChanged_SelectCharacter()
    {
        int index = dropdown_CharacterSelection.value;

        LoginDataManager.Instance.currentPlayer.recentCharacter = index;

        SummonCharacterModel();
        UIUpdate_CharacterSettingsWindowUpdate();
    }

    public void SummonCharacterModel()
    {
        if (characterModel is not null)
        {
            Destroy(characterModel);
        }

        int recentCharacter = LoginDataManager.Instance.currentPlayer.recentCharacter;
        GameObject prefab = CharacterDatabase.Instance.players[recentCharacter].prefab;
        characterModel = Instantiate(prefab, Vector3.zero, Quaternion.Euler(0, 180, 0));
    }

    public void UIUpdate_CharacterSettingsWindowUpdate()
    {
        var currentPlayer = LoginDataManager.Instance.currentPlayer;
        int recent = currentPlayer.recentCharacter;
        var settings = currentPlayer.dic_charSettings[recent];

        StringBuilder stringBuilder = new StringBuilder();

        List<Skill> skills = recent switch
        {
            0 => SkillDatabase.Instance.warriorSkill,
            1 => SkillDatabase.Instance.priestSkill,
            //2 => SkillDatabase.Instance.archerSkill,
            _ => throw new IndexOutOfRangeException("Invalid Character Index!")
        };
        //left
        #region equips
        if (settings.head == -1)
        {
            image_HelmetSlot.sprite = null;
            image_HelmetSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Helmet";
        }
        else
        {
            int itemIndex = currentPlayer.helmets[settings.head];
            Equipable head = ItemDatabase.Instance.helmets[itemIndex];

            image_HelmetSlot.sprite = head.sprite;
            image_HelmetSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = head.itemName;
        }

        if (settings.armor == -1)
        {
            image_ArmorSlot.sprite = null;
            image_ArmorSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Armor";
        }
        else
        {
            int itemIndex = currentPlayer.armors[settings.armor];
            Equipable armor = ItemDatabase.Instance.armors[itemIndex];

            image_ArmorSlot.sprite = armor.sprite;
            image_ArmorSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = armor.itemName;
        }

        if (settings.weapon == -1)
        {
            image_WeaponSlot.sprite = null;
            image_WeaponSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = "Weapon";
        }
        else
        {
            int itemIndex = currentPlayer.weapons[settings.weapon];
            Equipable weapon = ItemDatabase.Instance.weapons[itemIndex];

            image_WeaponSlot.sprite = weapon.sprite;
            image_WeaponSlot.transform.GetComponentInChildren<TextMeshProUGUI>().text = weapon.itemName;
        }
        #endregion equips

        #region consums
        if (settings.itemOne == -1)
        {
            image_OneConsumableSlot.sprite = null;
        }
        else
        {
            Consumable con = ItemDatabase.Instance.consumables[settings.itemOne];

            if (currentPlayer.dic_consumables[con] <= 0)
            {
                image_OneConsumableSlot.sprite = null;
                settings.itemOne = -1;
            }
            else
            {
                image_OneConsumableSlot.sprite = con.sprite;
            }
        }

        if (settings.itemTwo == -1)
        {
            image_TwoConsumableSlot.sprite = null;
        }
        else
        {
            Consumable con = ItemDatabase.Instance.consumables[settings.itemTwo];

            if (currentPlayer.dic_consumables[con] <= 0)
            {
                image_TwoConsumableSlot.sprite = null;
                settings.itemTwo = -1;
            }
            else
            {
                image_TwoConsumableSlot.sprite = con.sprite;
            }
        }

        if (settings.itemThree == -1)
        {
            image_ThreeConsumableSlot.sprite = null;
        }
        else
        {
            Consumable con = ItemDatabase.Instance.consumables[settings.itemThree];

            if (currentPlayer.dic_consumables[con] <= 0)
            {
                image_ThreeConsumableSlot.sprite = null;
                settings.itemThree = -1;
            }
            else
            {
                image_ThreeConsumableSlot.sprite = con.sprite;
            }
        }

        if (settings.itemFour == -1)
        {
            image_FourConsumableSlot.sprite = null;
        }
        else
        {
            Consumable con = ItemDatabase.Instance.consumables[settings.itemFour];

            if (currentPlayer.dic_consumables[con] <= 0)
            {
                image_FourConsumableSlot.sprite = null;
                settings.itemFour = -1;
            }
            else
            {
                image_FourConsumableSlot.sprite = con.sprite;
            }
        }
        #endregion consums

        #region skills
        if (settings.q == -1)
        {
            image_QSkillSlot.sprite = null;
        }
        else
        {
            image_QSkillSlot.sprite = skills[settings.q].sprite;
        }

        if (settings.w == -1)
        {
            image_WSkillSlot.sprite = null;
        }
        else
        {
            image_WSkillSlot.sprite = skills[settings.w].sprite;
        }

        if (settings.e == -1)
        {
            image_ESkillSlot.sprite = null;
        }
        else
        {
            image_ESkillSlot.sprite = skills[settings.e].sprite;
        }
        #endregion skills
        //left end

        //right
        #region equip dropdown
        for (int i = transform_EquipsDropdownContent.childCount - 1; i >= 0; --i)
        {
            Destroy(transform_EquipsDropdownContent.GetChild(i).gameObject);
        }

        var helmets = currentPlayer.helmets;
        var armors = currentPlayer.armors;
        var weapons = currentPlayer.weapons;
        var allHelmets = ItemDatabase.Instance.helmets;
        var allArmors = ItemDatabase.Instance.armors;
        var allWeapons = ItemDatabase.Instance.weapons;

        for (int i = 0; i < helmets.Count; ++i)
        {
            int helmet = helmets[i];
            UI_ItemPrefab item = Instantiate(itemPrefab, transform_EquipsDropdownContent).GetComponent<UI_ItemPrefab>();

            item.type = UI_ItemPrefab.Type.HELMETS;
            item.id = i;
            if (currentPlayer.dic_charSettings[currentPlayer.recentCharacter].head == i)
            {
                item.isSelected = true;
                item.GetComponent<Image>().color = new Color(0, 0, 1f, 0.5f);
            }

            item.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = allHelmets[helmet].itemName;

            item.GetComponent<Image>().sprite = allHelmets[helmet].sprite;
            if (allHelmets[helmet].description is not null && allHelmets[helmet].description.Length > 0)
            {
                item.description = allHelmets[helmet].description;
                item.go_descWindow = go_DescWindow;
                item.hasDesc = true;
            }

            UI_ItemPrefab localItem = item;
            item.GetComponentInChildren<Button>().onClick.AddListener(() => {
                BtnOnClick_ItemPrefab(localItem);
            });
        }

        for (int i = 0; i < armors.Count; ++i)
        {
            int armor = armors[i];
            UI_ItemPrefab item = Instantiate(itemPrefab, transform_EquipsDropdownContent).GetComponent<UI_ItemPrefab>();

            item.type = UI_ItemPrefab.Type.ARMORS;
            item.id = i;
            if (currentPlayer.dic_charSettings[currentPlayer.recentCharacter].armor == i)
            {
                item.isSelected = true;
                item.GetComponent<Image>().color = new Color(0, 0, 1f, 0.5f);
            }

            item.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = allArmors[armor].itemName;

            item.GetComponent<Image>().sprite = allArmors[armor].sprite;
            if (allArmors[armor].description is not null && allArmors[armor].description.Length > 0)
            {
                item.description = allArmors[armor].description;
                item.go_descWindow = go_DescWindow;
                item.hasDesc = true;
            }

            UI_ItemPrefab localItem = item;
            item.GetComponentInChildren<Button>().onClick.AddListener(() => {
                BtnOnClick_ItemPrefab(localItem);
            });
        }

        for (int i = 0; i < weapons.Count; ++i)
        {
            int weapon = weapons[i];
            UI_ItemPrefab item = Instantiate(itemPrefab, transform_EquipsDropdownContent).GetComponent<UI_ItemPrefab>();

            item.type = UI_ItemPrefab.Type.WEAPONS;
            item.id = i;
            if (currentPlayer.dic_charSettings[currentPlayer.recentCharacter].weapon == i)
            {
                item.isSelected = true;
                item.GetComponent<Image>().color = new Color(0, 0, 1f, 0.5f);
            }

            item.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = allWeapons[weapon].itemName;

            item.GetComponent<Image>().sprite = allWeapons[weapon].sprite;
            if (allWeapons[weapon].description is not null && allWeapons[weapon].description.Length > 0)
            {
                item.description = allWeapons[weapon].description;
                item.go_descWindow = go_DescWindow;
                item.hasDesc = true;
            }

            UI_ItemPrefab localItem = item;
            item.GetComponentInChildren<Button>().onClick.AddListener(() => {
                BtnOnClick_ItemPrefab(localItem);
            });
        }
        #endregion equip dropdown

        #region consum dropdown
        for (int i = transform_ConsumsDropdownContent.childCount - 1; i >= 0; --i)
        {
            Destroy(transform_ConsumsDropdownContent.GetChild(i).gameObject);
        }

        var myCons = LoginDataManager.Instance.currentPlayer.dic_consumables;
        var allCons = ItemDatabase.Instance.consumables;

        List<int> alreadies = new()
        {
            currentPlayer.dic_charSettings[currentPlayer.recentCharacter].itemOne,
            currentPlayer.dic_charSettings[currentPlayer.recentCharacter].itemTwo,
            currentPlayer.dic_charSettings[currentPlayer.recentCharacter].itemThree,
            currentPlayer.dic_charSettings[currentPlayer.recentCharacter].itemFour
        };

        for (int i = 0; i < allCons.Count; ++i)
        {
            var con = allCons[i];

            UI_ItemPrefab item = Instantiate(itemPrefab, transform_ConsumsDropdownContent).GetComponent<UI_ItemPrefab>();

            item.type = UI_ItemPrefab.Type.CONSUMS;
            item.id = i;

            item.GetComponent<Image>().sprite = con.sprite;
            if (con.description is not null && con.description.Length > 0)
            {
                item.description = con.description;
                item.go_descWindow = go_DescWindow;
                item.hasDesc = true;
            }

            if (alreadies.Contains(i))
            {
                item.isSelected = true;
                item.GetComponent<Image>().color = new Color(0, 0, 1f, 0.5f);
            }
            stringBuilder.Clear();
            stringBuilder.Append(con.itemName).Append(":");

            if (myCons.ContainsKey(con))
            {
                stringBuilder.Append(myCons[con]);
            }
            else
            {
                stringBuilder.Append(0);
            }
            item.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = stringBuilder.ToString();

            UI_ItemPrefab localItem = item;
            item.GetComponentInChildren<Button>().onClick.AddListener(() => {
                BtnOnClick_ItemPrefab(localItem);
            });
        }
        #endregion consum dropdown

        #region skill dropdown
        for (int i = transform_SkillsDropdownContent.childCount - 1; i >= 0; --i)
        {
            Destroy(transform_SkillsDropdownContent.GetChild(i).gameObject);
        }
        alreadies.Clear();
        alreadies.Add(currentPlayer.dic_charSettings[currentPlayer.recentCharacter].q);
        alreadies.Add(currentPlayer.dic_charSettings[currentPlayer.recentCharacter].w);
        alreadies.Add(currentPlayer.dic_charSettings[currentPlayer.recentCharacter].e);
        for (int i = 0; i < skills.Count; ++i)
        {
            Skill skill = skills[i];

            UI_ItemPrefab item = Instantiate(itemPrefab, transform_SkillsDropdownContent).GetComponent<UI_ItemPrefab>();

            item.type = UI_ItemPrefab.Type.SKILLS;
            item.id = i;

            item.GetComponent<Image>().sprite = skill.sprite;
            if (skill.description is not null && skill.description.Length > 0)
            {
                item.description = skill.description;
                item.go_descWindow = go_DescWindow;
                item.hasDesc = true;
            }

            if (alreadies.Contains(i))
            {
                item.isSelected = true;
                item.GetComponent<Image>().color = new Color(0, 0, 1f, 0.5f);
            }
            item.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = skill.skillName;

            UI_ItemPrefab localItem = item;
            item.GetComponentInChildren<Button>().onClick.AddListener(() => {
                BtnOnClick_ItemPrefab(localItem);
            });
        }
        #endregion skill dropdown
        //right end
    }

    public void UIUpdate_CraftWindowUpdate()
    {
        //target
        for (int i = transform_CraftTargetDropdownContent.childCount - 1; i >= 0; --i)
        {
            Destroy(transform_CraftTargetDropdownContent.GetChild(i).gameObject);
        }

        if (targetRecipe is null)
        {
            image_CraftTargetImage.sprite = null;
            image_CraftTargetImage.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "-";
        }
        else
        {
            image_CraftTargetImage.sprite = targetRecipe.result.sprite;
            image_CraftTargetImage.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = targetRecipe.result.itemName;

            var mats = targetRecipe.materials;

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < mats.Count; ++i)
            {
                stringBuilder.Clear();
                var mat = mats[i];
                TextMeshProUGUI matTMP = Instantiate(targetMaterialPrefab, transform_CraftTargetDropdownContent).GetComponentInChildren<TextMeshProUGUI>();
                stringBuilder.Append(mat.material.itemName).Append(":").Append(mat.count);
                matTMP.text = stringBuilder.ToString();
            }
        }
        //target end

        //recipes
        for (int i = transform_RecipesDropdownContent.childCount - 1; i >= 0; --i)
        {
            Destroy(transform_RecipesDropdownContent.GetChild(i).gameObject);
        }

        var recipes = ItemDatabase.Instance.recipes;

        for (int i = 0; i < recipes.Count; ++i)
        {
            var recipe = recipes[i];

            UI_ItemPrefab item = Instantiate(itemPrefab, transform_RecipesDropdownContent).GetComponent<UI_ItemPrefab>();

            item.type = UI_ItemPrefab.Type.RECIPES;
            item.id = i;

            item.GetComponent<Image>().sprite = recipe.result.sprite;
            item.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = recipe.result.itemName;
            if (recipe.result.description is not null && recipe.result.description.Length > 0)
            {
                item.description = recipe.result.description;
                item.go_descWindow = go_DescWindow;
                item.hasDesc = true;
            }

            UI_ItemPrefab localItem = item;
            item.GetComponentInChildren<Button>().onClick.AddListener(() => {
                BtnOnClick_ItemPrefab(localItem);
            });
        }
        //recipes end

        //mats
        for (int i = transform_MaterialsDropdownContent.childCount - 1; i >= 0; --i)
        {
            Destroy(transform_MaterialsDropdownContent.GetChild(i).gameObject);
        }

        var allMats = ItemDatabase.Instance.materials;

        for (int i = 0; i < allMats.Count; ++i)
        {
            var mat = allMats[i];
            UI_ItemPrefab item = Instantiate(itemPrefab, transform_MaterialsDropdownContent).GetComponent<UI_ItemPrefab>();

            item.type = UI_ItemPrefab.Type.NONE;
            item.id = i;

            item.GetComponent<Image>().sprite = mat.sprite;
            if (mat.description is not null && mat.description.Length > 0)
            {
                item.description = mat.description;
                item.go_descWindow = go_DescWindow;
                item.hasDesc = true;
            }

            int matCount;
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(mat.itemName).Append(":");
            if (LoginDataManager.Instance.currentPlayer.dic_itemMaterials.TryGetValue(mat, out matCount))
            {
                stringBuilder.Append(matCount);
            }
            else
            {
                stringBuilder.Append("0");
            }

            item.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = stringBuilder.ToString();
        }
        //mats end
    }

    public void UIUpdate_SaveWindowUpdate()
    {
        string folderPath = Path.Combine(Application.dataPath, "save");
        Transform imageTransform;
        Image image;
        if (!Directory.Exists(folderPath))
        {
            imageTransform = go_SaveSlotOne.transform.GetChild(0);
            image = imageTransform.GetComponent<Image>();
            image.sprite = null;
            image.color = Color.black;

            imageTransform = go_SaveSlotTwo.transform.GetChild(0);
            image = imageTransform.GetComponent<Image>();
            image.sprite = null;
            image.color = Color.black;

            imageTransform = go_SaveSlotThree.transform.GetChild(0);
            image = imageTransform.GetComponent<Image>();
            image.sprite = null;
            image.color = Color.black;
        }
        else
        {
            for (int i = 0; i < 3; ++i)
            {
                string filePath = Path.Combine(folderPath, $"savedata_{i}.csv");

                if (!File.Exists(filePath))
                {
                    switch (i)
                    {
                        case 0:
                            imageTransform = go_SaveSlotOne.transform.GetChild(0);
                            break;
                        case 1:
                            imageTransform = go_SaveSlotTwo.transform.GetChild(0);
                            break;
                        case 2:
                            imageTransform = go_SaveSlotThree.transform.GetChild(0);
                            break;
                        default:
                            throw new Exception("Invalid Save Slot!");
                    }
                    image = imageTransform.GetComponent<Image>();
                    image.sprite = null;
                    image.color = Color.black;
                }
                else
                {
                    PlayerData loaded = SaveLoadManager.LoadPlayerData(filePath);

                    switch (i)
                    {
                        case 0:
                            imageTransform = go_SaveSlotOne.transform.GetChild(0);
                            break;
                        case 1:
                            imageTransform = go_SaveSlotTwo.transform.GetChild(0);
                            break;
                        case 2:
                            imageTransform = go_SaveSlotThree.transform.GetChild(0);
                            break;
                        default:
                            throw new Exception("Invalid Save Slot!");
                    }
                    image = imageTransform.GetComponent<Image>();
                    image.sprite = CharacterDatabase.Instance.players[loaded.recentCharacter].sprite;
                    image.color = Color.white;
                }
            }
        }
    }

    public void UIUpdate_LoadWindowUpdate()
    {
        string folderPath = Path.Combine(Application.dataPath, "save");
        Transform imageTransform;
        Image image;
        if (!Directory.Exists(folderPath))
        {
            imageTransform = go_LoadSlotOne.transform.GetChild(0);
            image = imageTransform.GetComponent<Image>();
            image.sprite = null;
            image.color = Color.black;

            imageTransform = go_LoadSlotTwo.transform.GetChild(0);
            image = imageTransform.GetComponent<Image>();
            image.sprite = null;
            image.color = Color.black;

            imageTransform = go_LoadSlotThree.transform.GetChild(0);
            image = imageTransform.GetComponent<Image>();
            image.sprite = null;
            image.color = Color.black;
        }
        else
        {
            for (int i = 0; i < 3; ++i)
            {
                string filePath = Path.Combine(folderPath, $"savedata_{i}.csv");

                if (!File.Exists(filePath))
                {
                    switch (i)
                    {
                        case 0:
                            imageTransform = go_LoadSlotOne.transform.GetChild(0);
                            break;
                        case 1:
                            imageTransform = go_LoadSlotTwo.transform.GetChild(0);
                            break;
                        case 2:
                            imageTransform = go_LoadSlotThree.transform.GetChild(0);
                            break;
                        default:
                            throw new Exception("Invalid Save Slot!");
                    }
                    image = imageTransform.GetComponent<Image>();
                    image.sprite = null;
                    image.color = Color.black;
                }
                else
                {
                    PlayerData loaded = SaveLoadManager.LoadPlayerData(filePath);

                    switch (i)
                    {
                        case 0:
                            imageTransform = go_LoadSlotOne.transform.GetChild(0);
                            break;
                        case 1:
                            imageTransform = go_LoadSlotTwo.transform.GetChild(0);
                            break;
                        case 2:
                            imageTransform = go_LoadSlotThree.transform.GetChild(0);
                            break;
                        default:
                            throw new Exception("Invalid Save Slot!");
                    }
                    image = imageTransform.GetComponent<Image>();
                    image.sprite = CharacterDatabase.Instance.players[loaded.recentCharacter].sprite;
                    image.color = Color.white;
                }
            }
        }
    }

    public void BtnOnClick_DoCraft()
    {
        if (targetRecipe is null)
        {
            return;
        }

        var currentMaterials = LoginDataManager.Instance.currentPlayer.dic_itemMaterials;

        foreach (var item in targetRecipe.materials)
        {
            if (!currentMaterials.ContainsKey(item.material) || currentMaterials[item.material] < item.count)
            {
                return;
            }
        }

        foreach (var item in targetRecipe.materials)
        {
            currentMaterials[item.material] -= item.count;
            if (currentMaterials[item.material] <= 0)
            {
                currentMaterials.Remove(item.material);
            }
        }

        if (targetRecipe.result is Equipable)
        {
            Equipable result = targetRecipe.result as Equipable;
            switch (result.type)
            {
                case Equipable.Type.HELMET:
                    LoginDataManager.Instance.currentPlayer.helmets.Add(result.id);
                    break;
                case Equipable.Type.ARMOR:
                    LoginDataManager.Instance.currentPlayer.armors.Add(result.id);
                    break;
                case Equipable.Type.WEAPON:
                    LoginDataManager.Instance.currentPlayer.weapons.Add(result.id);
                    break;
            }
        }
        else if (targetRecipe.result is Consumable)
        {
            Consumable result = targetRecipe.result as Consumable;
            if (!LoginDataManager.Instance.currentPlayer.dic_consumables.ContainsKey(result))
            {
                LoginDataManager.Instance.currentPlayer.dic_consumables.Add(result, 1);
            }
            else
            {
                LoginDataManager.Instance.currentPlayer.dic_consumables[result] += 1;
            }
        }
        else
        {
            ItemMaterial result = targetRecipe.result as ItemMaterial;
            if (!LoginDataManager.Instance.currentPlayer.dic_itemMaterials.ContainsKey(result))
            {
                LoginDataManager.Instance.currentPlayer.dic_itemMaterials.Add(result, 1);
            }
            else
            {
                LoginDataManager.Instance.currentPlayer.dic_itemMaterials[result] += 1;
            }
        }

        UIUpdate_CharacterSettingsWindowUpdate();
        UIUpdate_CraftWindowUpdate();
    }

    public void BtnOnClick_Unequip(int index)
    {
        int nowPlayer = LoginDataManager.Instance.currentPlayer.recentCharacter;
        var settings = LoginDataManager.Instance.currentPlayer.dic_charSettings[nowPlayer];

        switch (index)
        {
            case 0:
                settings.head = -1;
                break;
            case 1:
                settings.armor = -1;
                break;
            case 2:
                settings.weapon = -1;
                break;
            default:
                throw new Exception("Invalid Equip Slot!");
        }

        UIUpdate_CharacterSettingsWindowUpdate();
    }

    public void BtnOnClick_RemoveConsumable(int index)
    {
        int nowPlayer = LoginDataManager.Instance.currentPlayer.recentCharacter;
        var settings = LoginDataManager.Instance.currentPlayer.dic_charSettings[nowPlayer];

        switch (index)
        {
            case 0:
                settings.itemOne = -1;
                break;
            case 1:
                settings.itemTwo = -1;
                break;
            case 2:
                settings.itemThree = -1;
                break;
            case 3:
                settings.itemFour = -1;
                break;
            default:
                throw new Exception("Invalid Consumable Slot Index!");
        }

        UIUpdate_CharacterSettingsWindowUpdate();
    }

    public void BtnOnClick_RemoveSkill(int index)
    {
        int nowPlayer = LoginDataManager.Instance.currentPlayer.recentCharacter;
        var settings = LoginDataManager.Instance.currentPlayer.dic_charSettings[nowPlayer];

        switch (index)
        {
            case 0:
                settings.q = -1;
                break;
            case 1:
                settings.w = -1;
                break;
            case 2:
                settings.e = -1;
                break;
            default:
                throw new Exception("Invalid Skill Slot Index!");
        }

        UIUpdate_CharacterSettingsWindowUpdate();
    }

    public void BtnOnClick_ItemPrefab(UI_ItemPrefab clicked)
    {
        var nowPlayer = LoginDataManager.Instance.currentPlayer;
        int nowCharacter = nowPlayer.recentCharacter;
        var settings = nowPlayer.dic_charSettings[nowCharacter];
        var cons = ItemDatabase.Instance.consumables;

        if (!clicked.isSelected)
        {
            switch (clicked.type)
            {
                case UI_ItemPrefab.Type.NONE:
                    break;
                case UI_ItemPrefab.Type.HELMETS:
                    settings.head = clicked.id;
                    UIUpdate_CharacterSettingsWindowUpdate();
                    break;
                case UI_ItemPrefab.Type.ARMORS:
                    settings.armor = clicked.id;
                    UIUpdate_CharacterSettingsWindowUpdate();
                    break;
                case UI_ItemPrefab.Type.WEAPONS:
                    settings.weapon = clicked.id;
                    UIUpdate_CharacterSettingsWindowUpdate();
                    break;
                case UI_ItemPrefab.Type.CONSUMS:
                    if (!nowPlayer.dic_consumables.ContainsKey(cons[clicked.id]) || nowPlayer.dic_consumables[cons[clicked.id]] <= 0)
                    {
                        return;
                    }
                    if (settings.itemOne == -1)
                    {
                        settings.itemOne = clicked.id;
                    }
                    else if (settings.itemTwo == -1)
                    {
                        settings.itemTwo = clicked.id;
                    }
                    else if (settings.itemThree == -1)
                    {
                        settings.itemThree = clicked.id;
                    }
                    else if (settings.itemFour == -1)
                    {
                        settings.itemFour = clicked.id;
                    }
                    UIUpdate_CharacterSettingsWindowUpdate();
                    break;
                case UI_ItemPrefab.Type.SKILLS:
                    if (settings.q == -1)
                    {
                        settings.q = clicked.id;
                    }
                    else if (settings.w == -1)
                    {
                        settings.w = clicked.id;
                    }
                    else if (settings.e == -1)
                    {
                        settings.e = clicked.id;
                    }
                    UIUpdate_CharacterSettingsWindowUpdate();
                    break;
                case UI_ItemPrefab.Type.RECIPES:
                    targetRecipe = ItemDatabase.Instance.recipes[clicked.id];
                    UIUpdate_CraftWindowUpdate();
                    break;
            }
        }
    }
}
