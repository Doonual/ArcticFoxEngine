using ArcticFoxEngine.Nodes;
using CoolClassLibrary;

namespace ArcticFoxEngine.Gui.Commands {
	internal class AddObjectCommand : Command {
		public string name => "add_obj";

		public void Execute(string[] args) {

			Node.rootNode.CreateChild<BaseNode>();

		}

		public string[] GetNextArgument(string[] args) {

			if (args.Length == 0) {
				return new string[] { "$Tname" };
			}

			return new string[0];
		}
	}
}
