using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character  {

    public int ID { get; private set; }
    public int CurrentHP { get; private set; }
    public string PrefabName { get; private set; }
    public int MaxHP { get; private set; }
    public int Attack { get; private set; }
    public int Defend { get; private set; }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public Guid GUID { get; private set; }
    public Controller Controller { get; private set; }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public Character(int ID, int MaxHP, int Attack, int Defend, string PrefabName, Vector3 BornPosition)
    {
        this.ID = ID;
        this.MaxHP = MaxHP;
        CurrentHP = MaxHP;
        this.Attack = Attack;
        this.Defend = Defend;
        this.PrefabName = PrefabName;
        this.Controller = UnityEngine.Object.Instantiate(Resources.Load<Controller>(Const.CharacterControllerPath + PrefabName));
        GUID = Guid.NewGuid();
        Controller.SetGUID(GUID);
        Controller.transform.position = BornPosition;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public void TakeDamage(int Dmg)
    {
        CurrentHP -= Dmg;
    }

}
