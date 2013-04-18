using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ICT309Game.Screens
{
    class GameScreen: IScreen
    {
        private readonly GameManager _game;

        public GameScreen(Game game):base()
        {
            _game = (GameManager)game;
        }

        public override void Initialize()
        {
            Console.WriteLine("GameScreen");
            base.Initialize();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(TimeSpan gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(TimeSpan gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
