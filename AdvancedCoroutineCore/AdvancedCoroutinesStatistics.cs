// <copyright file="AdvancedCoroutinesStatistics.cs" company="Parallax Pixels">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Michael Kulikov</author>
// <date>07/05/2016 19:09:58 AM </date>

using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCoroutines.Statistics
{
    /// <summary>
    /// Collect AdvancedCoroutines statistics
    /// </summary>
    public static class AdvancedCoroutinesStatistics
    {
        private static readonly Dictionary<Routine, string> _rouitinesStatistics;

        /// <summary>
        /// Total amount of started coroutines;
        /// </summary>
        public static int TotalCoroutinesStarts { get; private set; }

        /// <summary>
        /// Total amount of stopped coroutines;
        /// </summary>
        public static int TotalCoroutinesStops { get; private set; }

        static AdvancedCoroutinesStatistics()
        {
            _rouitinesStatistics = new Dictionary<Routine, string>();
            TotalCoroutinesStarts = 0;
            TotalCoroutinesStops = 0;
        }

        /// <summary>
        /// Add Routine data to statistics dictionary
        /// </summary>
        /// <param name="routine">Unique Routine</param>
        /// <param name="stackTrace">Call stack for tracing method call</param>
        /// <exception cref="ArgumentException">Throws when same coroutine runs twice without stop</exception>
        /// <exception cref="ArgumentException">Throws when stackTrace is 'null' or empty</exception>
        public static void Add(Routine routine, string stackTrace)
        {
            if(Routine.IsNull(routine)) return;

            if(_rouitinesStatistics.ContainsKey(routine))
                throw new ArgumentException("Routine already contained in _rouitinesStatistics");

            if(string.IsNullOrEmpty(stackTrace))
                throw new ArgumentException("stackTrack is null or empty");

            _rouitinesStatistics.Add(routine, stackTrace);
            TotalCoroutinesStarts++;
        }

        /// <summary>
        /// Get staticsitcs data for Routines
        /// </summary>
        /// <returns>Stat data dictionary where Key is Routine and Value is array of stack calls</returns>
        public static Dictionary<Routine, string[]> GetStatistics()
        {
            return _rouitinesStatistics.ToDictionary(statistic => statistic.Key, statistic => ParseStackTrace(statistic.Value));
        }

        /// <summary>
        /// Erase all collected staticstics data
        /// </summary>
        public static void Erase()
        {
            _rouitinesStatistics.Clear();
            TotalCoroutinesStarts = 0;
            TotalCoroutinesStops = 0;
        }

        internal static void Remove(Routine routine)
        {
            if(_rouitinesStatistics.ContainsKey(routine))
            {
                _rouitinesStatistics.Remove(routine);
                TotalCoroutinesStops++;
            }
        }

        private static string[] ParseStackTrace(string stackTrace)
        {
            return stackTrace.Split('\n');
        }
    }
}