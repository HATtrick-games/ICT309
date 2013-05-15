using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ICT309Game.Game_Components
{
    class StartScreenComponent : DrawableGameComponent
    {
        private Texture2D _splashScreen;
        private int timing = 0;

        public StartScreenComponent(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();

            _splashScreen = content.Load<Texture2D>("Screens/splashscreen");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            timing += gameTime.ElapsedGameTime.Milliseconds;

            if (timing > 2000)
            {
                Game.Components.Remove(this);
                Game.Components.Add(new MainMenuComponent(Game));
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ServiceLocator.Current.GetInstance<SpriteBatch>();

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(_splashScreen, Vector2.Zero, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
