//Copyright AWAN SOFTWORKS LTD 2025

using System;
using System.Threading.Tasks;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Armony.Utilities.Libraries
{
    public static class LibAsync
    {
        public static async void RunAsync(this Task _task)
        {
            try
            {
                await _task;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
        
        public static Task Task(this AsyncOperation _asyncOperation)
        {
            TaskCompletionSource<object> taskCompletionSource = new();
            _asyncOperation.completed += (_operation) =>
            {
                if (_operation.isDone)
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