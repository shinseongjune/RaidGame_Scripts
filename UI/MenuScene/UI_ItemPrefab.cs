using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_ItemPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Type
    {
        HELMETS,
        ARMORS,
        WEAPONS,
        CONSUMS,
        SKILLS,
        RECIPES,
        NONE
    }

    public Type type;
    public int id;

    public bool isSelected;

    public bool hasDesc;
    public string description;
    public GameObject go_descWindow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hasDesc)
        {
            Vector2 pos;
            Vector2 mousePos = eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)go_descWindow.transform.parent, mousePos, eventData.pressEventCamera, out pos);

            go_descWindow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = description;
            go_descWindow.GetComponent<RectTransform>().localPosition = pos;
            go_descWindow.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hasDesc)
        {
            go_descWindow.SetActive(false);
        }
    }
}
