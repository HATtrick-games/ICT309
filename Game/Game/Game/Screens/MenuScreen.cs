using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Screens
{
    class MenuScreen: IScreen
    {
        public override void Initialize()
        {
            Console.WriteLine("Menu Screen");

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
