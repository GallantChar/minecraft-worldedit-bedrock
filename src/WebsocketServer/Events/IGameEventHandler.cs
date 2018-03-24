using System.Collections.Generic;
using MinecraftPluginServer.Protocol.Response;

namespace MinecraftPluginServer
{
    public interface IGameEventHandler
    {
        List<GameEvent> CanHandle();
        Result Handle(Response message);    
    }
}