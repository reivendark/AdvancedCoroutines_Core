// <copyright file="Routine.cs" company="Parallax Pixels">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Michael Kulikov</author>
// <date>07/05/2016 19:09:58 AM </date>

using System;
using System.Collections;

namespace AdvancedCoroutines
{
    public class Routine
    {
        internal IEnumerator enumerator;
        internal object obj;

        //internal DateTime workTime;
        //internal DateTime startTime;
        internal float workTime;
        internal float startTime;

        internal bool isPaused;
        internal bool isStandalone;

        internal bool needToCheckEndOfUpdate = false;
        internal bool needToCheckPostRender = false;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumerator">Method representing coroutine logic</param>
        /// <param name="obj">object which Routine was created from</param>
        /// <exception cref="ArgumentNullException">Throws when object which Routine was created from is 'null'</exception>
        public Routine(IEnumerator enumerator, object obj)
        {
            if(enumerator == null)
            {
                throw new ArgumentNullException("Parameter 'enumerator' cannot be null");
            }

            isStandalone = (obj == null);

            this.enumerator = enumerator;
            this.obj = obj;
            //startTime = DateTime.UtcNow;
            startTime = 0f;
            isPaused = false;
            workTime = startTime;
        }

        /// <summary>
        /// Returns 'true' is Routine is Standalone or 'false' is it's linked to object
        /// </summary>
        public bool IsStandalone { get { return isStandalone; } }

        /// <summary>
        /// Set routine on pause
        /// </summary>
        public void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        /// Resume routine
        /// </summary>
        public void Resume()
        {
            isPaused = false;
        }

        /// <summary>
        /// Is routine paused
        /// </summary>
        /// <returns>Returns 'true' if Routine is on pause</returns>
        public bool IsPaused()
        {
            return isPaused;
        }

        /// <summary>
        /// Compare two Routins
        /// </summary>
        /// <param name="routine">Routine for comparison</param>
        /// <returns>Comparison result</returns>
        public bool Equals(Routine routine)
        {
            if(IsNull(routine))
                return false;
            return enumerator.Equals(routine.enumerator);
        }

        /// <summary>
        /// Check if routine is 'null'. <b>Attention!</b> All routines should be checked for 'null' through this method.
        /// </summary>
        /// <param name="routine">Routine object</param>
        /// <returns>returns 'true' if routine or its enumerator equals 'null'.</returns>
        public static bool IsNull(Routine routine)
        {
            return routine == null || routine.enumerator == null;
        }

        internal static void Erase(Routine routine)
        {
            routine.enumerator = null;
            routine.obj = null;
        }
    }
}