using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Player player;
    public bool isReady = false;
    void Update()
    {
        if (!isReady)
        {
            return;
        }

        if (player.character.isDead || player.character.isEnd)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            player.GetLeftClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            player.GetRightClick();
        }

        if (Input.GetButtonDown("Q"))
        {
            player.GetButton("q");
        }

        if (Input.GetButtonDown("W"))
        {
            player.GetButton("w");
        }

        if (Input.GetButtonDown("E"))
        {
            player.GetButton("e");
        }

        if (Input.GetButtonDown("R"))
        {
            player.GetButton("r");
        }

        if (Input.GetButtonDown("Space"))
        {
            player.GetButton("space");
        }

        if (Input.GetButtonDown("1"))
        {
            player.GetButton("1");
        }

        if (Input.GetButtonDown("2"))
        {
            player.GetButton("2");
        }

        if (Input.GetButtonDown("3"))
        {
            player.GetButton("3");
        }

        if (Input.GetButtonDown("4"))
        {
            player.GetButton("4");
        }

        //TODO: asdf zxcv enter escape f1~f5 g horizontal&vertical
    }
}
