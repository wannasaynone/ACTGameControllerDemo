using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager  {

    public CombatManager() { }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <returns>回傳傷害值</returns>
    public int NormalAttack(Character Attacker, Character Defender, bool isHeavyHit)
    {
        int dmg = (Attacker.Attack - Defender.Defend);
        dmg = dmg > 0 ? Random.Range(1, dmg + 1) : 1;

        Defender.TakeDamage(dmg);

        if (isHeavyHit)
        {
            if (Attacker.Controller.isFacingRight() && !Defender.Controller.isFacingRight()
                || !Attacker.Controller.isFacingRight() && Defender.Controller.isFacingRight())
                Defender.Controller.SetIsHeavyHurt(true);
            else
                Defender.Controller.SetIsHeavyHurt(false);
        }
        else
        {
            if (Attacker.Controller.isFacingRight() && !Defender.Controller.isFacingRight()
                || !Attacker.Controller.isFacingRight() && Defender.Controller.isFacingRight())
                Defender.Controller.SetIsHurt(true);
            else
                Defender.Controller.SetIsHurt(false);
        }

        if (Attacker.Controller.isFacingRight() && !Defender.Controller.isFacingRight()
            || !Attacker.Controller.isFacingRight() && Defender.Controller.isFacingRight())
        {
            if (Defender.CurrentHP <= 0)
                Defender.Controller.SetIsDie(true);
        }
        else
        {
            if (Defender.CurrentHP <= 0)
                Defender.Controller.SetIsDie(false);
        }

        return dmg;
    }

}
