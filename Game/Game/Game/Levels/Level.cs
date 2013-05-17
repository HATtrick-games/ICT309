using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Graphics.SceneGraph;
using ICT309Game.GameObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Practices.ServiceLocation;

namespace ICT309Game.Levels
{
    class Level
    {
        public string _levelModel { get; protected set; }
        public string _backgroundSong { get; protected set; }
        public bool[,] _boardData { get; protected set; }
        public List<CharacterObject> _characters { get; protected set; }
        public int NumberOfTraps;
        public List<int> ObstructedX = new List<int>();
        public List<int> ObstructedY = new List<int>();

        public Level()
        {
            _levelModel = null;
            _backgroundSong = null;
            _boardData = null;
            _characters = null;
        }
    }
}
