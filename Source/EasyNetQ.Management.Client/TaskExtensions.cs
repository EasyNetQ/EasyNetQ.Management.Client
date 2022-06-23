using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyNetQ.Management.Client
{
    internal static class TaskExtensions
    {
        public static Task<TNewResult> ContinueWithOrThrow<TNewResult, TResult>(this Task<TResult> task, Func<Task<TResult>, Task<TNewResult>> func, CancellationToken cancellationToken)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (task.Exception != null)
                throw task.Exception.GetBaseException();
            return task.ContinueWith(func, cancellationToken).Unwrap();
        }

        public static Task ContinueWithOrThrow<TResult>(this Task<TResult> task, Func<Task<TResult>, Task> func, CancellationToken cancellationToken)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (task.Exception != null)
                throw task.Exception.GetBaseException();
            return task.ContinueWith(func, cancellationToken).Unwrap();
        }
    }
}
