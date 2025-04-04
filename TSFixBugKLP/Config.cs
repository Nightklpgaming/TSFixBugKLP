using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TShockAPI;

namespace TSFixBugKLP
{

    public class Config
    {
        public CONFIG_MAIN Main;

        static string path = Path.Combine(TShock.SavePath, "TSFixBugKLP.json");

        public static Config Read()
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Default(), Formatting.Indented));
                return Default();
            }

            var args = JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));

            if (args == null) return Default();

            if (args.Main == null) args.Main = new();
            args.Main.FixNull();

            File.WriteAllText(path, JsonConvert.SerializeObject(args, Formatting.Indented));
            return args;
        }

        /// <summary>
        /// changes config file
        /// </summary>
        /// <param name="config"></param>
        public void Changeall(Config config)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(Default(), Formatting.Indented));
            }
            else
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }

        private static Config Default()
        {
            return new Config()
            {
                Main = new CONFIG_MAIN(),
            };
        }
    }

    #region [ Config Objects ]

    public class CONFIG_MAIN
    {
        public bool? Enable_ItemFrameFix = true;
        public bool? Enable_WeaponRackFix = true;
        public bool? Enable_FixCPlayerG = true;
        public CONFIG_MAIN() { }

        public void FixNull()
        {
            CONFIG_MAIN getdefault = new();

            if (Enable_ItemFrameFix == null) Enable_ItemFrameFix = getdefault.Enable_ItemFrameFix;
            if (Enable_WeaponRackFix == null) Enable_WeaponRackFix = getdefault.Enable_WeaponRackFix;
            if (Enable_FixCPlayerG == null) Enable_FixCPlayerG = getdefault.Enable_FixCPlayerG;
        }
    }

    #endregion
}
