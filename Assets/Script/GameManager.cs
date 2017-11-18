using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterSatate
{
    public int MaxHP;
    public int CurrentHP;
    public bool isInvincible;
    public GameObject GameObject;
}

public class GameManager  {

    public static GameManager insance
    {
        get
        {
            if (_instance == null)
                _instance = new GameManager();
            return _instance;
        }
    }

    private static GameManager _instance;

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private CombatManager combatManager;
    private List<Character> characters = new List<Character>();
    private GameObject hitEffect = Resources.Load<GameObject>("hit_effect");
    private Guid playerGuid;

    public GameManager()
    {
        combatManager = new CombatManager();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public GameObject CreateCharacter(int CharacterID)
    {
        if (characters.Count >= 2)
            characters = new List<Character>();//測試用->重Load遊戲後清空場景角色清單

        if(CharacterID == 0)                   //測試中，這邊應該要根據ID跟DB拿資料
        {
            Character newChar = new Character();
            newChar.Attack = 100;
            newChar.Defend = 90;
            newChar.MaxHP = 100;
            newChar.CurrentHP = newChar.MaxHP;
            newChar.PrefabName = "Player";
            newChar.Controller = UnityEngine.Object.Instantiate(Resources.Load<Controller>(Const.CharacterControllerPath + newChar.PrefabName));
            newChar.GUID = newChar.Controller.GetGUID();
            newChar.Controller.gameObject.transform.localPosition = new Vector3(-1, 0);
            characters.Add(newChar);
            playerGuid = newChar.GUID;
            return newChar.Controller.gameObject;
        }
        else
        {
            Character newChar = new Character();
            newChar.Attack = 100;
            newChar.Defend = 90;
            newChar.MaxHP = 100;
            newChar.CurrentHP = newChar.MaxHP;
            newChar.PrefabName = "AI";
            newChar.Controller = UnityEngine.Object.Instantiate(Resources.Load<Controller>(Const.CharacterControllerPath + newChar.PrefabName));
            newChar.GUID = newChar.Controller.GetGUID();
            newChar.Controller.gameObject.transform.localPosition = new Vector3(1, 0);
            characters.Add(newChar);
            return newChar.Controller.gameObject;
        }
    }

    public void Combat_Hit(GameObject Attacker, GameObject Defender, bool isHeavyHit)
    {
        Character attacker = characters.Find(x => x.Controller.gameObject == Attacker);
        Character defender = characters.Find(x => x.Controller.gameObject == Defender);

        if (attacker == null)
            return;
        if (defender == null)
            return;
        if (defender.Controller.IsInvincible)
            return;

        int defenderTakenDmg = combatManager.NormalAttack(attacker, defender);
        
        if(isHeavyHit)
        {
            if (attacker.Controller.isFacingRight() && !defender.Controller.isFacingRight()
                || !attacker.Controller.isFacingRight() && defender.Controller.isFacingRight())
                defender.Controller.SetIsHeavyHurt(true);
            else
                defender.Controller.SetIsHeavyHurt(false);
        }
        else
        {
            if (attacker.Controller.isFacingRight() && !defender.Controller.isFacingRight()
                || !attacker.Controller.isFacingRight() && defender.Controller.isFacingRight())
                defender.Controller.SetIsHurt(true);
            else
                defender.Controller.SetIsHurt(false);
        }

        if (attacker.Controller.isFacingRight() && !defender.Controller.isFacingRight()
            || !attacker.Controller.isFacingRight() && defender.Controller.isFacingRight())
        {
            if (defender.CurrentHP <= 0)
                defender.Controller.SetIsDie(true);
        }
        else
        {
            if (defender.CurrentHP <= 0)
                defender.Controller.SetIsDie(false);
        }
    }

    public void CreateHitEffect(Vector3 Posisiton)
    {
        GameObject hit = GameObject.Instantiate(hitEffect);
        hit.transform.position = Posisiton;
    }

    public CharacterSatate GetCharacterState(Guid GUID)
    {
        Character target = characters.Find(x => x.GUID == GUID);
        CharacterSatate state = new CharacterSatate();
        if (target != null)
        {
            state.MaxHP = target.MaxHP;
            state.CurrentHP = target.CurrentHP;
            state.isInvincible = target.Controller.IsInvincible;
            state.GameObject = target.Controller.gameObject;
        }
        return state;
    }

    public Guid GetPlayerGuid()
    {
        return playerGuid;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

}
