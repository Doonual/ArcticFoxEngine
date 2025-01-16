namespace ArcticFoxEngine.Rendering {
	public struct ProjectionInfo {

		public Matrix projectionMatrix; // 64 bytes

		public int screenWidth;		 // 4 bytes
		public int screenHeight;		// 4 bytes
		public float aspectRatio;	  // 4 bytes

	};
}
