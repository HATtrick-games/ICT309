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
    class MainCharacter : CharacterObject
    {
        public MainCharacter()
        {
            isAlly = true;
        }

        protected override void OnLoad()
        {
            LoadModel("Player/Militia");
            _model.ScaleLocal = new Vector3F(15.0f, 15.0f, 15.0f);

            CharacterName = "Main Character";

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
