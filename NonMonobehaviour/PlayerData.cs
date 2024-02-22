using Item;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyCharacterSettings
{
    public int basic = 0;

    public int q = 0;
    public int w = 1;
    public int e = 2;

    public int head = -1;
    public int armor = -1;
    public int weapon = -1;
    
    public int itemOne = -1;
    public int itemTwo = -1;
    public int itemThree = -1;
    public int itemFour = -1;
}

public class PlayerData
{
    //string id
    //string name tag

    public List<int> helmets = new List<int>();
    public List<int> armors = new List<int>();
    public List<int> weapons = new List<int>();
    public Dictionary<Consumable, int> dic_consumables = new Dictionary<Consumable, int>();
    public Dictionary<ItemMaterial, int> dic_itemMaterials = new Dictionary<ItemMaterial, int>();

    public int recentCharacter;
    public Dictionary<int, MyCharacterSettings> dic_charSettings = new Dictionary<int, MyCharacterSettings>();

    public int recentBoss;
}

#region Serializations
#region Serializable Classes
[Serializable]
public class ConsumableEntry
{
    public Consumable key;
    public int value;
}

[Serializable]
public class ItemMaterialEntry
{
    public ItemMaterial key;
    public int value;
}

[Serializable]
public class MyCharacterSettingsEntry
{
    public int key;
    public MyCharacterSettings value;
}

[Serializable]
public class SerializablePlayerData
{
    public List<int> helmets = new List<int>();
    public List<int> armors = new List<int>();
    public List<int> weapons = new List<int>();
    public List<ConsumableEntry> consumables = new List<ConsumableEntry>();
    public List<ItemMaterialEntry> itemMaterials = new List<ItemMaterialEntry>();

    public int recentCharacter;
    public List<MyCharacterSettingsEntry> charSettings = new List<MyCharacterSettingsEntry>();

    public int recentBoss;
}
#endregion Serializable Classes
public static class SaveLoadManager
{
    #region Serialize Functions
    static SerializablePlayerData ConvertToSerializablePlayerData(PlayerData data)
    {
        SerializablePlayerData serialized = new SerializablePlayerData();

        serialized.recentCharacter = data.recentCharacter;
        serialized.recentBoss = data.recentBoss;
        serialized.helmets = new(data.helmets);
        serialized.armors = new(data.armors);
        serialized.weapons = new(data.weapons);
        
        foreach(var item in data.dic_consumables)
        {
            ConsumableEntry entry = new ConsumableEntry();
            entry.key = item.Key;
            entry.value = item.Value;

            serialized.consumables.Add(entry);
        }

        foreach (var item in data.dic_itemMaterials)
        {
            ItemMaterialEntry entry = new ItemMaterialEntry();
            entry.key = item.Key;
            entry.value = item.Value;

            serialized.itemMaterials.Add(entry);
        }

        foreach (var item in data.dic_charSettings)
        {
            MyCharacterSettingsEntry entry = new MyCharacterSettingsEntry();
            entry.key = item.Key;
            entry.value = item.Value;

            serialized.charSettings.Add(entry);
        }

        return serialized;
    }

    static PlayerData ConvertToPlayerData(SerializablePlayerData serialized)
    {
        PlayerData playerData = new PlayerData();

        playerData.recentCharacter = serialized.recentCharacter;
        playerData.recentBoss = serialized.recentBoss;
        playerData.helmets = new(serialized.helmets);
        playerData.armors = new(serialized.armors);
        playerData.weapons = new(serialized.weapons);

        foreach(var item in serialized.consumables)
        {
            Consumable consumable = item.key;
            int value = item.value;

            playerData.dic_consumables.Add(consumable, value);
        }

        foreach (var item in serialized.itemMaterials)
        {
            ItemMaterial material = item.key;
            int value = item.value;

            playerData.dic_itemMaterials.Add(material, value);
        }

        foreach (var item in serialized.charSettings)
        {
            int key = item.key;
            MyCharacterSettings value = item.value;

            playerData.dic_charSettings.Add(key, value);
        }

        return playerData;
    }
    #endregion Serialize Functions

    public static void SavePlayerData(PlayerData data, string filePath)
    {
        SerializablePlayerData serialized = ConvertToSerializablePlayerData(data);
        string json = JsonUtility.ToJson(serialized);

        System.IO.File.WriteAllText(filePath, json);
    }

    public static PlayerData LoadPlayerData(string filePath)
    {
        string json = System.IO.File.ReadAllText(filePath);

        SerializablePlayerData serialized = JsonUtility.FromJson<SerializablePlayerData>(json);

        PlayerData playerData = ConvertToPlayerData(serialized);

        return playerData;
    }
}
#endregion Serializations