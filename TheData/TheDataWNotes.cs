//In this example we will be saving data from players who have knocked on the door
//This information will be stored in a json file in the oxide/data folder
//For this example we need to include System.Collections.Generic and Oxide.Core
//The code is a continuation of TheHooks
//We will set up the code in the same way but add in the extras

using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("TheDataWNotes", "MikeHawke", "0.0.1")]
    class TheDataWNotes : RustPlugin
    {
        private ConfigData configData;
        class ConfigData
        {
            [JsonProperty(PropertyName = "Door Ent Net Id")]
            public uint door = 0;
        }

        private bool LoadConfigVariables()
        {
            try
            {
                configData = Config.ReadObject<ConfigData>();
            }
            catch
            {
                return false;
            }
            SaveConfig(configData);
            return true;
        }

        void Init()
        {
            permission.RegisterPermission("TheDataWNotes.admin", this);
            if (!LoadConfigVariables())
            {
                Puts("Config file issue detected. Please delete file, or check syntax and fix.");
                return;
            }
        }

        protected override void LoadDefaultConfig()
        {
            Puts("Creating new config file.");
            configData = new ConfigData();
            SaveConfig(configData);
        }

        void SaveConfig(ConfigData config)
        {
            Config.WriteObject(config, true);
        }
        //So Far we have set up the plugin as we have done before
        //If you are unsure on the above, head back to TheConfig

        //Lets declare our Data class and name it storedData
        StoredData storedData;
        class StoredData
        {
            //In the data file we want a list titled knocked
            public List<ulong> Knocked = new List<ulong>();
        }
        //When the server is loaded we want it to read and write the data file
        void Loaded()
        {
            //Read the data
            storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("TheDataWONotes");
            //Write the data
            Interface.Oxide.DataFileSystem.WriteObject("TheDataWONotes", storedData);
        }
        //Data save function
        void SaveData()
        {
            //Same as above (in Loaded()) write the data
            Interface.Oxide.DataFileSystem.WriteObject("TourneyBlock", storedData);
        }

        [ChatCommand("MyDoor")]
        void mydoor(BasePlayer player)
        {

            if (!permission.UserHasPermission(player.userID.ToString(), "TheDataWNotes.admin"))
            {
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                Door door;
                if (!DOORLOOK(player, out door))
                {
                    SendReply(player, "No door Found");
                    return;
                }
                SendReply(player, $"found door {door}");
                configData.door = door.net.ID;
                SaveConfig(configData);
            }
        }
        //In this command we will add fuctionality to add the players userid (steamID) to the data list
        void OnDoorKnocked(Door door, BasePlayer player)
        {
            if (door.net.ID == configData.door)
            {
                SendReply(player, "This is an admin base... Go Away");
                //Add the player to the list
                storedData.Knocked.Add(player.userID);
                //Call save funtion to save the data list
                SaveData();
            }
            else
            {
                return;
            }
        }

        //Heres an extra command to clear the data list
        [ChatCommand("ClearDD")]
        void clearDD(BasePlayer player)
        {
            //This function clears the list
            storedData.Knocked.Clear();
            //And then we need to save the change to file so we call the save function
            SaveData();
            //Then we will infor the player that the data has been cleared
            SendReply(player, "Data Cleared");
        }

        //This is a useful hook, this hook is only called on a new save,
        //Meaning this hook is only called on the first save of the wipe
        //With this hook we will clear the data file as the server has been wiped
        private void OnNewSave(string filename)
        {
            //clear the data file
            storedData.Knocked.Clear();
            //Save the changes
            SaveData();
            //print in console that the data has been cleared
            //due to the server being wiped
            Puts("Server Wipe Detected Knocked Data Cleared.");
        }

        private bool DOORLOOK(BasePlayer player, out Door door)
        {
            RaycastHit hit;
            door = null;
            if (Physics.Raycast(player.eyes.HeadRay(), out hit, 3))
            {
                door = hit.GetEntity() as Door;
            }
            return door != null;

        }
    }
}