using Project_Radon.Models;

namespace Project_Radon.Contracts.ViewModels
{
    public interface INavigationAwareVM
    {
        void OnNavigatedTo(ViewModelAndUrl viewModelAndUrl);
        void OnNavigatedFrom();
    }
    public interface INavigationAware
    {
        void OnNavigatedTo(object parameter);

        void OnNavigatedFrom();
    }
}
