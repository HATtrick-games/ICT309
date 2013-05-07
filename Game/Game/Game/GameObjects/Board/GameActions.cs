using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICT309Game.GameObjects;
using Microsoft.Xna.Framework;

namespace ICT309Game.GameObjects.Board
{
    class GameActions
    {
        public static bool ResolveCombat(CharacterObject attacker, CharacterObject defender)
        {
            if (attacker.isAlly == defender.isAlly)
                return false;

            int totalDamage = attacker.Damage - ((attacker.Damage * defender.Armor) / 100);
            
            // Armor Break
            defender.Armor -= attacker.ArmorDamage;
            defender.Armor = (int)MathHelper.Max((float)defender.Armor, (float)defender.MinArmor);

            defender.HitPoints -= totalDamage;

            return true;
        }
    }
}
