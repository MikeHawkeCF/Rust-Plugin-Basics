using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("TheFinallyWONotes", "MikeHawke", "0.0.1")]
    class TheFinallyWONotes : RustPlugin
    {
        private ConfigData configData;
        class ConfigData
        {
            [JsonProperty(PropertyName = "Lockdown")]
            public bool lockdown = false;
            [JsonProperty(PropertyName = "button Ent Net Id")]
            public uint button = 0;
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
            permission.RegisterPermission("TheFinallyWONotes.admin", this);
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
            storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("TheFinallyWONotes");
            Interface.Oxide.DataFileSystem.WriteObject("TheFinallyWONotes", storedData);
        }
        void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject("TheFinallyWONotes", storedData);
        }
        [ChatCommand("SetButton")]
        void setbtn(BasePlayer player)
        {
            if (!permission.UserHasPermission(player.userID.ToString(), "TheFinallyWONotes.admin"))
            {
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                IOEntity button;
                if (!BUTTONLOOKUP(player, out button))
                {
                    SendReply(player, "No button Found");
                    return;
                }
                SendReply(player, $"found button {button}");
                configData.button = button.net.ID;
                SaveConfig(configData);
            }
        }

        void OnButtonPress(PressButton button, BasePlayer player)
        {
            if (button.net.ID == configData.button)
            {
                SendReply(player, "You have been added to the Whitelist");
                storedData.Whitelisted.Add(player.userID);
                SaveData();
            }
            else
            {
                return;
            }
        }

        void OnPlayerConnected(BasePlayer player)
        {
            if (configData.lockdown == false) return;
            if (!storedData.Whitelisted.Contains(player.userID))
            {
                Network.Net.sv.Kick(player.net.connection, rust.QuoteSafe(configData.kick));
            }
            else
            {
                Puts($"User {player.displayName} is on the whitlist");
                return;
            }
        }

        [ChatCommand("Lockdown")]
        void lockdown(BasePlayer player)
        {
            if (!permission.UserHasPermission(player.userID.ToString(), "TheFinallyWONotes.admin"))
            {
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                configData.lockdown = !configData.lockdown;
                SaveConfig(configData);
                SendReply(player, $"Lockdown is set to {configData.lockdown}");
            }
        }

        [ChatCommand("ClearDD")]
        void clearDD(BasePlayer player)
        {
            if (!permission.UserHasPermission(player.userID.ToString(), "TheFinallyWONotes.admin"))
            {
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                storedData.Whitelisted.Clear();
                SaveData();
                SendReply(player, "Data Cleared");
            }
        }

        [ConsoleCommand("ClearDD")]
        private void conClearDD(ConsoleSystem.Arg arg)
        {
            if (arg.Player() == null || arg.Player()?.IsAdmin == true)
            {
                storedData.Whitelisted.Clear();
                SaveData();
                Puts("Data Cleared");
            }
        }

        private void OnNewSave(string filename)
        {
            storedData.Whitelisted.Clear();
            SaveData();
            Puts("New Map Detected Whitelist Data Cleared.");
            configData.lockdown = false;
            SaveConfig(configData);
        }

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