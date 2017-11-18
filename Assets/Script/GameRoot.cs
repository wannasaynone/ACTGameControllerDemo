using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour {

    private GameObject playerGO = null;

    private void Start () {
        GameManager.insance.CreateCharacter(0);
        GameManager.insance.CreateCharacter(1);
        playerGO = GameManager.insance.GetCharacterState(GameManager.insance.GetPlayerGuid()).GameObject;
    }

    private void Update()
    {
        Camera.main.transform.position = new Vector3(playerGO.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
    }

}
