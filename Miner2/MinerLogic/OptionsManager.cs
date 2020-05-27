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
        //private GameType _gameType;

        /// <summary>
        /// возвращает текущие настройки
        /// </summary>
        public Options CurrentOptions // текущие настройки
        {
            get { return _current; }
            set
            {
                _current = value;
                //if (_current == Options.Easy)
                //{
                //    _gameType = GameType.Easy;
                //}
                //else if (_current == Options.Medium)
                //{
                //    _gameType = GameType.Medium;
                //}
                //else if (_current == Options.Hard)
                //{
                //    _gameType = GameType.Hard;
                //}
                //else
                //{
                //    _gameType = GameType.Custom;
                //}
            }
        }

        /// <summary>
        /// возвращает текущий тип игры
        /// </summary>
        //public GameType CurrentGameType
        //{
        //    get { return _gameType; }
        //}

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
        /// востанавливаем ранее сохраненные настройки
        /// </summary>
        public static Options OptionsFromInnerSettings()
        {
            return new Options(Properties.Settings.Default.Width, Properties.Settings.Default.Height, Properties.Settings.Default.AmountOfMines);
        }

        /// <summary>
        /// возвращает тип игры га основании настроек
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
