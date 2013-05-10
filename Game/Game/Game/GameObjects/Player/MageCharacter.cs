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
    class MageCharacter : CharacterObject
    {

        protected override void OnLoad()
        {
            LoadModel("Player/rangedally");

            isAlly = true;
            CharacterName = "Mage Character";
            HitPoints = 200;
            MaxHitPoints = 200;
            Damage = 75;
            ArmorDamage = 0;
            Armor = 30;
            MinArmor = 5;
            Range = 4;
            Movement = 4;

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
