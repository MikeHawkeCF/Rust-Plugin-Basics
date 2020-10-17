//In this example we will take a look at one of the various hooks
//that oxide/uMod have avalible to us
//You can see a list of the hooks at https://umod.org/documentation/games/rust
//In this example we will use OnDoorKnocked
//We will set up our plugin with a config and a string value that we can change via a command
//However on this plugin we will also need the UnityEngine Dependency
//We include this with "using UnityEngine;"
//If you have followed along you will know how to do this, if not got back and check TheConfig and TheChange

using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("TheHooksWNotes", "MikeHawke", "0.0.1")]
    class TheHooksWNotes : RustPlugin
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
            permission.RegisterPermission("TheHooksWNotes.admin", this);

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
        // This finishes our Plugin, Config and permission setup.
        // Now lets make a Command

        [ChatCommand("MyDoor")]
        void mydoor(BasePlayer player)
        {
            //Check For Permission
            if (!permission.UserHasPermission(player.userID.ToString(), "TheHooksWNotes.admin"))
            {
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                //Okay.. we are going to check if the player is looking at a door
                //Then grab details about the door and put it in the config
                //We are going to call some info from a function called DOORLOOK
                //Don't worry about that now, all you need to know about it
                //is that we check what the player is looking at and if its a door
                //or not and then we continue.

                Door door;
                if (!DOORLOOK(player, out door))
                {
                    SendReply(player, "No door Found");
                    return;
                }
                SendReply(player, $"found door {door}");
                //Lets take the Entity Net ID of the door and add it to the config
                configData.door = door.net.ID;
                //and save the config
                SaveConfig(configData);
            }
        }

        //Now that we have some values to work with lets use our hook and variables
        void OnDoorKnocked(Door door, BasePlayer player)
        {
            //OnDoorKnocked is called anytime any player knocks anydoor...
            //so lets limit the function to only the door in the config.
            if (door.net.ID == configData.door)
            {
                //if the door being knocked is the some one in the config
                //send a message to the player knocking
                SendReply(player, "This is an admin base... Go Away");
            }
            else
            {
                //if the door is not the one in the config close the door.
                return;
            }
        }

        //This function is a bit advanced so dont worry about it now
        //but all it does is check if the player is looking at a door
        //and send information about the door back to the MyDoor Command
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