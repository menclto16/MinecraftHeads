using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinecraftHeads
{
    /// <summary>
    /// Interaction logic for SkinTile.xaml
    /// </summary>
    public partial class SkinTile : UserControl
    {
        public SkinTile()
        {
            InitializeComponent();
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Tile tile = sender as Tile;
            ContextMenu contextMenu = tile.ContextMenu;
            contextMenu.PlacementTarget = tile;
            contextMenu.IsOpen = true;
            e.Handled = true;
        }

        
        private void UploadSkinCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.APIHandlerObject.UploadSkin("skinlib/" + SkinName.Content + ".png");
            App.MainPageObject.UpdateMainPage();
        }
        private void RenameSkinCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            App.MainPageObject.ShowRenameSkinWindow(SkinName.Content.ToString());
        }
        private void DeleteSkinCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new FileHandler().DeleteSkin("skinlib/" + SkinName.Content + ".png");
            App.MainPageObject.UpdateGalleryPage();
        }
        private void UploadSkinCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void RenameSkinCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void DeleteSkinCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
