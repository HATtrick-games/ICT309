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
    class AIRangedCharacter : CharacterObject
    {
        protected override void OnLoad()
        {
            _color = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            LoadModel("AI/rangedenemy");

            isAlly = false;
            CharacterName = "AI Mini Boss Character";
            HitPoints = 650;
            Damage = 60;
            ArmorDamage = 5;
            Armor = 45;
            MinArmor = 10;
            Range = 6;
            Movement = 3;

            PosX = 3;

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
