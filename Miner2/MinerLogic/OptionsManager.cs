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
        private GameType _gameType;

        /// <summary>
        /// возвращает текущие настройки
        /// </summary>
        public Options CurrentOptions // текущие настройки
        {
            get { return _current; }
            set
            {
                _current = value;
                if (_current == Options.Easy)
                {
                    _gameType = GameType.Easy;
                }
                else if (_current == Options.Medium)
                {
                    _gameType = GameType.Medium;
                }
                else if (_current == Options.Hard)
                {
                    _gameType = GameType.Hard;
                }
                else
                {
                    _gameType = GameType.Custom;
                }
            }
        }

        /// <summary>
        /// возвращает текущий тип игры
        /// </summary>
        public GameType CurrentGameType
        {
            get { return _gameType; }
        }

        public OptionsManager(Options options)
        {
            CurrentOptions = options;
        }

        /// <summary>
        /// при вызове без параметров востанавливаем ранее сохраненные настройки
        /// </summary>
        public OptionsManager()
        {
            CurrentOptions = GetOptionsFromInnerSettings();
        }

        private Options GetOptionsFromInnerSettings()
        {
            return new Options(Properties.Settings.Default.Width, Properties.Settings.Default.Height, Properties.Settings.Default.AmountOfMines);
        }
    }
}
