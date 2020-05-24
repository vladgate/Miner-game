﻿using MinerLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF_Miner
{
    internal sealed class MessageService : IMessageService
    {
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Miner 2.0", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public bool WinGame(string text, string caption)
        {
            DialogResult dr = MessageBox.Show(text, caption, MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool LooseGame(string text, string caption)
        {
            DialogResult dr = MessageBox.Show(text, caption, MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
