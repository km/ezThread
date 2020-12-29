using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ezThread
{
    public class Job
    {
        public Action A;
        private CancellationToken CT;
        private CancellationTokenSource source = new CancellationTokenSource();
        public Job(Action a)
        {
            A = a;
            CT = source.Token;
        }
        //Executes the task
        public void execute(int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                A();
            }
        }
        //Executes the task asynchronously (Doesn't keep the program open) 
        public async void executeAsync(int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                await Task.Run(A, CT);
            }
        }
        //Cancels the cancellation token then the async task will stop
        public void cancelExecution()
        {
            source.Cancel();
        }
    }
}
