namespace Oxide.Plugins
{
    [Info("TheCommandWONotes", "MikeHawke", "0.0.1")]
    class TheCommandWONotes : RustPlugin
    {
        [ChatCommand("test")]
        void test(BasePlayer player)
        {
            SendReply(player, "You sent the test command");
        }

        [ConsoleCommand("ConTest")]
        void contest(ConsoleSystem.Arg args)
        {
            Puts("You sent the Console Test Command And it Works");
        }
    }
}