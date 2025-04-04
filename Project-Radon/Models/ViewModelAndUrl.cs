using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Project_Radon.Models.Interfaces;

namespace Project_Radon.Models
{
    public class ViewModelAndUrl : IViewModelAndUrl
    {
        public ObservableRecipient ObservableRecipientVm { get; set; }
        public Uri NavigatedUrl { get; set; }

        public ViewModelAndUrl()
        {

        }

        public ViewModelAndUrl(Uri navigateUrl, ObservableRecipient viewModel)
        {
            NavigatedUrl = navigateUrl;
            ObservableRecipientVm = viewModel;
        }
    }
}
