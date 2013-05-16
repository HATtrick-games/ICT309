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
    class MainCharacter : CharacterObject
    {

       // public static readonly override int ImageID =
          //  CreateProperty<Microsoft.Xna.Framework.Graphics.Texture2D>("Character Image", "Common", "Defines the characters splash image.", Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("OrcPic")).Id;

        protected override void OnLoad()
        {
            LoadModel("Player/DudeMain");
            //LoadModel("Player/militia");
           // _model.ScaleLocal = new Vector3F(15.0f, 15.0f, 15.0f);

            isAlly = true;
            CharacterName = "Main Character";
            HitPoints = 350;
            MaxHitPoints = 350;
            Damage = 100;
            ArmorDamage = 3;
            Armor = 70;
            MinArmor = 30;
            Range = 1;
            Movement = 3;

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
