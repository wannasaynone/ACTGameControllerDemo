using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour {

    [SerializeField]
    Camera CameraF;//TEST
    public static GameObject PLAYER;//TEST

    // Use this for initialization
    void Start () {
        PLAYER = GameManager.insance.CreateCharacter(0);
        GameManager.insance.CreateCharacter(1);
    }

    private void Update()
    {
        CameraF.transform.position = new Vector3(PLAYER.transform.position.x, CameraF.transform.position.y, CameraF.transform.position.z);
        Debug.DrawLine(PLAYER.transform.position, Vector3.zero);
    }

}
