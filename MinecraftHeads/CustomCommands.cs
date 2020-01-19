using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MinecraftHeads
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand UploadSkin = new RoutedUICommand
                (
                    "UploadSkin",
                    "UploadSkin",
                    typeof(CustomCommands)
                );

        public static readonly RoutedUICommand DeleteSkin = new RoutedUICommand
            (
                "DeleteSkin",
                "DeleteSkin",
                typeof(CustomCommands)
            );

        public static readonly RoutedUICommand RenameSkin = new RoutedUICommand
            (
                "RenameSkin",
                "RenameSkin",
                typeof(CustomCommands)
            );
    }
}
