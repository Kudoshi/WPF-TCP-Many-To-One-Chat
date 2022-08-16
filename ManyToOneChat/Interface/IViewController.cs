using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ManyToOneChat.Interface
{
    internal interface IViewController
    {
        MainWindow PageManager { get; set; }
        Page ViewPage { get; set; }
        Page SetupController(MainWindow window);
    }
}
