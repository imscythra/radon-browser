using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Project_Radon.Models.Interfaces
{
    interface IHistoryModel
    {
        Uri TheUrl { get; set; }
        BitmapImage TheContents { get; set; }
        Page ThePage { get; set; }
        UIElement TheElement { get; set; }
        string TheDocumentTitle { get; set; }
        int Position { get; set; }
        bool Equals(HistoryModel other);

        ObservableRecipient ViewModel { get; set; }
        RenderTargetBitmap Bitmap { get; set; }

    }
}
