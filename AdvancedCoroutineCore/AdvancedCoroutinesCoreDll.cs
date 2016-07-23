// <copyright file="AdvancedCoroutinesCoreDll.cs" company="Parallax Pixels">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Michael Kulikov</author>
// <date>07/05/2016 19:09:58 AM </date>

using System;
using System.Collections;
using System.Collections.Generic;

namespace AdvancedCoroutines.Core
{
    public class AdvancedCoroutinesCoreDll
    {
        /// <summary>
        /// Method signature for extention methods
        /// </summary>
        /// <param name="o">Any object</param>
        /// <returns>Must return 'true' when coroutine needs to wait for condition 
        /// and 'false' when it needs to proceed</returns>
        /// <example>
        /// <code>
        /// bool SomeMethod(Object o)
        /// {
        ///     if(o is SomeEntitiy && (o as SomeEntity).NeedsToWait)
        ///     {
        ///         return true;
        ///     }
        ///     return false;
        /// }
        /// </code>
        /// </example>
        public delegate bool ExtentionMethod(object o);

        private List<Routine> _routines;

        private static ExtentionMethod ExtMethod;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="extentionMethod">Method for extending Coroutines</param>
        public AdvancedCoroutinesCoreDll(ExtentionMethod extentionMethod)
        {
            ExtMethod = extentionMethod;
            _routines = new List<Routine>();
        }

        /// <summary>
        /// Update must be called only from CoroutineManager class 
        /// </summary>
        /// <param name="deltaTime">Time between frames</param>
        public void Update(float deltaTime)
        {
            UpdateRoutine(deltaTime);
        }

        ///// <summary>
        ///// LateUpdate must be called only from CoroutineManager class
        ///// </summary>
        public void LateUpdate()
        {
            LateUpdateRoutine();
        }

        /// <summary>
        /// Start new coroutine.
        /// If parameter 'obj' is null coroutine will be marked as standalone coroutine.
        /// Standalone coroutine can be stopped by StopCoroutine() 
        /// Non-standalone can be stopped by StopCoroutine() and StopAllCoroutines(). 
        /// Lives until it stops or until object from which Routine was created is destroyed
        /// </summary>
        /// <param name="enumerator">IEnumerator method</param>
        /// <param name="obj">Object which StartCoroutine was executed from. 'this' in most cases. 'null' for standalone coroutine</param>
        /// <returns>New Routine object</returns>
        /// <example><code>yield return new Wait(seconds)</code> yields for 'seconds' seconds</example>
        /// <example><code>yield return new Wait(Wait.WaitType.ForEndOfUpdate)</code> yields for end of Update</example>
        /// <example><code>yield return new Wait(Wait.WaitType.ForEndIfFrame)</code> yields for next frame</example>
        public Routine StartCoroutine(IEnumerator enumerator, object obj)
        {
            return StartRoutine(enumerator, obj);
        }

        /// <summary>
        /// Stop coroutine
        /// </summary>
        /// <param name="routine">Routine object of coroutine</param>
        public void StopCoroutine(Routine routine)
        {
            if(Routine.IsNull(routine))
                return;

            Routine.Erase(routine);
        }

        /// <summary>
        /// Stop all non-standalone coroutines executed from certain object
        /// </summary>
        /// <param name="o">Object created all coroutines that must be stopped. 'this' in most of cases</param>
        public void StopAllCoroutines(object o)
        {
            for(int i = 0; i < _routines.Count; i++)
            {
                if(_routines[i].obj != null && _routines[i].obj.Equals(o))
                {
                    Routine.Erase(_routines[i]);
                }
            }
        }

        private Routine StartRoutine(IEnumerator enumerator, object obj)
        {
            Routine newRoutine = new Routine(enumerator, obj);
            if (!newRoutine.enumerator.MoveNext()) return null;

            if(newRoutine.enumerator.Current != null)
            {
                if(newRoutine.enumerator.Current is Wait)
                {
                    Wait wait = (Wait)newRoutine.enumerator.Current;

                    switch (wait.waitTypeInternal)
                    {
                        case Wait.WaitTypeInternal.ForEndOfUpdate:
                        case Wait.WaitTypeInternal.ForEndOfFrame:
                        {
                            _routines.Add(newRoutine);
                            return newRoutine;
                        }

                        case Wait.WaitTypeInternal.ForTime:
                        {
                            //if(DateTime.Compare(newRoutine.startTime.AddSeconds(wait.Seconds), newRoutine.workTime) >= 0)
                            if((newRoutine.startTime + wait.Seconds) - newRoutine.workTime >= 0)
                            {
                                _routines.Add(newRoutine);
                                return newRoutine;
                            }
                            break;
                        }
                    }
                }
                else if(ExtMethod != null && ExtMethod(newRoutine.enumerator.Current))
                {
                    _routines.Add(newRoutine);
                    return newRoutine;
                }
            }

            _routines.Add(newRoutine);
            return newRoutine;
        }

        private void UpdateRoutine(float deltaTime)
        {
            for (int iRoutine = 0; iRoutine < _routines.Count; iRoutine++)
            {
                var routine = _routines[iRoutine];
                if(!IsRoutineInNormalCondition(iRoutine))
                {
                    DeleteRoutineFromStorage(ref iRoutine);
                    continue;
                }

                if(routine.isPaused) continue;

                if(routine.enumerator.Current != null)
                {
                    if(routine.enumerator.Current is Wait)
                    {
                        Wait wait = (Wait)routine.enumerator.Current;

                        if (wait.waitTypeInternal == Wait.WaitTypeInternal.ForEndOfUpdate)
                        {
                            routine.needToCheckEndOfUpdate = true;
                            continue;
                        }
                        if (wait.waitTypeInternal == Wait.WaitTypeInternal.ForEndOfFrame)
                        {
                            routine.needToCheckPostRender = true;
                            continue;
                        }
                        if(wait.waitTypeInternal == Wait.WaitTypeInternal.ForTime)
                        {
                            float waitTime = wait.Seconds;

                            //_routines[iRoutine].workTime = _routines[iRoutine].workTime.AddSeconds(deltaTime);
                            routine.workTime += deltaTime;
                            //if(DateTime.Compare(_routines[iRoutine].startTime.AddSeconds(waitTime), _routines[iRoutine].workTime) >= 0)
                            if((routine.startTime + waitTime) - routine.workTime > 0)
                                continue;

                            //_routines[iRoutine].startTime = DateTime.UtcNow;
                            //_routines[iRoutine].workTime = DateTime.UtcNow;

                            routine.startTime = 0;
                            routine.workTime = 0;
                        }
                    }
                    else if(ExtMethod != null && ExtMethod(routine.enumerator.Current)) continue;
                }

                if (!routine.enumerator.MoveNext())
                {
                    DeleteRoutineFromStorage(ref iRoutine);
                }
            }
        }

        private void LateUpdateRoutine()
        {
            for (int iRoutine = 0; iRoutine < _routines.Count; iRoutine++)
            {
                if(!_routines[iRoutine].needToCheckEndOfUpdate) continue;
                if(!IsRoutineInNormalCondition(iRoutine))
                {
                    DeleteRoutineFromStorage(ref iRoutine);
                    continue;
                }
                if(_routines[iRoutine].isPaused) continue;
                if (_routines[iRoutine].enumerator.Current == null) continue;
                if (!(_routines[iRoutine].enumerator.Current is Wait)) continue;
        
                Wait wait = ((Wait)_routines[iRoutine].enumerator.Current);
                if(wait.waitTypeInternal != Wait.WaitTypeInternal.ForEndOfUpdate) continue;
                {
                    if (!_routines[iRoutine].enumerator.MoveNext())
                    {
                        DeleteRoutineFromStorage(ref iRoutine);
                    }
                }
            }
        }

        public void OnPostRender()
        {
            for (int iRoutine = 0; iRoutine < _routines.Count; iRoutine++)
            {
                if(!_routines[iRoutine].needToCheckPostRender) continue;
                if(!IsRoutineInNormalCondition(iRoutine))
                {
                    DeleteRoutineFromStorage(ref iRoutine);
                    continue;
                }
                if(_routines[iRoutine].isPaused) continue;
                if (_routines[iRoutine].enumerator.Current == null) continue;
                if (!(_routines[iRoutine].enumerator.Current is Wait)) continue;
            
                Wait wait = ((Wait)_routines[iRoutine].enumerator.Current);
                if(wait.waitTypeInternal != Wait.WaitTypeInternal.ForEndOfFrame) continue;
                if (!_routines[iRoutine].enumerator.MoveNext())
                {
                    DeleteRoutineFromStorage(ref iRoutine);
                }
            }
        }

        private bool IsRoutineInNormalCondition(int index)
        {
            bool enumeratorIsNull = _routines[index].enumerator == null;
            if (enumeratorIsNull || (!_routines[index].isStandalone && (_routines[index].obj == null || _routines[index].obj.Equals(null))))
            {
                return false;
            }
            return true;
        }

        private void DeleteRoutineFromStorage(ref int iRoutine)
        {
            if(Statistics.AdvancedCoroutinesStatistics.IsActive)
                Statistics.AdvancedCoroutinesStatistics.Remove(_routines[iRoutine]);
            Routine.Erase(_routines[iRoutine]);
            _routines.RemoveAt(iRoutine);
            iRoutine--;
        }
    }
}