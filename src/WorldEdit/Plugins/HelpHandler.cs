using System.Collections.Generic;

namespace WorldEdit
{
    public class HelpHandler : ChatHandler
    {
        private string help;
        public HelpHandler(string commandList)
        {
            help = commandList;
            ChatCommand = "help";
            ChatCommandDescription = "Displays the help.";
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            CommandService.Status("\nHELP\n" + help);
        }
    }
}