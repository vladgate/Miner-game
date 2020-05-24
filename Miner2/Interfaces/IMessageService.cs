using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.Interfaces
{
    public interface IMessageService
    {
        void ShowMessage(string message);
        void ShowError(string message);

        /// <summary>
        /// вывод сообщения о выиграше в игре с предложением начать новую игру
        /// </summary>
        /// <param name="text">текст сообщения</param>
        /// <param name="caption">заголовок</param>
        /// <returns>true если хотим начать новую игру</returns>
        bool WinGame(string text, string caption);
        /// <summary>
        /// вывод сообщения о проигрыше в игре с предложением начать новую игру
        /// </summary>
        /// <param name="text">текст сообщения</param>
        /// <param name="caption">заголовок</param>
        /// <returns>true если хотим начать новую игру</returns>
        bool LooseGame(string text, string caption);
    }
}
