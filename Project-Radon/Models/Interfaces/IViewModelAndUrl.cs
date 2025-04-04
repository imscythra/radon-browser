using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Project_Radon.Models.Interfaces
{
    interface IViewModelAndUrl
    {
        ObservableRecipient ObservableRecipientVm { get; set; }

        Uri NavigatedUrl { get; set; }

    }
}
