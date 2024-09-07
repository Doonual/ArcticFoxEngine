using ArcticFoxEngine.Nodes;
using ArcticFoxEngine.Debug.Commands;
using ArcticFoxEngine.Testing.SceneTest;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace ArcticFoxEngine.Testing.ChildTest {
	public class ChildTestNode : Node {

		internal override string debugName => "Child Test";

		public ChildTestNode() : base() {


			CreateChild<CameraController>();

			Node platformsNode = CreateChild<EmptyNode>("Platforms");
			

			#region Block tower

			Node blockPlatform = platformsNode.CreateChild<EmptyNode>("Block tower platform");
			blockPlatform.CreateChild<Transform>();
			blockPlatform.transformChild.position = new Vector3(0f, -0.5f, 0f);
			blockPlatform.transformChild.scale = new Vector3(5f, 1f, 5f);
			blockPlatform.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			Node blockTowerA = CreateChild<EmptyNode>("Block Tower A");
			blockTowerA.CreateChild<Transform>();
			blockTowerA.transformChild.position = new Vector3(0f, 0.5f, 0f);
			blockTowerA.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			Node blockTowerB = blockTowerA.CreateChild<EmptyNode>("Block Tower B");
			blockTowerB.CreateChild<Transform>();
			MeshRenderer mrB = blockTowerB.CreateChild<MeshRenderer>();
			mrB.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			mrB.SetRenderPipeline("mandelbrot");
			blockTowerB.transformChild.position = new Vector3(0f, 2f, 0f);

			Node blockTowerC = blockTowerB.CreateChild<EmptyNode>("Block Tower C");
			blockTowerC.CreateChild<Transform>();
			blockTowerC.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			blockTowerC.transformChild.position = new Vector3(0f, 2f, 0f);

			#endregion
			#region Cube Cube

			Node cubePlatform = platformsNode.CreateChild<EmptyNode>("Cube cube platform");
			cubePlatform.CreateChild<Transform>();
			cubePlatform.transformChild.position = new Vector3(-7f, -0.5f, 1f);
			cubePlatform.transformChild.scale = new Vector3(7f, 1f, 7f);
			cubePlatform.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			Node cubeParent = CreateChild<EmptyNode>("Cube Cube");
			cubeParent.CreateChild<Transform>();
			cubeParent.transformChild.position = new Vector3(-7f, 4f, 1f);
			Vector3[] positions = new Vector3[] {
				new Vector3(-2f, -2f, -2f),
				new Vector3(2f, -2f, -2f),
				new Vector3(-2f, 2f, -2f),
				new Vector3(2f, 2f, -2f),
				new Vector3(-2f, -2f, 2f),
				new Vector3(2f, -2f, 2f),
				new Vector3(-2f, 2f, 2f),
				new Vector3(2f, 2f, 2f),
			};
			for (int i = 0; i < positions.Length; i++) {
				Node cubeObj = cubeParent.CreateChild<EmptyNode>("Cube #" + i);
				cubeObj.CreateChild<Transform>();
				cubeObj.transformChild.position = positions[i];
				cubeObj.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
			}
			cubeParent.CreateChild<CubeRoller>();

			#endregion
			#region Cube Wheel

			float cubeWheelSize = 8f;

			Node cubeWheelPlatform = platformsNode.CreateChild<EmptyNode>("Cube wheel platform");
			cubeWheelPlatform.CreateChild<Transform>();
			cubeWheelPlatform.transformChild.position = new Vector3(cubeWheelSize / 2f + 3.5f, -0.5f, (cubeWheelSize / 2f) - 2.5f);
			cubeWheelPlatform.transformChild.scale = new Vector3(cubeWheelSize, 1f, cubeWheelSize);
			cubeWheelPlatform.CreateChild<MeshRenderer>().SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));

			CubeWheel cubeWheel = CreateChild<CubeWheel>("Cube Wheel");
			cubeWheel.transformChild.position.x = cubeWheelSize / 2f + 3.5f;
			cubeWheel.Propagate(5, 4f);
			cubeWheel.Stop();

			#endregion


			SetName("Child Test Node");
			Enable();
		}

		public override void Debug() {
			if (ImGui.Button("Recurse") == true) {
				Recurse();
			}
		}

		public void Recurse() {

			Node nextCopy = CreateChild<EmptyNode>("Next Copy");

			Transform nextTransform = nextCopy.CreateChild<Transform>();
			nextTransform.position = new Vector3(-5.25f, 0f, -6f);
			nextTransform.scale = new Vector3(0.5f, 0.5f, 0.5f);

			ChildTestNode nextChildTestNode = nextCopy.CreateChild<ChildTestNode>();
			nextChildTestNode.GetChild(0).Disable();

		}

	}
}
