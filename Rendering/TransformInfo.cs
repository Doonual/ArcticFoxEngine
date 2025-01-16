namespace ArcticFoxEngine.Rendering {

	// Input structs must be 256 byte aligned!!

	public struct TransformInfo {
		public Matrix transformationMatrix; // 64 bytes
		public Matrix inverseTransformationMatrix;
	};
}
