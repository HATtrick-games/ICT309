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
    class MeleeFighterCharacter : CharacterObject
    {

        protected override void OnLoad()
        {
            importance = 4;
            LoadModel("Player/DudeMain");
            // LoadModel("Player/rangedally");

            isAlly = true;
            CharacterName = "Spearman";
            HitPoints = 310;
            MaxHitPoints = 310;
            Damage = 150;
            ArmorDamage = 5;
            Armor = 35;
            MinArmor = 15;
            Range = 1;
            Movement = 5;

            Image = Content.Load<Texture2D>("SpearmanPic");
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
