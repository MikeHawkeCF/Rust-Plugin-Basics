//On this example we will create a config and reference it
//in This example we need to reference a dependancy called Newtonsoft Json
//We do this by doing the following:

using Newtonsoft.Json;

namespace Oxide.Plugins
{
    //Name the Plugin, Author Name and version
    [Info("TheConfigWNotes", "MikeHawke", "0.0.1")]
    class TheConfigWNotes : RustPlugin
    // Class same as plugin name
    {
        //Okay.. Lets take this slow and easy
        // Fist lets Declare the config as configData and write whats in it
        private ConfigData configData;
        class ConfigData
        {
            //This is where we will put stuff be it a bool, string, interger, ulong etc etc
            //We will also use JsonProperty to give it a nice name.

            [JsonProperty(PropertyName = "Reply Message")]
            public string rep = "This is the reply that is set in the config.";

            //We added a string called rep with a value
            //Using Json Property it will appear in the config as
            // Reply Message : "This is the reply that is set in the config."
            //Without Json Property it would appear as
            // rep : "This is the reply that is set in the config."

            //Without referencing Newtonsoft.Json the Json Properties will not work and cause errors
        }

        //Okay.. lets make a check and a save function.
        //Lets first check that if there is a config we can read it.
        //we will make that into a bool and refernce it

        //Name the bool LoadConfigVariables
        private bool LoadConfigVariables()
        {
            try
            {
                configData = Config.ReadObject<ConfigData>();
            }
            //See if the config data is the same as what we want
            catch
            {
                return false;
            }
            //If there is an error like a syntax error make the bool false
            SaveConfig(configData);
            return true;
            //Otherwise call the save function (we will see that later) and make the bool true
        }


        //On plugin initialise
        void Init()
        {
            //check it the bool above is false
            if (!LoadConfigVariables())
            {
                //If its false there is an error. so we will print that to console.
                Puts("Config file issue detected. Please delete file, or check syntax and fix.");
                return;
            }
        }

        //Lets create the config if there isnt one in the config folder
        protected override void LoadDefaultConfig()
        {
            //print a message to console to tell you what we are doing
            Puts("Creating new config file.");
            // define the config data we want
            configData = new ConfigData();
            //Call the save function (below)
            SaveConfig(configData);
        }

        //The save function 
        void SaveConfig(ConfigData config)
        {
            //Every time we have called the save function we have done it with an extra bit of information
            //The function we called was "SaveConfig(configData);" what we did is passed the information "configData"
            //The function we are calling "SaveConfig(ConfigData config)" is looking for the information in config.
            //E.g. if we did "SaveConfig(cheese);" config would equal "cheese"

            //We are refenceing that here
            //Config.WriteObject (write an object in the config folder)
            //(config, true) config = configData... or cheese and true as in go ahead
            Config.WriteObject(config, true);

            //Config saved..
        }

        //Lets do something with that information..
        //Lets make a command and call the info from the config

        //Lets use /ConfigTest and reference the player
        [ChatCommand("ConfigTest")]
        void confTest(BasePlayer player)
        {
            //Same as we did before we will send a reply however this time we will call the string "rep" from the configData

            SendReply(player, configData.rep);

            //And there we go, the player should get the response "This is the reply that is set in the config"
        }
    }
}
