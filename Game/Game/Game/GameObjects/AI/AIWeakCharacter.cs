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
    class AIWeakCharacter : CharacterObject
    {

        protected override void OnLoad()
        {
            _color = new Vector4(0.0f, 1.0f, 0.0f, 1.0f);
            LoadModel("Player/Orc");

            isAlly = false;
            CharacterName = "AI Weak Character";
            HitPoints = 100;
            MaxHitPoints = 100;
            Damage = 30;
            ArmorDamage = 2;
            Armor = 0;
            MinArmor = 0;
            Range = 1;
            Movement = 3;

            PosX = 1;
            PosY = 2;
            Image = Content.Load<Texture2D>("OrcPic");
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
