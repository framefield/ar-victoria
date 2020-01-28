// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Sharing
{
    /// <summary>
    /// Allows users of the SessionManager to register to receive event callbacks without
    /// having their classes inherit directly from SessionManagerListener
    /// </summary>
    public class SessionManagerAdapter : SessionManagerListener
    {
        public event System.Action<Session> CreateSucceededEvent;
        public event System.Action<XString> CreateFailedEvent;
        public event System.Action<Session> SessionAddedEvent;
        public event System.Action<Session> SessionClosedEvent;
        public event System.Action<Session, User> UserJoinedSessionEvent;
        public event System.Action<Session, User> UserLeftSessionEvent;
        public event System.Action<Session, User> UserChangedEvent;
        public event System.Action ServerConnectedEvent;
        public event System.Action ServerDisconnectedEvent;

        public SessionManagerAdapter() { }

        public override void OnCreateSucceeded(Session newSession)
        {
            Profile.BeginRange("OnCreateSucceeded");
            if (CreateSucceededEvent != null)
            {
                CreateSucceededEvent(newSession);
            }
            Profile.EndRange();
        }

        public override void OnCreateFailed(XString reason)
        {
            Profile.BeginRange("OnCreateFailed");
            if (CreateFailedEvent != null)
            {
                CreateFailedEvent(reason);
            }
            Profile.EndRange();
        }

        public override void OnSessionAdded(Session newSession)
        {
            Profile.BeginRange("OnSessionAdded");
            if (SessionAddedEvent != null)
            {
                SessionAddedEvent(newSession);
            }
            Profile.EndRange();
        }

        public override void OnSessionClosed(Session session)
        {
            Profile.BeginRange("OnSessionClosed");
            if (SessionClosedEvent != null)
            {
                SessionClosedEvent(session);
            }
            Profile.EndRange();
        }

        public override void OnUserJoinedSession(Session session, User newUser)
        {
            Profile.BeginRange("OnUserJoinedSession");
            if (UserJoinedSessionEvent != null)
            {
                UserJoinedSessionEvent(session, newUser);
            }
            Profile.EndRange();
        }

        public override void OnUserLeftSession(Session session, User user)
        {
            Profile.BeginRange("OnUserLeftSession");
            if (UserLeftSessionEvent != null)
            {
                UserLeftSessionEvent(session, user);
            }
            Profile.EndRange();
        }

        public override void OnUserChanged(Session session, User user)
        {
            Profile.BeginRange("OnUserChanged");
            if (UserChangedEvent != null)
            {
                UserChangedEvent(session, user);
            }
            Profile.EndRange();
        }

        public override void OnServerConnected()
        {
            Profile.BeginRange("OnServerConnected");
            if (ServerConnectedEvent != null)
            {
                ServerConnectedEvent();
            }
            Profile.EndRange();
        }

        public override void OnServerDisconnected()
        {
            Profile.BeginRange("OnServerDisconnected");
            if (ServerDisconnectedEvent != null)
            {
                ServerDisconnectedEvent();
            }
            Profile.EndRange();
        }
    }
}