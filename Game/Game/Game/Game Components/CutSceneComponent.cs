using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework.Content;

namespace ICT309Game.Game_Components
{
    /// <summary> Cut scene component.</summary>
    class CutSceneComponent : DrawableGameComponent
    {
        Video cutScene;
        VideoPlayer player;
        Texture2D videoTexture;

        int videoTimer;

        /// <summary> Constructor.</summary>
        ///
        /// <param name="game"> The game.</param>
        public CutSceneComponent(Game game)
            : base(game)
        {
            videoTimer = 0;
        }

        /// <summary> Called when graphics resources need to be loaded. Override this
        ///           method to load any component-specific graphics resources.</summary>
        protected override void LoadContent()
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();

            cutScene = content.Load<Video>("Cutscenes/Cutscene");
            player = new VideoPlayer();

            base.LoadContent();
        }

        /// <summary> Releases the unmanaged resources used by the
        ///           DrawableGameComponent and optionally releases the managed
        ///           resources.</summary>
        ///
        /// <param name="disposing"> true to release both managed and unmanaged
        ///                          resources; false to release only unmanaged
        ///                          resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        /// <summary> Called when the GameComponent needs to be updated. Override this
        ///           method with component-specific update code.</summary>
        ///
        /// <param name="gameTime"> Time elapsed since the last call to Update.</param>
        public override void Update(GameTime gameTime)
        {
            if (player.State == MediaState.Stopped)
            {
                player.IsLooped = false;
                player.Play(cutScene);                
            } 
            else if (player.State == MediaState.Playing)
            {
                videoTimer += gameTime.ElapsedGameTime.Milliseconds;
            }

            if (videoTimer > cutScene.Duration.TotalMilliseconds)
            {
                Game.Components.Remove(this);
                Game.Components.Add(new MainGameComponent(Game));
                Dispose(true);
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

            if (player.State != MediaState.Stopped)
            {
                videoTexture = player.GetTexture();
            }

            Rectangle screen = new Rectangle(GraphicsDevice.Viewport.X,
                GraphicsDevice.Viewport.Y,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height);

            if (videoTexture != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(videoTexture, screen, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
