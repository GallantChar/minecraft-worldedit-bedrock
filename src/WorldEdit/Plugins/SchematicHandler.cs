using System.Collections.Generic;
using System.Linq;
using WorldEdit.Input;
using WorldEdit.Schematic;

namespace WorldEdit
{
    public class SchematicHandler : ChatHandler
    {
        private List<SavedPosition> SavedPositions => _savedPositionHandler.SavedPositions.Positions;
        private readonly SavedPositionHandler _savedPositionHandler;
        public SchematicHandler(SavedPositionHandler posHandler)
        {
            ChatCommand = "schematic";
            ChatCommandDescription = "Schematic import utility.";
            _savedPositionHandler = posHandler;
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            var position = CommandService.GetLocation();
            new SchematicProcessor(CommandService).SchematicCommandProcessor(position, SavedPositions, args.Skip(1));
        }
    }
}