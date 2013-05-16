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
    class AIFinalBossCharacter : CharacterObject
    {
        protected override void OnLoad()
        {
            _color = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            LoadModel("Player/Golem");

            isAlly = false;
            CharacterName = "AI Final Boss Character";
            HitPoints = 1000;
            MaxHitPoints = 1000;
            Damage = 180;
            ArmorDamage = 10;
            Armor = 60;
            MinArmor = 60;
            Range = 1;
            Movement = 4;

            PosX = 3;
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
