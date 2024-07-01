using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.D3DCompiler;

namespace ArcticFoxEngine {
	internal class StandardIncludeHandler : CppObject, Include {
		public IDisposable Shadow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		internal StandardIncludeHandler() : base(new IntPtr(1)) { }

		public void Close(Stream stream) { }


		public Stream Open(IncludeType type, string fileName, Stream parentStream) {
			throw new NotImplementedException();
		}
	}
}
