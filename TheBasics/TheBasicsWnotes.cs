//Fistly anything written with two slashes (//) are notes and not read by rust/oxide
//So we can note everything we are doing as we go along

namespace Oxide.Plugins
{
    //Here we write the Plugin name, author name and version

    [Info("TheBasicsWnotes", "MikeHawke", "0.0.1")]
    class TheBasicsWnotes : RustPlugin

    //Your Class name is the same as your plugin name

    {
        //Here is where we write all our code E.g. Init is called when the plugin is initialized
        //So we will use that. The command 'Puts' writes text to console 
        void Init()
        {
            Puts("We made our first plugin!");
        }

    }
}
//Loading this into the plugins folder will tell you that the plugin has compiled and "We made our first plugin!"