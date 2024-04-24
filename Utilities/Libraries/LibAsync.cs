using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Armony.Utilities.Libraries
{
    public static class LibAsync
    {
        public static async void RunAsync(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
        
        public static Task Task(this AsyncOperation asyncOperation)
        {
            TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

            asyncOperation.completed += (operation) =>
            {
                if (operation.isDone)
                {
                    taskCompletionSource.TrySetResult(null);
                }
                else
                {
                    taskCompletionSource.TrySetCanceled();
                }
            };

            return taskCompletionSource.Task;
        }
    }
}