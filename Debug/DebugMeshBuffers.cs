using ImGuiNET;
using CoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using ArcticFoxEngine.Components;

namespace ArcticFoxEngine.Debug {
	internal class DebugMeshBuffers : DebugWindow {

		int maxEntries = 64;
		int cubeCount = 0;
		int quadCount = 0;

		float colorStride = 0.781f;

		internal override string name => "Mesh Buffer Viewer";
		internal override void Render() {

			GeometryResources geometry = Scene.activeScene.mainGeometry;
			MeshRenderer delMeshFilter = null;

			ImGui.InputInt("Max Display Entries", ref maxEntries);

			int[] meshFilterIndex;

			#region Vertex null buttons

			#region Finding mesh filter indices

			meshFilterIndex = new int[maxEntries];
			for (int i = 0; i < Math.Min(geometry.vertexGap.Length, maxEntries); i++) {

				meshFilterIndex[i] = -1;
				for (int n = 0; n < geometry.meshRenderers.Count; n++) {
					int currentBufferStart = geometry.meshRendererPositions[n].vbStart;
					int bufferLength = geometry.meshRenderers[n].mesh.vertices.Length;
					if (i >= currentBufferStart && i < currentBufferStart + bufferLength) {
						meshFilterIndex[i] = n;
					}
				}

			}

			#endregion
			#region Drawing buttons

			ImGui.Text("Vertex:");
			for (int i = 0; i < Math.Min(geometry.vertexGap.Length, maxEntries); i++) {

				ImGui.SameLine();

				Vector4 buttonCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				Vector4 buttonHoverCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				Vector4 buttonActiveCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				string objectName = " ";
				if (meshFilterIndex[i] != -1) {

					double ratio = (colorStride * (1 + Math.Sqrt(5)) / 2.0);
					double hue = (meshFilterIndex[i] % ratio) / ratio;

					System.Drawing.Color col;
					col = MathUtil.HsvToRgb(360.0 * hue, 0.6, 0.6);
					buttonCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

					col = MathUtil.HsvToRgb(360.0 * hue, 0.7, 0.7);
					buttonHoverCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

					col = MathUtil.HsvToRgb(360.0 * hue, 0.8, 0.8);
					buttonActiveCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

				}
				else {
					objectName = geometry.vertexGap[i].ToString();
					if (geometry.vertexGap[i] == -1) {
						objectName = "-";
					}
				}
				ImGui.PushID(i + 1);
				ImGui.PushStyleColor(ImGuiCol.Button, buttonCol);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, buttonHoverCol);
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, buttonActiveCol);
				bool delCurrent = ImGui.Button(objectName);
				ImGui.PopStyleColor(3);
				ImGui.PopID();

				if (delCurrent == true && meshFilterIndex[i] != -1) {
					delMeshFilter = geometry.meshRenderers[meshFilterIndex[i]];
				}


			}

			#endregion

			#endregion
			#region Index null buttons

			#region Finding mesh filter indices

			meshFilterIndex = new int[maxEntries];
			for (int i = 0; i < Math.Min(geometry.indexGap.Length, maxEntries); i++) {

				meshFilterIndex[i] = -1;
				for (int n = 0; n < geometry.meshRenderers.Count; n++) {
					int currentBufferStart = geometry.meshRendererPositions[n].ibStart;
					int bufferLength = geometry.meshRenderers[n].mesh.indices.Length;
					if (i >= currentBufferStart && i < currentBufferStart + bufferLength) {
						meshFilterIndex[i] = n;
					}
				}

			}

			#endregion
			#region Drawing buttons

			ImGui.Text("Index :");
			for (int i = 0; i < Math.Min(geometry.indexGap.Length, maxEntries); i++) {
				ImGui.SameLine();

				Vector4 buttonCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				Vector4 buttonHoverCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				Vector4 buttonActiveCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				string objectName = " ";
				if (meshFilterIndex[i] != -1) {

					double ratio = (colorStride * (1 + Math.Sqrt(5)) / 2.0);
					double hue = (meshFilterIndex[i] % ratio) / ratio;

					System.Drawing.Color col;
					col = MathUtil.HsvToRgb(360.0 * hue, 0.6, 0.6);
					buttonCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

					col = MathUtil.HsvToRgb(360.0 * hue, 0.7, 0.7);
					buttonHoverCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

					col = MathUtil.HsvToRgb(360.0 * hue, 0.8, 0.8);
					buttonActiveCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);
					
				}
				else {
					objectName = geometry.indexGap[i].ToString();
					if (geometry.indexGap[i] == -1) {
						objectName = "-";
					}
				}

				ImGui.PushID(i + 1);
				ImGui.PushStyleColor(ImGuiCol.Button, buttonCol);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, buttonHoverCol);
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, buttonActiveCol);
				bool delCurrent = ImGui.Button(objectName);
				ImGui.PopStyleColor(3);
				ImGui.PopID();

				if (delCurrent == true && meshFilterIndex[i] != -1) {
					delMeshFilter = geometry.meshRenderers[meshFilterIndex[i]];
				}


			}

			#endregion

			#endregion
			#region Object null buttons

			#region Finding mesh filter indices

			meshFilterIndex = new int[maxEntries];
			for (int i = 0; i < Math.Min(geometry.objectGap.Length, maxEntries); i++) {

				meshFilterIndex[i] = -1;
				for (int n = 0; n < geometry.meshRenderers.Count; n++) {
					int currentBufferStart = geometry.meshRendererPositions[n].obStart;
					int bufferLength = 1;
					if (i >= currentBufferStart && i < currentBufferStart + bufferLength) {
						meshFilterIndex[i] = n;
					}
				}

			}

			#endregion
			#region Drawing buttons

			ImGui.Text("Object:");
			for (int i = 0; i < Math.Min(geometry.objectGap.Length, maxEntries); i++) {
				ImGui.SameLine();

				Vector4 buttonCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				Vector4 buttonHoverCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				Vector4 buttonActiveCol = new Vector4(0.2f, 0.2f, 0.2f, 1f);
				string objectName = " ";
				if (meshFilterIndex[i] != -1) {

					double ratio = (colorStride * (1 + Math.Sqrt(5)) / 2.0);
					double hue = (meshFilterIndex[i] % ratio) / ratio;

					System.Drawing.Color col;
					col = MathUtil.HsvToRgb(360.0 * hue, 0.6, 0.6);
					buttonCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

					col = MathUtil.HsvToRgb(360.0 * hue, 0.7, 0.7);
					buttonHoverCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

					col = MathUtil.HsvToRgb(360.0 * hue, 0.8, 0.8);
					buttonActiveCol = new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1f);

				}
				else {
					objectName = geometry.objectGap[i].ToString();
					if (geometry.objectGap[i] == -1) {
						objectName = "-";
					}
				}

				ImGui.PushID(i + 1);
				ImGui.PushStyleColor(ImGuiCol.Button, buttonCol);
				ImGui.PushStyleColor(ImGuiCol.ButtonHovered, buttonHoverCol);
				ImGui.PushStyleColor(ImGuiCol.ButtonActive, buttonActiveCol);
				bool delCurrent = ImGui.Button(objectName);
				ImGui.PopStyleColor(3);
				ImGui.PopID();

				if (delCurrent == true && meshFilterIndex[i] != -1) {
					delMeshFilter = geometry.meshRenderers[meshFilterIndex[i]];
				}


			}

			#endregion

			#endregion


			if (delMeshFilter != null) {
				geometry.RemoveMesh(delMeshFilter);
			}

			ImGui.Separator();
			if (ImGui.Button("Add Cube") == true) {

				Log.Info("Adding Cube");
				GameObject cubeObj = Scene.activeScene.InstantiateObject("Cube #" + cubeCount);
				MeshRenderer cubeMeshFilter = cubeObj.AddComponent<MeshRenderer>();
				cubeMeshFilter.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Cube));
				cubeCount++;

			}
			ImGui.SameLine();
			if (ImGui.Button("Add Quad") == true) {

				Log.Info("Adding quad");
				GameObject quadObj = Scene.activeScene.InstantiateObject("Quad #" + quadCount);
				MeshRenderer quadMeshFilter = quadObj.AddComponent<MeshRenderer>();
				quadMeshFilter.SetMesh(Mesh.CreatePrimitive(Mesh.Primitive.Quad));
				quadCount++;

			}
		}


	}
}
