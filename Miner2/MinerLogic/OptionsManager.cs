using MinerLogic.CommonPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic
{
    internal sealed class OptionsManager
    {
        private Options _current;

        /// <summary>
        /// returns current settings
        /// </summary>
        public Options CurrentOptions
        {
            get { return _current; }
            set
            {
                _current = value;
            }
        }

        internal void SaveCurrentOptions()
        {
            Properties.Settings.Default.Height = _current.Height;
            Properties.Settings.Default.Width = _current.Width;
            Properties.Settings.Default.AmountOfMines = _current.MinesAmount;
            Properties.Settings.Default.Save();

        }

        public OptionsManager(Options options)
        {
            CurrentOptions = options;
        }

        /// <summary>
        /// restore previously saved settings
        /// </summary>
        public static Options OptionsFromInnerSettings()
        {
            return new Options(Properties.Settings.Default.Width, Properties.Settings.Default.Height, Properties.Settings.Default.AmountOfMines);
        }

        /// <summary>
        /// returns gametype according to settings
        /// </summary>
        internal GameType GetGameType(Options options)
        {
            if (options == Options.Easy)
            {
                return GameType.Easy;
            }
            else if (options == Options.Medium)
            {
                return GameType.Medium;
            }
            else if (options == Options.Hard)
            {
                return GameType.Hard;
            }
            else
            {
                return GameType.Custom;
            }
        }
    }
}
