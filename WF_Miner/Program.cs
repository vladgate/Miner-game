﻿using MinerLogic.Interfaces;
using MinerPresenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WF_Miner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm mainView = new MainForm();
            IMessageService messageService = new MessageService();
            Presenter presenter = new Presenter(mainView, messageService);
            Application.Run(mainView);
        }
    }
}
