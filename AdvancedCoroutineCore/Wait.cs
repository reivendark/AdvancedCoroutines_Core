// <copyright file="Wait.cs" company="Parallax Pixels">
// Copyright (c) 2016 All Rights Reserved
// </copyright>
// <author>Michael Kulikov</author>
// <date>07/05/2016 19:09:58 AM </date>

using System;

namespace AdvancedCoroutines
{
    public struct Wait
    {
        internal float Seconds;
        internal WaitTypeInternal waitTypeInternal;

        public enum WaitType
        {
            ForEndOfUpdate,
            ForEndOfFrame,
        }

        internal enum WaitTypeInternal
        {
            ForEndOfFrame,
            ForEndOfUpdate,
            ForTime,
        }

        /// <summary>
        /// Wait parameter for AdvancedCoroutines
        /// </summary>
        /// <param name="waitType">What to wait</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws when 'waitType' doesn't exist in WaitType enum</exception>
        public Wait(WaitType waitType)
        {
            Seconds = 0;
            switch (waitType)
            {
                case WaitType.ForEndOfUpdate:
                    waitTypeInternal = WaitTypeInternal.ForEndOfUpdate;
                    break;
                case WaitType.ForEndOfFrame:
                    waitTypeInternal = WaitTypeInternal.ForEndOfFrame;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Wait parameter for AdvancedCoroutines
        /// </summary>
        /// <param name="seconds">Amount of time to wait in seconds</param>
        public Wait(float seconds)
        {
            waitTypeInternal = WaitTypeInternal.ForTime;
            Seconds = seconds;
        }
    }
}