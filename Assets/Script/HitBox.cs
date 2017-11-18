using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    [SerializeField]
    private Controller Attacker = null;
    [SerializeField]
    private bool isHeavyHit = false;

    private void Awake()
    {
        if (Attacker == null)
            Debug.LogError("Attack == null");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Controller defender = collision.gameObject.GetComponent<Controller>();
        if(defender)
            GameManager.insance.Combat_Hit(Attacker.GUID, defender.GUID, isHeavyHit);
    }
}
