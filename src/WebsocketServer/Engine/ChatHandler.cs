using System.Collections.Generic;
using System.Linq;
using MinecraftPluginServer;
using MinecraftPluginServer.Protocol.Response;
using WorldEdit.Output;

namespace WorldEdit
{
    public class ChatHandler : IGameEventHandler, ISendCommand
    {
        //private readonly CommandControl _cmdHandler;
        public string ChatCommand;
        public string ChatCommandDescription;


        public ChatHandler()
        {
            ChatCommandDescription = $"Runs the {ChatCommand} command.";
        }

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