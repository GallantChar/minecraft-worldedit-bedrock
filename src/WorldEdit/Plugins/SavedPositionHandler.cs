using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WorldEdit.Input;

namespace WorldEdit
{
    public class SavedPositionHandler : ChatHandler
    {
        public readonly SavedPositionService SavedPositions = new SavedPositionService();

        public SavedPositionHandler()
        {
            ChatCommand = "pos";
            ChatCommandDescription = "Used to save and list positions. Type pos help for more details.";
        }

        public override void HandleMessage(IEnumerable<string> args)
        {
            HandlePositionCommand(args.Skip(1));
        }

        private void HandlePositionCommand(IEnumerable<string> args)
        {
            var name = args.ElementAtOrDefault(1);
            switch (args.ElementAtOrDefault(0))
            {
                case "add":
                    CommandAdd(name);
                    break;
                case "list":
                    CommandList(args.ElementAtOrDefault(2));
                    break;
                case "remove":
                    CommandRemove(name);
                    break;
                case "save":
                    CommandSave(args.ElementAtOrDefault(2));
                    break;
                case "load":
                    CommandLoad(args.ElementAtOrDefault(2));
                    break;
                case "help":
                    CommandHelp(args.ElementAtOrDefault(2));
                    break;
               
            }
        }

        private void CommandHelp(string subcommand)
        {
            switch (subcommand.ToLowerInvariant())
            {
                case "add":
                    CommandService.Status("pos add [name]: saves a new poisition with the specified [name]");
                    CommandService.Status("Type \"pos help\" for a complete list of saved positions \"pos\" commands.");
                    break;
                case "list":
                    CommandService.Status("pos list: list saved poisitions");
                    CommandService.Status("Type \"pos help\" for a complete list of saved positions \"pos\" commands.");
                    break;
                case "remove":
                    CommandService.Status("pos remove [name]: remove a saved position with [name]");
                    CommandService.Status("Type \"pos help\" for a complete list of saved positions \"pos\" commands.");
                    break;
                case "save":
                    CommandService.Status("pos save [name]: saves the list of positions to [name] file");
                    CommandService.Status("Type \"pos help\" for a complete list of saved positions \"pos\" commands.");
                    break;
                case "load":
                    CommandService.Status("pos load [name]: loads the list of positions from [name] file");
                    CommandService.Status("Type \"pos help\" for a complete list of saved positions \"pos\" commands.");
                    break;
                case "help":
                    CommandService.Status("pos help: shows the saved positions \"pos\" help");
                    break;
                default:
                    CommandService.Status("pos commands:");
                    CommandService.Status("pos add [name]: saves a new poisition with the specified [name]");
                    CommandService.Status("pos list: list saved poisitions");
                    CommandService.Status("pos remove [name]: remove a saved position with [name]");
                    CommandService.Status("pos save [name]: saves the list of positions to [name] file");
                    CommandService.Status("pos load [name]: loads the list of positions from [name] file");
                    CommandService.Status("pos help: shows this list of commands");
                    break;
            }

            throw new NotImplementedException();
        }

        private void CommandLoad(string filename)
        {
            try
            {
                filename = GetPositionFileName(filename);

                if (!File.Exists(filename)) return;

                SavedPositions.Positions.AddRange(
                    JsonConvert.DeserializeObject<List<SavedPosition>>(File.ReadAllText(filename)));

                CommandService.Status("Positions loaded.");
            }
            catch(Exception ex)
            {
                CommandService.Status("There was a problem loading the saved positions.");
                // TODO: Log Error.
            }
        }

        private string GetPositionFileName(string filename)
        {
            var autoCreate = false;
            var positionFolder = Path.GetFullPath(ConfigurationManager.AppSettings["positions"]);
            if (!Directory.Exists(positionFolder))
            {
                Directory.CreateDirectory(positionFolder);
            }

            if (String.IsNullOrEmpty(filename))
            {
                filename = "saved.json";
                autoCreate = true;
            }
            else
            {
                filename = new Regex($"[{Path.GetInvalidFileNameChars()}]").Replace(filename, "");
                if (!filename.EndsWith(".json")) filename += ".json";
            }

            var files = Directory.GetFiles(positionFolder, filename);

            if (files.Any()) return Path.GetFullPath(files[0]);
            if (autoCreate)
            {
                File.Create(filename).Close();
                return Path.Combine(positionFolder, filename);
            }
            CommandService.Status($"Unable to locate position file: {filename}");
            return null;
        }

        private void CommandSave(string filename)
        {
            filename = GetPositionFileName(filename);
            var json = JsonConvert.SerializeObject(SavedPositions.Positions);
            File.WriteAllText(filename, json);

            CommandService.Status("Positions saved.");
        }

        private void CommandRemove(string name)
        {
            var posToDelete = SavedPositions.Positions.SingleOrDefault(a => a.Name.Equals(name));
            if (posToDelete != null)
            {
                SavedPositions.Positions.Remove(posToDelete);
                CommandService.Status($"Removed position {posToDelete.Name}.");
            }
        }

        private void CommandList(string arg)
        {
            if ("files".Equals(arg, StringComparison.InvariantCultureIgnoreCase))
            {
                CommandListFiles();
            }

            if (SavedPositions.Positions.Any())
                CommandService.Status("Positions: " +
                                      SavedPositions.Positions.Select(b => $"\n{b.Name} at {b.Position}")
                                          .Aggregate((a, b) => a += b));
            else
                CommandService.Status("No saved positions.");
        }

        private void CommandListFiles()
        {
            CommandService.Status("No saved positions.");
            var files = Directory.GetFiles(ConfigurationManager.AppSettings["positions"], "*.json");

            CommandService
                .Status("Saved Position Files: \n" +
                        String.Join("\n", files.Select(Path.GetFileNameWithoutExtension).OrderBy(a => a)));
        }

        private void CommandAdd(string name)
        {
            var position = CommandService.GetLocation();
            SavedPositions.Positions.Add(new SavedPosition {Position = position, Name = name});
            CommandService.Status($"Saved postition {name} at {position}.");
        }
    }
}