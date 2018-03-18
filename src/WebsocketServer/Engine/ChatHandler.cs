﻿using System.Collections.Generic;
using System.Linq;
using MinecraftPluginServer;
using MinecraftPluginServer.Protocol.Response;
using WorldEdit.Output;

namespace WorldEdit
{
    public class ChatHandler : IGameEventHander, ISendCommand
    {
        //private readonly CommandControl _cmdHandler;
        protected string ChatCommand;

        public List<GameEvent> CanHandle()
        {
            return new List<GameEvent>(){ GameEvent.PlayerMessage};
        }

        public Result Handle(Response message)
        {
            if (message.body.properties.MessageType.Equals("chat"))
            {
                var args = message.body.properties.Message.Split(' ');
                if (args.Length >= 1 && args[0].Equals(ChatCommand))
                {
                    HandleMessage(args.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray());
                }
            }
            return new Result();
        }

        public IMinecraftCommandService CommandService { get; set; }

        public void Command(string command)
        {
            CommandService.Command(command);
        }

        public virtual void HandleMessage(IEnumerable<string> args)
        {
        }
    }
}