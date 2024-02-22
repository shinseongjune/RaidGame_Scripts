using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player
{
    public int id;

    public string playerName;

    public CharacterControlComponent character;

    public InputHandler inputHandler;

    public bool isSkillIndicatorOn = false;

    //TODO:메뉴화면 등에서 인풋 분리. enum state?

    public void GetLeftClick()
    {
        if (!isSkillIndicatorOn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI", "MouseInputCollider")))
            {
                int hitLayer = hit.transform.gameObject.layer;

                if (hitLayer == LayerMask.NameToLayer("UI"))
                {

                }
                else if (hitLayer == LayerMask.NameToLayer("MouseInputCollider"))
                {
                    character.GetLeftClick(hit.point);
                }
            }
        }
    }

    public void GetRightClick()
    {
        if (!isSkillIndicatorOn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("UI", "MouseInputCollider")))
            {
                int hitLayer = hit.transform.gameObject.layer;

                if (hitLayer == LayerMask.NameToLayer("UI"))
                {

                }
                else if (hitLayer == LayerMask.NameToLayer("MouseInputCollider"))
                {
                    character.GetRightClick(hit.point);
                }
            }
        }
    }

    public void GetButton(string button)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("MouseInputCollider")))
        {
            switch (button)
            {
                case "q":
                case "w":
                case "e":
                case "r":
                    character.GetSkillButton(hit.point, button);
                    break;
                case "space":
                    character.GetSpaceBar(hit.point);
                    break;
                case "1":
                case "2":
                case "3":
                case "4":
                    character.GetItemButton(hit.point, button);
                    break;
            }
        }
    }
}
