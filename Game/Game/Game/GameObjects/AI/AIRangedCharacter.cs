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
    class AIRangedCharacter : CharacterObject
    {
        protected override void OnLoad()
        {
            _color = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            LoadModel("Player/EnemyCaster");

            isAlly = false;
            CharacterName = "AI Ranged Character";
            HitPoints = 160;
            MaxHitPoints = 160;
            Damage = 200;
            ArmorDamage = 3;
            Armor = 35;
            MinArmor = 10;
            Range = 4;
            Movement = 4;

            Image = Content.Load<Texture2D>("GolemPic");
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
