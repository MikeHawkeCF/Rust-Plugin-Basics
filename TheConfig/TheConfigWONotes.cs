using Newtonsoft.Json;

namespace Oxide.Plugins
{
    [Info("TheConfigWONotes", "MikeHawke", "0.0.1")]
    class TheConfigWONotes : RustPlugin
    {
        private ConfigData configData;
        class ConfigData
        {
            [JsonProperty(PropertyName = "Reply Message")]
            public string rep = "This is the reply that is set in the config.";
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

        [ChatCommand("ConfigTest")]
        void confTest(BasePlayer player)
        {
            SendReply(player, configData.rep);
        }
    }
}