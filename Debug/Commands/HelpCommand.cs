using CoolClassLibrary;

namespace ArcticFoxEngine.Debug.Commands {
	internal class HelpCommand : Command {
		public string name => "help";

		public void Execute(string[] args) {
			Log.Info("Listing commands...");
			for (int i = 0; i < CommandController.commands.Count; i++) {
				Log.Raw(CommandController.commands[i].name);
			}
		}

		public string[] GetNextArgument(string[] args) {
			return new string[0];
		}
	}
}
