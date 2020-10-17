//In This example we will make 2 commands
//First will be an in game chat command
//second will be a console command
//We will start our code as we did in The Basics

namespace Oxide.Plugins
{
    //Plugin Name, Author Name and version number
    [Info("TheCommandWNotes", "MikeHawke", "0.0.1")]
    class TheCommandWNotes : RustPlugin
    //Class name same as plugin name
    {
        //Lets create our chat command we will make it /test
       [ChatCommand("test")]
       //Lets name the function test and lets reference the player calling the command
       void test (BasePlayer player)
        {
            //Here we will respond to the player with SendReply
            SendReply(player, "You sent the test command");
        }

       //So typing /test in chat would cause the plugin to reply in game and only on the calling players screen
       //Now lets do the same again but this time as a console command

      [ConsoleCommand("ConTest")]
      void contest(ConsoleSystem.Arg args)
        {
            Puts("You sent the Console Test Command And it Works");
        }

        //This time I didn't not the command directly so you can see the structure.
        //We created the command "ConTest" and named the function and all the function does is use the
        //Puts function to write to console 
    }
}