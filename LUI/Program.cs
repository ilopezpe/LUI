using LUI.config;
using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LUI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            #region Parse command line options

            var configfile = Constants.DefaultConfigFileLocation;
            var show_help = false;
            var p = new OptionSet
            {
                {
                    "f|file", "Configuration file",
                    v => configfile = v
                },
                {
                    "h|help", "Show this help text and exit",
                    v => show_help = true
                }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine("Invalid arguments: " + e.Message);
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            #endregion Parse command line options

            #region Deserialize XML and setup LuiConfig

            LuiConfig Config;

            try
            {
                var serializer = new XmlSerializer(typeof(LuiConfig));
                using (var reader = new StreamReader(configfile))
                {
                    Config = (LuiConfig)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                    Config = LuiConfig.DummyConfig();
                else if (ex is InvalidOperationException)
                    Config = LuiConfig.DummyConfig();
                else
                    throw;
            }

            Config.ConfigFile = configfile;
            Config.ConfigureLogging();

            #endregion Deserialize XML and setup LuiConfig

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ParentForm(Config));
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("LUI " + Assembly.GetExecutingAssembly().GetName().Version + " Help");
            Console.WriteLine("=============");
            foreach (var o in p) Console.WriteLine(o.Prototype + "\t\t\t" + o.Description);
        }
    }
}