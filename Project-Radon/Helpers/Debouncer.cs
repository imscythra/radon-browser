using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Project_Radon.Helpers
{
    public class Debouncer
    {
        private CancellationTokenSource _cancellationTokenSource = null;

        public void Debounce(Func<Task> action, int delayMilliseconds = 300)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Delay(delayMilliseconds, _cancellationTokenSource.Token)
                .ContinueWith(async task =>
                {
                    if (!task.IsCanceled)
                    {
                        await action();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
