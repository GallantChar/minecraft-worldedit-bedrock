using System.Collections.Generic;

namespace WorldEdit
{
    public class TestCommandHandler : ChatHandler
    {
        public TestCommandHandler()
        {
            ChatCommand = "thaw";
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            Command(string.Join(" ", args));
        }
    }
}