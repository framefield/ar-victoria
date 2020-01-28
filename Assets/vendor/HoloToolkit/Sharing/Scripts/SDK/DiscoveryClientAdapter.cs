// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Sharing
{
    public class DiscoveryClientAdapter : DiscoveryClientListener
    {
        public event Action<DiscoveredSystem> DiscoveredEvent;
        public event Action<DiscoveredSystem> LostEvent;

        public override void OnRemoteSystemDiscovered(DiscoveredSystem remoteSystem) 
        {
            if (DiscoveredEvent != null)
            {
                DiscoveredEvent(remoteSystem);
            }
        }

        public override void OnRemoteSystemLost(DiscoveredSystem remoteSystem)
        {
            if (LostEvent != null)
            {
                LostEvent(remoteSystem);
            }
        }
    }
}
