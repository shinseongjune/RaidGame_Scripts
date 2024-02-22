using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSlot
{
    public Consumable item;
    public float cooldown = 0;

    public int count = 0;
}

public class ItemSlots : MonoBehaviour
{
    PlayerData playerData;

    public Dictionary<string, ItemSlot> slots = new();

    public Transform firePoint;

    public ItemSlot one
    {
        get { return slots.TryGetValue("1", out ItemSlot value) ? value : null; }
    }

    public ItemSlot two
    {
        get { return slots.TryGetValue("2", out ItemSlot value) ? value : null; }
    }

    public ItemSlot three
    {
        get { return slots.TryGetValue("3", out ItemSlot value) ? value : null; }
    }

    public ItemSlot four
    {
        get { return slots.TryGetValue("4", out ItemSlot value) ? value : null; }
    }

    public void AssignItem()
    {
        ItemSlot oneSlot = new();
        ItemSlot twoSlot = new();
        ItemSlot threeSlot = new();
        ItemSlot fourSlot = new();

        ItemDatabase itemDB = ItemDatabase.Instance;
        PlayerData data = LoginDataManager.Instance.currentPlayer;

        int oneIndex = data.dic_charSettings[data.recentCharacter].itemOne;
        int twoIndex = data.dic_charSettings[data.recentCharacter].itemTwo;
        int threeIndex = data.dic_charSettings[data.recentCharacter].itemThree;
        int fourIndex = data.dic_charSettings[data.recentCharacter].itemFour;

        if (oneIndex is not -1)
        {
            int oneCount = data.dic_consumables[itemDB.consumables[oneIndex]];
            Consumable con = itemDB.consumables[data.dic_charSettings[data.recentCharacter].itemOne];
            oneSlot.item = con;

            if (oneCount >= con.maxCount)
            {
                oneSlot.count = con.maxCount;
            }
            else
            {
                oneSlot.count = oneCount;
            }
        }
        slots.Add("1", oneSlot);

        if (twoIndex is not -1)
        {
            int twoCount = data.dic_consumables[itemDB.consumables[twoIndex]];
            Consumable con = itemDB.consumables[data.dic_charSettings[data.recentCharacter].itemTwo];
            twoSlot.item = con;

            if (twoCount >= con.maxCount)
            {
                twoSlot.count = con.maxCount;
            }
            else
            {
                twoSlot.count = twoCount;
            }
        }
        slots.Add("2", twoSlot);

        if (threeIndex is not -1)
        {
            int threeCount = data.dic_consumables[itemDB.consumables[twoIndex]];
            Consumable con = itemDB.consumables[data.dic_charSettings[data.recentCharacter].itemThree];
            threeSlot.item = con;

            if (threeCount >= con.maxCount)
            {
                threeSlot.count = con.maxCount;
            }
            else
            {
                threeSlot.count = threeCount;
            }
        }
        slots.Add("3", threeSlot);

        if (fourIndex is not -1)
        {
            int fourCount = data.dic_consumables[itemDB.consumables[fourIndex]];
            Consumable con = itemDB.consumables[data.dic_charSettings[data.recentCharacter].itemThree];
            fourSlot.item = con;

            if (fourCount >= con.maxCount)
            {
                fourSlot.count = con.maxCount;
            }
            else
            {
                fourSlot.count = fourCount;
            }
        }
        slots.Add("4", fourSlot);
    }

    private void Awake()
    {
        playerData = LoginDataManager.Instance.currentPlayer;
    }

    void Update()
    {
        foreach (var slot in slots.Values)
        {
            if (slot.cooldown > 0)
            {
                slot.cooldown = Mathf.Max(slot.cooldown - Time.deltaTime, 0);
            }
        }
    }

    //TODO: 플레이어 인벤토리에서 사용할 때마다 아이템 소모
    public bool UseItem(string input, Vector3 point)
    {
        ItemSlot slot;

        switch (input)
        {
            case "1":
                slot = one;
                break;
            case "2":
                slot = two;
                break;
            case "3":
                slot = three;
                break;
            case "4":
                slot = four;
                break;
            default:
                throw new System.Exception("invalid item number input!");
        }

        if (slot == null || slot.item == null || slot.count == 0 || slot.cooldown > 0)
        {
            return false;
        }

        Consumable item = slot.item;

        slot.cooldown = item.coolDown;
        --slot.count;
        //TODO: slot.count == 0이면 UI 수정.

        Vector3 itemPosition;

        switch (item.type)
        {
            case Consumable.Type.THROW:
                itemPosition = firePoint == null ? transform.position + Vector3.up : firePoint.position;

                //TODO: Instantiate
                //TODO: ItemBase 만들어서 프리팹 만들고 구현.
                ThrowableBase throwable = Instantiate(item.itemPrefab, itemPosition, transform.rotation).GetComponent<ThrowableBase>();
                throwable.owner = gameObject;
                throwable.source = item;
                throwable.startPosition = itemPosition;
                throwable.endPosition = point;
                throwable.GetOn();
                break;
            case Consumable.Type.INSTANT:
                itemPosition = transform.position;

                //TODO: ItemBase 만들어서 프리팹 만들고 구현.
                ItemBase instant = Instantiate(item.itemPrefab, itemPosition, transform.rotation).GetComponent<ItemBase>();

                instant.owner = gameObject;
                instant.source = item;
                instant.GetOn();
                break;
        }

        playerData.dic_consumables[item] -= 1;

        return true;
    }
}