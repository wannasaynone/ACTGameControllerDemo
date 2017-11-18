using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager  {

    public CombatManager() { }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <returns>回傳傷害值</returns>
    public int NormalAttack(CharacterState Attacker, CharacterState Defender)
    {
        int dmg = (Attacker.Attack - Defender.Defend);
        dmg = dmg > 0 ? dmg : 0;

        return dmg;
    }

}
