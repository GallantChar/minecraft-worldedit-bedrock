using System.Collections.Generic;
using System.Linq;
using WorldEdit.Schematic;

namespace WorldEdit
{
    public class SchematicHandler : ChatHandler
    {
        public SchematicHandler()
        {
            ChatCommand = "schematic";
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            var position = CommandService.GetLocation();
            new SchematicProcessor(CommandService).SchematicCommandProcessor(position, args.Skip(1));
        }
    }
}