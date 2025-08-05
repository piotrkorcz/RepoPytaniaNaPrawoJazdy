using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// This helper class provides an extension method to allow 'await'ing
/// any standard Unity AsyncOperation.
/// </summary>
public static class UnityAsyncExtentions
{
    /// <summary>
    /// Gets an awaiter for an AsyncOperation.
    /// </summary>
    /// <param name="asyncOp">The async operation to await.</param>
    /// <returns>A task that completes when the async operation completes.</returns>
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += _ => { tcs.SetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }

    public static IEnumerator WaitForTask(Task task)
    {
        // Yield each frame until the task is completed.
        while (!task.IsCompleted)
        {
            yield return null;
        }

        // Optional: Check for errors after the task is done.
        if (task.IsFaulted)
        {
            Debug.LogError(task.Exception);
        }
    }
}


