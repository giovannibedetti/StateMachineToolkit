using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.gb.statemachine_toolkit
{
    public static class Utilities
    {
        public static Coroutine WaitThenAct(MonoBehaviour caller, float wait, Action action)
        {
            return caller.StartCoroutine(WaitingThenAct(wait, action));
        }

        static IEnumerator WaitingThenAct(float wait, Action action)
        {
            //yield return new WaitForSeconds(wait);
            var startTime = Time.time;
            Debug.Log($"{Time.time} started waiting for {wait} seconds");
            while ((startTime + wait) > Time.time) yield return null;
            Debug.Log($"{Time.time} finished waiting for {wait} seconds, calling action");
            action?.Invoke();
        }

        /// <summary>
        /// Utility function to check if the specified parameter exists in the given animator
        /// </summary>
        /// <param name="paramName">The parameter to check for existence</param>
        /// <param name="animator">The animator to check</param>
        /// <returns>If the parameter exists or not</returns>
        public static bool HasParameter(string paramName, Animator animator)
        {
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                if (param.name == paramName) return true;
            }
            return false;
        }
    }
}
