using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinerLogic.Interfaces
{
   public interface IMessageService
   {
      void ShowMessage(string message, string caption);
      void ShowError(string message, string caption);

      /// <summary>
      /// message about win game with propose to new game
      /// </summary>
      /// <returns>true if want new game</returns>
      bool WinGame(string text, string caption);
      /// <summary>
      /// message about lost game with propose to new game
      /// </summary>
      /// <returns>true if want new game</returns>
      bool LooseGame(string text, string caption);
   }
}
