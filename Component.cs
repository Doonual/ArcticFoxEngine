using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcticFoxEngine {
	public class Component {

		public GameObject gameObject {
			get;
			internal set;
		}

		public virtual void Start() {

		}
		public virtual void Update() {

		}
		public virtual void OnRender() {

		}
		public virtual void Debug() {

		}

	}
}
