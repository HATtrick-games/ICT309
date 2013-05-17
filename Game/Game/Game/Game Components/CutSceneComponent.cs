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
    class CutSceneComponent : DrawableGameComponent
    {
        Video cutScene;
        VideoPlayer player;
        Texture2D videoTexture;

        int videoTimer;

        public CutSceneComponent(Game game)
            : base(game)
        {
            videoTimer = 0;
        }

        protected override void LoadContent()
        {
            var content = ServiceLocator.Current.GetInstance<ContentManager>();

            cutScene = content.Load<Video>("Cutscenes/Cutscene");
            player = new VideoPlayer();

            base.LoadContent();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

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
