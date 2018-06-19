﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;


namespace HoloToolkit.Unity
{
    /// <summary>
    /// HandsManager determines if the hand is currently detected or not.
    /// </summary>
    public partial class HandsManager : Singleton<HandsManager>
    {
        /// <summary>
        /// HandDetected tracks the hand detected state.
        /// Returns true if the list of tracked hands is not empty.
        /// </summary>
        public bool HandDetected
        {
            get { return trackedHands.Count > 0; }
        }

        /// <summary>
        /// Occurs when users hand is detected or lost.
        /// </summary>
        /// <param name="handDetected">True if a hand is Detected, else false.</param>
        public delegate void HandInViewDelegate(bool handDetected);
        public event HandInViewDelegate HandInView;

        private HashSet<uint> trackedHands = new HashSet<uint>();

        private void Awake()
        {
            UnityEngine.XR.WSA.Input.InteractionManager.SourceDetected += InteractionManager_SourceDetected;
            UnityEngine.XR.WSA.Input.InteractionManager.SourceLost += InteractionManager_SourceLost;
        }

        private void InteractionManager_SourceDetected(UnityEngine.XR.WSA.Input.InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != UnityEngine.XR.WSA.Input.InteractionSourceKind.Hand)
            {
                return;
            }

            trackedHands.Add(state.source.id);

            if (HandInView != null)
            {
                HandInView(HandDetected);
            }
        }

        private void InteractionManager_SourceLost(UnityEngine.XR.WSA.Input.InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != UnityEngine.XR.WSA.Input.InteractionSourceKind.Hand)
            {
                return;
            }

            if (trackedHands.Contains(state.source.id))
            {
                trackedHands.Remove(state.source.id);

                if (HandInView != null)
                {
                    HandInView(HandDetected);
                }
            }
        }

        private void OnDestroy()
        {
            UnityEngine.XR.WSA.Input.InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
            UnityEngine.XR.WSA.Input.InteractionManager.SourceLost -= InteractionManager_SourceLost;
        }
    }
}