//In this example we will put together what we know to create a plugin
//The functionality of the plugin will be a whitelist with a twist
//We will use a button to add a player to a list in storedata
//we will have a command that toggles the blocking function on and off
//And when the player connects we will check the list to see if they are on
//it or not and if not kick them.

using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("TheFinallyWNotes", "MikeHawke", "0.0.1")]
    class TheFinallyWNotes : RustPlugin
    {
        private ConfigData configData;
        class ConfigData
        {
            //The config will contain a bool which states if the whitelist is in effect or not
            [JsonProperty(PropertyName = "Lockdown")]
            public bool lockdown = false;
            //A uint for the button net ID
            [JsonProperty(PropertyName = "button Ent Net Id")]
            public uint button = 0;
            //and a string which hold the message we will show people when we kick them out
            [JsonProperty(PropertyName = "Kick Message")]
            public string kick = "You are not on the whitelist";
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
            permission.RegisterPermission("TheFinallyWNotes.admin", this);
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
        StoredData storedData;
        class StoredData
        {
            public List<ulong> Whitelisted = new List<ulong>();
        }
        void Loaded()
        {
            storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("TheFinallyWNotes");
            Interface.Oxide.DataFileSystem.WriteObject("TheFinallyWNotes", storedData);
        }
        void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject("TheFinallyWNotes", storedData);
        }
        //So far so good, we setup the plugin as we have done previously this time with the list Whitelisted in the data file

        //Same as before we will set up the command, check if the player has permission
        //call a function to check if the player is looking at a button or not
        //and save the data about the button to the config
        [ChatCommand("SetButton")]
        void setbtn(BasePlayer player)
        {
            //Perm check, Remember checking if the player does NOT have the perm with the ! symbol
            if (!permission.UserHasPermission(player.userID.ToString(), "TheFinallyWNotes.admin"))
            {
                //Statment true, player has no perm, tell them to go away and close the door
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                //Player has permission
                //Calling BUTTONLOOKUP (remember its a bit advanced so not to worry about it too much)
                IOEntity button;
                if (!BUTTONLOOKUP(player, out button))
                {
                    SendReply(player, "No button Found");
                    return;
                }
                SendReply(player, $"found button {button}");
                //set the config data to the hold the button net ID
                configData.button = button.net.ID;
                //Save the config
                SaveConfig(configData);
            }
        }

        //Oxide/uMod has the hook on ButtonPress which will fire everytime a button is pressed
        //by any player
        void OnButtonPress(PressButton button, BasePlayer player)
        {
            //we will filter the results to buttons that match out config
            if (button.net.ID == configData.button)
            {
                //If the button is the same as the one in the config we tell the player
                SendReply(player, "You have been added to the Whitelist");
                //add the player to the whitelist
                storedData.Whitelisted.Add(player.userID);
                //and save the data file
                SaveData();
            }
            else
            {
                //if its not our button Return
                return;
            }
        }

        //OnPlayerConnected is a function that is called when a player loads into the server
        void OnPlayerConnected(BasePlayer player)
        {
            //First we will check if our lockdown bool is true or false and if it is false
            //we will stop the code running
            if (configData.lockdown == false) return;
            //Next we will check if the player is NOT on the whitelist using the ! symbol
            if (!storedData.Whitelisted.Contains(player.userID))
            {
                //If the player is not on the list we want to kick them out
                //We will do this with the following command, but notice on the
                //end we reference our configData.kick
                //This way the plugin will kick the player and they will recive the message
                //Disconnected: You are not on the whitelist
                Network.Net.sv.Kick(player.net.connection, rust.QuoteSafe(configData.kick));
            }

            //If the player is on the whitelist we want to leave them alone
            //so we will return the code.
            //this part is not really necessary but lets keep the code clean
            //and we can add further functionality later on easily.
            //I'll add an example of something we can place there
            //We will use puts and reference some information
            //player we are getting from the hooks BasePlayer player
            //if we just refernce {player} we get the name and steamid
            //we dont want that so we can shorten it using
            //displayName (capital N) just to show their name
            else
            {
                Puts($"User {player.displayName} is on the whitlist");
                return;
            }
        }

        //Okay onto the home straight.. lets make a command to toggle the lockdown on and off
        //as previous we will take the config value, reverse it using the ! symbol, save it
        //and tell the player what the value is using our $ symbol and bracklets.
        [ChatCommand("Lockdown")]
        void lockdown(BasePlayer player)
        {
            //check if player has permission
            if (!permission.UserHasPermission(player.userID.ToString(), "TheFinallyWNotes.admin"))
            {
                //stop them if not
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                //change config value, save and inform the player if they
                //have the perm
                configData.lockdown = !configData.lockdown;
                SaveConfig(configData);
                SendReply(player, $"Lockdown is set to {configData.lockdown}");
            }
        }

        //Heres a command to clear the list via ingame chat
        [ChatCommand("ClearDD")]
        void clearDD(BasePlayer player)
        {
            //Check player permission
            if (!permission.UserHasPermission(player.userID.ToString(), "TheFinallyWNotes.admin"))
            {
                //Stop them if they have no permission
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                //If they have the permission
                //clear the data
                storedData.Whitelisted.Clear();
                //save the data
                SaveData();
                //tell the player
                SendReply(player, "Data Cleared");
            }
        }

        //Lets make a console version of that command too as there is a difference in permission
        //handling in regards to console commands
        //First a console command can be sent by a password protected rcon
        //and a player using f1 and as all players have access to the f1 console
        //we need to protect our command
        [ConsoleCommand("ClearDD")]
        private void conClearDD(ConsoleSystem.Arg arg)
        {
            //we will do this by checking is there is a player or not OR if the player is and admin
            //Using the || symbols is the same as saying OR
            if (arg.Player() == null || arg.Player()?.IsAdmin == true)
            {
                //The above statment is true, either there is not player so
                //We are in Rcon OR the player has the admin tag

                //Wipe the data as we have done before
                storedData.Whitelisted.Clear();
                //save the cleared list
                SaveData();
                //print to rcon that we have done it
                Puts("Data Cleared");
            }
        }

        //We will keep the newsave function so that if we wipe the server it wipes the list
        //This is explained in TheData and is a usefull function to have for automation
        //But we will add a rule to set the lockdown back to false
        private void OnNewSave(string filename)
        {
            storedData.Whitelisted.Clear();
            SaveData();
            Puts("New Map Detected Whitelist Data Cleared.");
            //here we will add in the rule
            configData.lockdown = false;
            //and save it
            SaveConfig(configData);
        }

        //Again, here is the function that we use to look up the IOEntity
        //bit more advanced so dont worry about it too much yet
        //by now however you should see how I have changed it to look
        //for doors and ioentities
        private bool BUTTONLOOKUP(BasePlayer player, out IOEntity button)
        {
            RaycastHit hit;
            button = null;
            if (Physics.Raycast(player.eyes.HeadRay(), out hit, 3))
            {
                button = hit.GetEntity() as PressButton;
            }
            return button != null;

        }
    }
}