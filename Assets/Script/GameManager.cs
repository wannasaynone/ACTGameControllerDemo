using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterState
{
    public int MaxHP { private set; get; }
    public int CurrentHP { private set; get; }
    public int Attack { private set; get; }
    public int Defend { private set; get; }
    public bool isInvincible { private set; get; }
    public GameObject GameObject { private set; get; }

    public CharacterState(Character Character)
    {
        MaxHP = Character.MaxHP;
        CurrentHP = Character.CurrentHP;
        Attack = Character.Attack;
        Defend = Character.Defend;
        isInvincible = Character.Controller.IsInvincible;
        GameObject = Character.Controller.gameObject;
    }
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

    public GameManager()
    {
        combatManager = new CombatManager();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    private List<Character> characters = new List<Character>();
    private GameObject hitEffect = Resources.Load<GameObject>("hit_effect");
    private Guid playerGuid;

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public GameObject CreateCharacter(int CharacterID)
    {
        if(CharacterID == 0)                   //測試中，這邊應該要根據ID跟DB拿資料
        {                                      
            Character newChar = new Character(
                CharacterID,
                100,
                100,
                90,
                "Player",
                new Vector3(-1, 0));
            characters.Add(newChar);
            playerGuid = newChar.GUID;//測試中，玩家的產生應該要另外開一個API
            return newChar.Controller.gameObject;
        }
        else
        {
            Character newChar = new Character(
                CharacterID,
                100,
                100,
                90,
                "AI",
                new Vector3(CharacterID, 0));
            characters.Add(newChar);
            return newChar.Controller.gameObject;
        }
    }

    public void Combat_Hit(Guid Attacker, Guid Defender, bool isHeavyHit)
    {
        Character attacker = characters.Find(x => x.GUID == Attacker);
        Character defender = characters.Find(x => x.GUID == Defender);
        CharacterState attackerState = new CharacterState(attacker);
        CharacterState defenderState = new CharacterState(defender);

        if (attacker == null)
            return;
        if (defender == null)
            return;
        if (defender.Controller.IsInvincible)
            return;

        int defenderTakenDmg = combatManager.NormalAttack(attacker, defender, isHeavyHit);
        Debug.Log(defenderState.GameObject.name + " Take " + defenderTakenDmg + " dmg");
    }

    public void CreateHitEffect(Vector3 Posisiton)
    {
        GameObject hit = UnityEngine.Object.Instantiate(hitEffect);
        hit.transform.position = Posisiton;
    }

    public CharacterState GetCharacterState(Guid GUID)
    {
        Character target = characters.Find(x => x.GUID == GUID);
        CharacterState state = new CharacterState(target);
        return state;
    }

    public CharacterState GetPlayer()
    {
        return GetCharacterState(playerGuid);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

}
