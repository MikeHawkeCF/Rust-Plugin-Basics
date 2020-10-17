using Newtonsoft.Json;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("TheHooksWONotes", "MikeHawke", "0.0.1")]
    class TheHooksWONotes : RustPlugin
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
            permission.RegisterPermission("TheHooksWONotes.admin", this);
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
        
        [ChatCommand("MyDoor")]
        void mydoor(BasePlayer player)
        {
       
            if (!permission.UserHasPermission(player.userID.ToString(), "TheHooksWONotes.admin"))
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

        void OnDoorKnocked(Door door, BasePlayer player)
        {
            if (door.net.ID == configData.door)
            {
                SendReply(player, "This is an admin base... Go Away");
            }
            else
            {
               return;
            }
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