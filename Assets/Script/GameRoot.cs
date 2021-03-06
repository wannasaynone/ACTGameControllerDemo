﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour {

    private GameObject playerGO = null;

    private void Start () {
        GameManager.insance.CreateCharacter(0);
        GameManager.insance.CreateCharacter(1);
        GameManager.insance.CreateCharacter(2);
        GameManager.insance.CreateCharacter(3);
        GameManager.insance.CreateCharacter(4);
        GameManager.insance.CreateCharacter(5);
        playerGO = GameManager.insance.GetPlayer().GameObject;
    }

    private void Update()
    {
        Camera.main.transform.position = new Vector3(playerGO.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

}
