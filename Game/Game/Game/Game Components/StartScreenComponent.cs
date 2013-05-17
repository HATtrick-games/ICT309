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
    /// <summary> Start screen component.</summary>
    class StartScreenComponent : DrawableGameComponent
    {
        private Texture2D _splashScreen;
        private int timing = 0;

        /// <summary> Constructor.</summary>
        ///
        /// <param name="game"> The game.</param>
        public StartScreenComponent(Game game)
            : base(game)
        {

        }

        /// <summary> Called when graphics resources need to be loaded. Override this
        ///           method to load any component-specific graphics resources.</summary>
        protected override void LoadContent()
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();

            _splashScreen = content.Load<Texture2D>("Screens/splashscreen");

            base.LoadContent();
        }

        /// <summary> Called when the GameComponent needs to be updated. Override this
        ///           method with component-specific update code.</summary>
        ///
        /// <param name="gameTime"> Time elapsed since the last call to Update.</param>
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

        /// <summary> Called when the DrawableGameComponent needs to be drawn. Override
        ///           this method with component-specific drawing code. Reference
        ///           page contains links to related conceptual articles.</summary>
        ///
        /// <param name="gameTime"> Time passed since the last call to Draw.</param>
        ///
        /// ### <param name="gameTime"> Time elapsed since the last call to Update.</param>
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
