using MinerLogic.CommonPublic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.Interfaces
{
    public interface IOptionsView
    {
        short Mines { get; set; }
        byte FieldWidth { get; set; }
        byte FieldHeight { get; set; }

        /// <summary>
        /// установка доступности полей настройки количества мин, высоты, ширины
        /// </summary>
        void EnableCustomValues(bool enable);


        event EventHandler NotCustomGameSelect;
        event EventHandler CustomGameSelect;
        event EventHandler<ChangeOptionsEventArgs> ConfirmSelectionClick;

        void Show();
        void Close();
        void SetSelectedOption(GameType currentGameType);
    }
}
