using System;
using System.Collections.Generic;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Client.Models.Builders;
using TwitchLib.Communication.Events;

namespace MattsTwitchBot.Tests.IntegrationTests.TestHelpers
{
    public class FakeTwitchClient : ITwitchClient
    {

        public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        public void Connect()
        {
        }

        public void FakeRaiseMessage(string message, string channelName = null, string username = null)
        {
            if(channelName == null) channelName = "fakeChannelName";
            if(username == null) username = "fakeUsername";
            var twitchLibMessage = TwitchLibMessageBuilder.Create()
                .WithUsername(username)
                .Build();
            var chatMessage = ChatMessageBuilder.Create()
                .WithTwitchLibMessage(twitchLibMessage)
                .WithChannel(channelName)
                .WithMessage(message)
                .Build();

            var handler = OnMessageReceived;
            if (handler == null)
                throw new Exception("In FakeRaiseMessage: No handler has been defined.");
            handler.Invoke(this, new OnMessageReceivedArgs { ChatMessage = chatMessage });
        }

        #region Not Implemented
        public void Initialize(ConnectionCredentials credentials, string channel = null, char chatCommandIdentifier = '!',
            char whisperCommandIdentifier = '!', bool autoReListenOnExceptions = true)
        {
            throw new NotImplementedException();
        }

        public void SetConnectionCredentials(ConnectionCredentials credentials)
        {
            throw new NotImplementedException();
        }

        public void AddChatCommandIdentifier(char identifier)
        {
            throw new NotImplementedException();
        }

        public void AddWhisperCommandIdentifier(char identifier)
        {
            throw new NotImplementedException();
        }

        public void RemoveChatCommandIdentifier(char identifier)
        {
            throw new NotImplementedException();
        }

        public void RemoveWhisperCommandIdentifier(char identifier)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Reconnect()
        {
            throw new NotImplementedException();
        }

        public JoinedChannel GetJoinedChannel(string channel)
        {
            throw new NotImplementedException();
        }

        public void JoinChannel(string channel, bool overrideCheck = false)
        {
            throw new NotImplementedException();
        }

        public void JoinRoom(string channelId, string roomId, bool overrideCheck = false)
        {
            throw new NotImplementedException();
        }

        public void LeaveChannel(JoinedChannel channel)
        {
            throw new NotImplementedException();
        }

        public void LeaveChannel(string channel)
        {
            throw new NotImplementedException();
        }

        public void LeaveRoom(string channelId, string roomId)
        {
            throw new NotImplementedException();
        }

        public void OnReadLineTest(string rawIrc)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(JoinedChannel channel, string message, bool dryRun = false)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string channel, string message, bool dryRun = false)
        {
            throw new NotImplementedException();
        }

        public void SendQueuedItem(string message)
        {
            throw new NotImplementedException();
        }

        public void SendRaw(string message)
        {
            throw new NotImplementedException();
        }

        public void SendWhisper(string receiver, string message, bool dryRun = false)
        {
            throw new NotImplementedException();
        }

        public bool AutoReListenOnException { get; set; }
        public MessageEmoteCollection ChannelEmotes { get; }
        public ConnectionCredentials ConnectionCredentials { get; }
        public bool DisableAutoPong { get; set; }
        public bool IsConnected { get; }
        public bool IsInitialized { get; }
        public IReadOnlyList<JoinedChannel> JoinedChannels { get; }
        public bool OverrideBeingHostedCheck { get; set; }
        public WhisperMessage PreviousWhisper { get; }
        public string TwitchUsername { get; }
        public bool WillReplaceEmotes { get; set; }
        public event EventHandler<OnBeingHostedArgs> OnBeingHosted;
        public event EventHandler<OnChannelStateChangedArgs> OnChannelStateChanged;
        public event EventHandler<OnChatClearedArgs> OnChatCleared;
        public event EventHandler<OnChatColorChangedArgs> OnChatColorChanged;
        public event EventHandler<OnChatCommandReceivedArgs> OnChatCommandReceived;
        public event EventHandler<OnConnectedArgs> OnConnected;
        public event EventHandler<OnConnectionErrorArgs> OnConnectionError;
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;
        public event EventHandler<OnExistingUsersDetectedArgs> OnExistingUsersDetected;
        public event EventHandler<OnGiftedSubscriptionArgs> OnGiftedSubscription;
        public event EventHandler<OnHostingStartedArgs> OnHostingStarted;
        public event EventHandler<OnHostingStoppedArgs> OnHostingStopped;
        public event EventHandler OnHostLeft;
        public event EventHandler<OnIncorrectLoginArgs> OnIncorrectLogin;
        public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;
        public event EventHandler<OnLeftChannelArgs> OnLeftChannel;
        public event EventHandler<OnLogArgs> OnLog;

        public event EventHandler<OnMessageSentArgs> OnMessageSent;
        public event EventHandler<OnModeratorJoinedArgs> OnModeratorJoined;
        public event EventHandler<OnModeratorLeftArgs> OnModeratorLeft;
        public event EventHandler<OnModeratorsReceivedArgs> OnModeratorsReceived;
        public event EventHandler<OnNewSubscriberArgs> OnNewSubscriber;
        public event EventHandler<OnNowHostingArgs> OnNowHosting;
        public event EventHandler<OnRaidNotificationArgs> OnRaidNotification;
        public event EventHandler<OnReSubscriberArgs> OnReSubscriber;
        public event EventHandler<OnSendReceiveDataArgs> OnSendReceiveData;
        public event EventHandler<OnUserBannedArgs> OnUserBanned;
        public event EventHandler<OnUserJoinedArgs> OnUserJoined;
        public event EventHandler<OnUserLeftArgs> OnUserLeft;
        public event EventHandler<OnUserStateChangedArgs> OnUserStateChanged;
        public event EventHandler<OnUserTimedoutArgs> OnUserTimedout;
        public event EventHandler<OnWhisperCommandReceivedArgs> OnWhisperCommandReceived;
        public event EventHandler<OnWhisperReceivedArgs> OnWhisperReceived;
        public event EventHandler<OnWhisperSentArgs> OnWhisperSent;
        public event EventHandler<OnMessageThrottledEventArgs> OnMessageThrottled;
        public event EventHandler<OnWhisperThrottledEventArgs> OnWhisperThrottled;
        public event EventHandler<OnErrorEventArgs> OnError;
        public event EventHandler<OnReconnectedEventArgs> OnReconnected;
        public event EventHandler<OnVIPsReceivedArgs> OnVIPsReceived;
        public event EventHandler<OnCommunitySubscriptionArgs> OnCommunitySubscription;
        public event EventHandler<OnMessageClearedArgs> OnMessageCleared;
        public event EventHandler<OnRitualNewChatterArgs> OnRitualNewChatter;

        #endregion
    }
}