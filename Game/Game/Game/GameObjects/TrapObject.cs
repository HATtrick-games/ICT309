using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game;

namespace ICT309Game.GameObjects
{
    class TrapObject : GameObject
    {
        public static readonly int PosXID =
            CreateProperty<int>("Position X", "Common", "Defines the X position of the trap.", 0).Id;

        public static readonly int PosYID =
            CreateProperty<int>("Position Y", "Common", "Defines the Y position of the trap.", 0).Id;

        public static readonly int DamageID =
            CreateProperty("Damage", "Common", "The Amount of damage the trap does.", 0).Id;

        public int PosX
        {
            get { return GetValue<int>(PosXID); }
            set { SetValue(PosXID, value); }
        }

        public int PosY
        {
            get { return GetValue<int>(PosYID); }
            set { SetValue(PosYID, value); }
        }

        public int Damage
        {
            get { return GetValue<int>(DamageID); }
            private set { SetValue(DamageID, value); }
        }

        public TrapObject(int posX, int posY, int damage)
        {
            PosX = posX;
            PosY = posY;
            Damage = damage;
        }

        public TrapObject()
        {
            Random r = new Random();

            PosX = r.Next(0, 10);
            PosY = r.Next(0, 10);
            Damage = r.Next(10, 100);
        }
    }
}
