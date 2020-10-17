using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("TheChangeWONotes", "MikeHawke", "0.0.1")]
    class TheChangeWONotes : RustPlugin
    {
        private ConfigData configData;
        class ConfigData
        {
           [JsonProperty(PropertyName = "Bool")]
            public bool rep = true;
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
            permission.RegisterPermission("TheChangeWONotes.admin", this);
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

        [ChatCommand("Configcheck")]
        void confcheck(BasePlayer player)
        {
            SendReply(player, $"The config value is {configData.rep}");
        }

        [ChatCommand("ChangeConfig")]
        void confchng(BasePlayer player)
        {
            if (!permission.UserHasPermission(player.userID.ToString(), "TheChangeWONotes.admin"))
            {
                SendReply(player, "You do not have permission to use this command");
                return;
            }
            else
            {
                configData.rep = !configData.rep;
                SaveConfig(configData);
                SendReply(player, $"The config value has been changed from {!configData.rep} to {configData.rep}");
            }
        }
    }
}