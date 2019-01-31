﻿using System;
using System.Collections.Generic;
using System.Linq;
using Pegasus.Database;
using Pegasus.Network;
using Pegasus.Network.Packet;

namespace Pegasus.Social
{
    public static class ConversationManager
    {
        public static void SendMessage(Session sender, string recipient, string message)
        {
            List<Session> recipientSessions = NetworkManager.FindSession(recipient).ToList();
            if (recipientSessions.Count == 0)
            {
                var conversationError = new NetworkObject();
                conversationError.AddField(0, NetworkObjectField.CreateIntField((int)ConversationAction.NotOnline));
                conversationError.AddField(1, NetworkObjectField.CreateStringField(recipient));
                sender.EnqueuePacket(new ServerObjectPacket(ObjectOpcode.Conversation, conversationError, false));
                return;
            }

            foreach (Session session in recipientSessions)
            {
                var receiveTell = new NetworkObject();
                receiveTell.AddField(0, NetworkObjectField.CreateIntField((int)ConversationAction.ReceiveTell));
                receiveTell.AddField(1, NetworkObjectField.CreateStringField(sender.Account.Username));
                receiveTell.AddField(2, NetworkObjectField.CreateStringField(message));
                session.EnqueuePacket(new ServerObjectPacket(ObjectOpcode.Conversation, receiveTell, false));
            }

            List<Session> senderSessions = NetworkManager.FindSession(sender.Account.Username).ToList();
            foreach (Session session in senderSessions)
            {
                var sendTell = new NetworkObject();
                sendTell.AddField(0, NetworkObjectField.CreateIntField((int)ConversationAction.SendTell));
                sendTell.AddField(1, NetworkObjectField.CreateStringField(recipient));
                sendTell.AddField(2, NetworkObjectField.CreateStringField(message));
                session.EnqueuePacket(new ServerObjectPacket(ObjectOpcode.Conversation, sendTell, false));
            }
        }
    }
}
