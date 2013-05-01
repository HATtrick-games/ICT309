using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DigitalRune.Game.UI;
using DigitalRune.Game.Input;
using Microsoft.Practices.ServiceLocation;

namespace ICT309Game.Game_Components
{
    class MainMenuComponent : GameComponent
    {
        private readonly IInputService _inputService;
        private readonly IUIService _uiService;

        public MainMenuComponent(Game game) : base(game)
        {
            _inputService = ServiceLocator.Current.GetInstance<IInputService>();
            _uiService = ServiceLocator.Current.GetInstance<IUIService>();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
