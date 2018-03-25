using System;
using MinecraftPluginServer.Protocol.Response;

namespace MinecraftPluginServer
{
    public static class EnumExtensions
    {
        public  static MessagePurpose ToMessagePurpose(this string value)
        {
                try
                {
                    return (MessagePurpose) Enum.Parse(typeof(MessagePurpose), value, true);
                }
                catch
                {
                    return MessagePurpose.Error;
                }
        }
        public  static GameEvent ToEvent(this string eventMessage)
        {
            return (GameEvent)Enum.Parse(typeof(GameEvent), eventMessage, true);
        }

    }
}