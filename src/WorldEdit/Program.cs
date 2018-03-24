﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MinecraftPluginServer;

namespace WorldEdit
{
    internal class Program
    {
        private static void Main()
        {
            var pluginServer = new PluginServer();


            //game event handlers
            pluginServer.Plugin(new SavedPositionHandler());
            pluginServer.Plugin(new DrainHandler());
            pluginServer.Plugin(new ThawHandler());
            pluginServer.Plugin(new TestCommandHandler());
            var createHandler = new CreateHandler();
            pluginServer.Plugin(createHandler);
            pluginServer.Plugin(new SchematicHandler());
            pluginServer.Plugin(new ChatLogHandler());
            pluginServer.Plugin(new RunCreatesHandler(createHandler));
            //local hotkey handlers.
            pluginServer.Plugin(new RadiusHandler());
            pluginServer.Plugin(new LandSculptHandler());
            pluginServer.Plugin(new HelpHandler(pluginServer.CommandList));



            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                pluginServer.Stop();
            };

            pluginServer.Start("WorldEdit", "12112");
        }
    }

    public class ChatLogHandler : ChatHandler
    {
        public ChatLogHandler()
        {
            ChatCommand = "create";
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            using (var file = File.AppendText(".\\create.log"))
            {
                file.WriteLine(args.Aggregate((a,b)=>a+=b +" "));
            }
        }
    }

    public class RunCreatesHandler : ChatHandler
    {
        private readonly CreateHandler _handler;

        public RunCreatesHandler(CreateHandler handler)
        {
            _handler = handler;
            ChatCommand = "script";
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            var lines = File.ReadAllLines($".\\{args.FirstOrDefault() ?? "worldedit"}.log");

            foreach (var command in lines)
            {
                _handler.HandleMessage(command.Split(' ').Where(a => !string.IsNullOrWhiteSpace(a)).ToArray());
            }
            
        }
    }

}