//Lets use a command to change a value in the config
//Lets also create a permission and check if the player has the permssions before letting
//them change any config values whiles within the game

//Lets set up or dependancies and config as we did in TheConfig 
using Newtonsoft.Json;

namespace Oxide.Plugins
{
    //Set up our plugname, author, version and classname as usual
    [Info("TheChangeWNotes", "MikeHawke", "0.0.1")]
    class TheChangeWNotes : RustPlugin
    {
        //create our config data
        private ConfigData configData;
        class ConfigData
        {
            //This time we will make rep a bool so will have a value of true or false
            [JsonProperty(PropertyName = "Bool")]
            public bool rep = true;
        }

        //do our checks
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

        //However... when we start the plugin we want to create our permission
        void Init()
        {
            //Make sure you give the permssion name the same name as the plugin otherwise
            //oxide will print an error and plugins that manage permissions may not see the new permission
            permission.RegisterPermission("TheChangeWNotes.admin", this);

            //Then we continue as before
            if (!LoadConfigVariables())
            {
                Puts("Config file issue detected. Please delete file, or check syntax and fix.");
                return;
            }
        }

        //Create config
        protected override void LoadDefaultConfig()
        {
            Puts("Creating new config file.");
            configData = new ConfigData();
            SaveConfig(configData);
        }

        //Save function 
        void SaveConfig(ConfigData config)
        {
            Config.WriteObject(config, true);
        }

        //This time we will make two commands as we have previously done
        //one will print the value of the bool rep and one will change it

        //Lets start with checking the value of rep
        [ChatCommand("Configcheck")]
        void confcheck(BasePlayer player)
        {
            //This command that we used in TheConfig would work but will be quite boring
            //SendReply(player, configData.rep);
            //So lets make it look a bit better by adding some description and refencing the config
            //we do this but opening with a $ symbol and placing our reference in bracklets

            SendReply(player, $"The config value is {configData.rep}");
        }

        //Lets now make the second command
        [ChatCommand("ChangeConfig")]
        void confchng(BasePlayer player)
        {
            //Lets first check if the player has the permission
            //We are checking if the players steamID has NOT got the permission registered to it
            //we do this by using the symbol !

            if (!permission.UserHasPermission(player.userID.ToString(), "TheChangeWNotes.admin"))
            {
                //In this area we write what happens if the above statment is true..
                //We reply to the player that they do not have the perrmission to use the command.
                SendReply(player, "You do not have permission to use this command");
                //Now we must return the function, think of this as closing to door to stop the rest of the function
                //being run
                return;
            }
            else
            {
                //This is where we write what happens if the statment above is false
                // (The player has the permission to use the command)

                //What we are going to do in this command is call the value from the config and
                //reverse it using the ! symbol

                configData.rep = !configData.rep;

                //Then we are going to save that information
                //by calling the saveconfig function above
                SaveConfig(configData);

                //And while we are at it, lets tell the player that we actually did something

                SendReply(player, $"The config value has been changed from {!configData.rep} to {configData.rep}");
            }
        }
    }
}