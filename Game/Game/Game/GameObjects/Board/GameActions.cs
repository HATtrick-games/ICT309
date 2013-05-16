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
        

        public GameActions()
        {
            //Pather = new PathFinder();
        }

       /* public void AnimateCombat(CharacterObject attacker, CharacterObject defender)
        {
            if (attacker.PosX - defender.PosX < 0)
            {
                attacker.Rotation(90);
            }
            else if (attacker.PosX - defender.PosX > 0)
            {
                attacker.Rotation(270);
            }
            else if (attacker.PosY - defender.PosY > 0)
            {
                attacker.Rotation(180);
            }
            else if (attacker.PosY - defender.PosY < 0)
            {
                attacker.Rotation(360);
            }
        }*/

        public static bool ResolveCombat(CharacterObject attacker, CharacterObject defender)
        {
            if (attacker.isAlly == defender.isAlly)
                return false;

            /**********ATTACK ANIMATION*********/
            if (attacker.PosX - defender.PosX < 0)
            {
                attacker.Rotation(90);
            }
            else if (attacker.PosX - defender.PosX > 0)
            {
                attacker.Rotation(270);
            }
            else if (attacker.PosY - defender.PosY > 0)
            {
                attacker.Rotation(180);
            }
            else if (attacker.PosY - defender.PosY < 0)
            {
                attacker.Rotation(360);
            }

            attacker.Attack();
            /******************************************/

            int totalDamage = attacker.Damage - ((attacker.Damage * defender.Armor) / 100);
            
            // Armor Break
            defender.Armor -= attacker.ArmorDamage;
            defender.Armor = (int)MathHelper.Max((float)defender.Armor, (float)defender.MinArmor);

            defender.HitPoints -= totalDamage; 

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            gameLog.AddMessage(attacker.CharacterName + " attacked " + defender.CharacterName + " dealing " + totalDamage + " damage.");

            return true;
        }

        public static void MovePattern(CharacterObject character, int posX, int posY)
        {

           // Pather.Intiialise();
           // Pather.FindPath(character.PosX, character.PosY, posX, posY);
        }

        public static bool MoveCharacter(CharacterObject character, int posX, int posY)
        {
            if (posX >= 10 || posX < 0) return false;
            if (posY >= 10 || posY < 0) return false;

            MovePattern(character, posX, posY);
            character.PosX = posX;
            character.PosY = posY;

            var gameLog = ServiceLocator.Current.GetInstance<GameLog>();
            gameLog.AddMessage(character.CharacterName + " moved to " + "(" + posX + ", " + posY + ")");

            return true;
        }
    }
}
