using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcticFoxEngine.Nodes;

namespace ArcticFoxEngine.Debug.Commands {
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
