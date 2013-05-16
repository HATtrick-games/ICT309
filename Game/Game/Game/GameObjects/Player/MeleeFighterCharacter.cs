using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;
using DigitalRune.Graphics.SceneGraph;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;
using DigitalRune.Graphics;
using ICT309Game.Graphics;
using Microsoft.Xna.Framework;
using DigitalRune.Graphics.Effects;
using DigitalRune.Mathematics.Algebra;
using DigitalRune.Animation;


namespace ICT309Game.GameObjects
{
    class MeleeFighterCharacter : CharacterObject
    {

        protected override void OnLoad()
        {
            LoadModel("Player/DudeMain");
            // LoadModel("Player/rangedally");

            isAlly = true;
            CharacterName = "Melee Fighter Character";
            HitPoints = 310;
            Damage = 150;
            ArmorDamage = 5;
            Armor = 35;
            MinArmor = 15;
            Range = 1;
            Movement = 5;

            PosX = 2;
            PosY = 1;

            base.OnLoad();
        }

        protected override void OnUnload()
        {

            base.OnUnload();
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            base.OnUpdate(deltaTime);
        }
    }
}
