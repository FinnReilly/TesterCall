using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TesterCall.Powershell
{
    public class TesterCallCmdlet : Cmdlet
    {
        protected virtual void Await(Task awaitableTask,
                                    string taskName,
                                    string taskMessage)
        {
            var i = 0.0;
            var spaceFormat = string.IsNullOrEmpty(taskMessage) ? "" : ": ";
            while (!awaitableTask.IsCompleted && !awaitableTask.IsFaulted)
            {
                Thread.Sleep(1);
                i++;
                WriteProgress(new ProgressRecord(0,
                                                taskName,
                                                $"{taskMessage}{spaceFormat}Waited {i / 1000.0} seconds"));
            }

            if (awaitableTask.IsFaulted)
            {
                throw awaitableTask.Exception.InnerException;
            }
        }

        protected virtual T AwaitResult<T>(Task<T> awaitableTask,
                                            string taskName,
                                            string taskMessage)
        {
            Await(awaitableTask, taskName, taskMessage);

            return awaitableTask.Result;
        }
    }
}
