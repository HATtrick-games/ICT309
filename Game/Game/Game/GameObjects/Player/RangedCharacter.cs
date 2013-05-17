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
using Microsoft.Xna.Framework.Graphics;


namespace ICT309Game.GameObjects
{
    class RangedCharacter : CharacterObject
    {

        protected override void OnLoad()
        {
            importance = 7;
            LoadModel("Player/Archer");

            isAlly = true;
            CharacterName = "Archer";
            HitPoints = 200;
            MaxHitPoints = 200;
            Damage = 70;
            ArmorDamage = 8;
            Armor = 45;
            MinArmor = 15;
            Range = 5;
            Movement = 5;

            Image = Content.Load<Texture2D>("ArcherPic");
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
