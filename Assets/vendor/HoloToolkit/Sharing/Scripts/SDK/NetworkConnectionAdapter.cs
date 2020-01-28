// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of NetworkConnection to register to receive event callbacks without
    /// having their classes inherit directly from NetworkConnectionListener
    /// </summary>
    public class NetworkConnectionAdapter : NetworkConnectionListener
    {
        public event System.Action<NetworkConnection> ConnectedCallback;
        public event System.Action<NetworkConnection> ConnectionFailedCallback;
        public event System.Action<NetworkConnection> DisconnectedCallback;
        public event System.Action<NetworkConnection, NetworkInMessage> MessageReceivedCallback;

        public NetworkConnectionAdapter() { }

        public override void OnConnected(NetworkConnection connection)
        {
            Profile.BeginRange("OnConnected");
            if (ConnectedCallback != null)
            {
                ConnectedCallback(connection);
            }
            Profile.EndRange();
        }

        public override void OnConnectFailed(NetworkConnection connection)
        {
            Profile.BeginRange("OnConnectFailed");
            if (ConnectionFailedCallback != null)
            {
                ConnectionFailedCallback(connection);
            }
            Profile.EndRange();
        }

        public override void OnDisconnected(NetworkConnection connection)
        {
            Profile.BeginRange("OnDisconnected");
            if (DisconnectedCallback != null)
            {
                DisconnectedCallback(connection);
            }
            Profile.EndRange();
        }

        public override void OnMessageReceived(NetworkConnection connection, NetworkInMessage message)
        {
            Profile.BeginRange("OnMessageReceived");
            if (MessageReceivedCallback != null)
            {
                MessageReceivedCallback(connection, message);
            }
            Profile.EndRange();
        }
    }
}