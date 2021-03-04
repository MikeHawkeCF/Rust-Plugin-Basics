using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("MoreDatas", "MikeHawke", "0.0.1")]
    class MoreDatas : RustPlugin
    {
        private ConfigData configData;
        class ConfigData
        {
            //Stuff to save in the config
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

        StoredData storedData;
        class StoredData
        {
            //Our new Dictionary 
            public Dictionary<string, Hits> PlayerStats = new Dictionary<string, Hits>();
        }
        //Our new class
        class Hits
        {
            //Stuff we want in our data file
            public int attacks = 0;
            public int deaths = 0;
        }
        void Loaded()
        {
            storedData = Interface.Oxide.DataFileSystem.ReadObject<StoredData>("MoreDatas");
            Interface.Oxide.DataFileSystem.WriteObject("MoreDatas", storedData);
        }

        //Kill the UI for all players on reload/unload
        void Unload()
        {
            foreach (BasePlayer current in BasePlayer.activePlayerList)
            {
                CuiHelper.DestroyUi(current, "HUD");
            }
        }
        void SaveData()
        {
            Interface.Oxide.DataFileSystem.WriteObject("MoreDatas", storedData);
        }
        //Hooks
        void OnPlayerAttack(BasePlayer attacker, HitInfo info)
        {
            if(!storedData.PlayerStats.ContainsKey(attacker.UserIDString))
            {
                Hits value = new Hits();
                storedData.PlayerStats.Add(attacker.UserIDString, value);
            }
            storedData.PlayerStats[attacker.UserIDString].attacks++;
            SaveData();
        }
        object OnPlayerDeath(BasePlayer player, HitInfo info)
        {
            if (!storedData.PlayerStats.ContainsKey(player.UserIDString))
            {
                Hits value = new Hits();
                storedData.PlayerStats.Add(player.UserIDString, value);
            }
            storedData.PlayerStats[player.UserIDString].deaths++;
            SaveData();
            return null;
        }
        //UI
        void ui(BasePlayer player)
        {
            //Destroy the UI to stop multiples appearing
            CuiHelper.DestroyUi(player, "HUD");

            var elements = new CuiElementContainer();
            
            //The Correct way you write a UI Elements
            //The Panel
            var HUDUI = elements.Add(new CuiPanel 
            { 
                Image =
                { 
                    Color = "0 0 0 0.85" 
                },
                RectTransform = 
                { 
                    AnchorMax = "0.735 0.894", 
                    AnchorMin = "0.266 0.617"
                },
                CursorEnabled = true,
                FadeOut = 0.5f 
            }, "Overlay", "HUD");

            //How I write my UI Elements
            //Label
            elements.Add(new CuiLabel { Text = { Text = "Player Name:", Color = "1 1 1 1"}, RectTransform = { AnchorMax = "0.18 0.935", AnchorMin = "0.035 0.85" } }, "HUD");
            //Label
            elements.Add(new CuiLabel { Text = { Text = storedData.PlayerStats[player.UserIDString].attacks.ToString(), Color = "1 1 1 1" }, RectTransform = { AnchorMax = "0.343 0.935", AnchorMin = "0.198 0.855" } }, "HUD");
            //Button
            elements.Add(new CuiButton { Button = { Command = "closeui", Color = "1 0 0 1" }, RectTransform = { AnchorMax = "1 0.125", AnchorMin = "0.958 0" }, Text = { Text = "X", Color = "1 1 1 1", FontSize = 20, Align=TextAnchor.MiddleCenter } }, HUDUI);
            //Create UI
            CuiHelper.AddUi(player, elements);
        }

        //Command to call UI
        [ChatCommand("callui")]
        void callui(BasePlayer player)
        {
            ui(player);
        }
        
        //Command to close UI
        [ConsoleCommand("closeui")]
        void callclose(ConsoleSystem.Arg args)
        {
            if (args.Player() == null) return;
            CuiHelper.DestroyUi(args.Player(), "HUD");
        }
    }
}