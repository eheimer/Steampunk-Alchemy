using System;
using System.Collections;
using UnityEngine;

namespace Spinach
{
  public static class Utils
  {
    /// <summary>
    /// Waits for the specified delay, then executes the provided action.
    /// </summary>
    /// <param name="delay">The delay in seconds before the action is executed.</param>
    /// <param name="actionToExecute">The action to execute after the delay.</param>
    /// <returns>An IEnumerator that can be used to start a coroutine.</returns>
    public static IEnumerator WaitAndExecute(float delay, Action actionToExecute)
    {
      yield return new WaitForSeconds(delay);
      actionToExecute();
    }
  }
}