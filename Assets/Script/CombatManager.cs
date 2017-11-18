using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager  {

    public CombatManager() { }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <returns>回傳傷害值</returns>
    public int NormalAttack(Character Attacker, Character Defender)
    {
        int dmg = (Attacker.Attack - Defender.Defend);
        dmg = dmg > 0 ? dmg : 0;

        return dmg;
    }

}
