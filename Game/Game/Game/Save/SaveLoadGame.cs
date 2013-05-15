using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using ICT309Game.GameObjects;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml.Serialization;

namespace ICT309Game.Save
{
    class SaveLoadGame
    {
        StorageDevice device;
        string containerName = "GamesStorage";
        string filename = "mysave.sav";

        [Serializable]
        public struct SaveGame
        {
            public int levelsCompleted;
            public List<CharacterObject> playerObjects;
        }

        private SaveGame currentSave;

        public void InitiateSave(int levels, List<CharacterObject> playerCharacters)
        {
            if (!Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.SaveToDevice, null);
            }

            currentSave = new SaveGame()
            {
                levelsCompleted = levels,
                playerObjects = playerCharacters,
            };
        }

        private void SaveToDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            if (device != null && device.IsConnected)
            {
                IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = device.EndOpenContainer(r);
                if (container.FileExists(filename))
                    container.DeleteFile(filename);
                Stream stream = container.CreateFile(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                serializer.Serialize(stream, currentSave);
                stream.Close();
                container.Dispose();
                result.AsyncWaitHandle.Close();
            }
        }

        private SaveGame currentLoad;

        public SaveGame InitiateLoad()
        {
            if (Guide.IsVisible)
            {
                device = null;
                StorageDevice.BeginShowSelector(PlayerIndex.One, this.LoadFromDevice, null);
            }

            return currentLoad;
        }

        private void LoadFromDevice(IAsyncResult result)
        {
            device = StorageDevice.EndShowSelector(result);
            IAsyncResult r = device.BeginOpenContainer(containerName, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = device.EndOpenContainer(r);
            result.AsyncWaitHandle.Close();
            if (container.FileExists(filename))
            {
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
                currentLoad = (SaveGame)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();
            }
        }
    }
}
