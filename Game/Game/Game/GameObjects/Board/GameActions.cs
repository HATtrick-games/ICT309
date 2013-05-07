using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICT309Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Practices.ServiceLocation;
using ICT309Game.Game_Components.UI;

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

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            gameLog.AddMessage(attacker.CharacterName + " attacked " + defender.CharacterName + " dealing " + totalDamage + " damage.");

            return true;
        }

        public static bool MoveCharacter(CharacterObject character, int posX, int posY)
        {
            if (posX >= 10 || posX < 0) return false;
            if (posY >= 10 || posY < 0) return false;

            character.PosX = posX;
            character.PosY = posY;

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            gameLog.AddMessage(character.CharacterName + " moved to " + "(" + posX + ", " + posY + ")");

            return true;
        }
    }
}
