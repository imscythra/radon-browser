using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Radon.Helpers
{
    static class CollectionHelper
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
        {

            return enumerableList != null ? new ObservableCollection<T>(enumerableList) : null;

        }
    }
}
