using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinecraftPluginServer;
using WorldEdit.Input;
using WorldEdit.Output;

namespace WorldEdit
{
    public class PluginServer
    {
        private static bool keepRunning = true;
        private IMinecraftCommandService minecraftService;
        private List<IGameEventHandler> GameHandlers { get; } = new List<IGameEventHandler>();
        private List<IHotkeyHandler> HotkeyHandlers { get; } = new List<IHotkeyHandler>();


        public string CommandList
        {
            get
            {
                return String.Join("",
                    GameHandlers.Where(a => a is ChatHandler).Select(h => String.Format("{0,-13}  {1}\n",
                        ((ChatHandler) h).ChatCommand, ((ChatHandler) h).ChatCommandDescription)));
            }
        }
        public void Start(string serverName, string portNumber)
        {
            using (var server = new SocketServer("ws://0.0.0.0:"+portNumber)) // will stop on disposal.
            {
                server.Start();
                minecraftService = new MinecraftWebsocketCommandService(server);

                foreach (var gameHandler in GameHandlers)
                {
                    if (gameHandler is ISendCommand)
                    {
                        ((ISendCommand) gameHandler).CommandService = minecraftService;
                    }
                    server.AddHandler(gameHandler);
                }
                var eventSubscriptions=GameHandlers.SelectMany(a => a.CanHandle()).Distinct().OrderBy(a => a).ToList();
                server.AddHandler(new ConnectionHandler(minecraftService,serverName, eventSubscriptions ));
                var ahk = AutoHotKey.Run("hotkeys.ahk");
                AutoHotKey.Callback = s =>
                {
                    Console.WriteLine(s);
                    var args = s.Split(' ');
                    HandleHotKeys(args);
                };

                using (var cancelationToken = minecraftService.Run())
                {
                    while (keepRunning)
                        Thread.Sleep(500);
                    ahk.Terminate();
                    minecraftService.Status(serverName+" Shutting Down");
                    minecraftService.Wait();
                    minecraftService.ShutDown();
                    cancelationToken.Cancel();
                }
                server.Stop();
            }
        }

        public void Plugin(IGameEventHandler handler)
        {
            GameHandlers.Add(handler);
        }

        public void Plugin(IHotkeyHandler handler)
        {
            HotkeyHandlers.Add(handler);
        }

        private void HandleHotKeys(string[] args)
        {
            foreach (var handler in HotkeyHandlers)
            {
                if (handler.CanHandle(args))
                {
                    handler.Handle(args, minecraftService);
                }
            }
        }

        public void Stop()
        {
            keepRunning = false;
        }
    }
}