using ImGuiNET;
using CoolClassLibrary;
using ArcticFoxEngine.Backend;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ArcticFoxEngine.Debug {
	internal class DebugPerformance : DebugWindow {

		float msMax = 0.0f;
		float msMaxView = 0.0f;

		float msMin = 0.0f;
		float msMinView = 0.0f;

		float[] totalFrameTimes;
		Dictionary<string, float[]> profilerVals;
		float lastFrameTime = 0f;
		List<(string, float)> profilerUpdateQueue;

		int numElements;
		float plotMaxMs = 10f;
		bool updatePlot = true;
		bool updatePlotActual = true;

		bool showMoreOptions = true;
		

		internal override string name => "Performance";

		internal DebugPerformance() {

			numElements = 2000;

			totalFrameTimes = new float[numElements];
			profilerVals = new Dictionary<string, float[]>();
			profilerVals.Add("Untracked", new float[numElements]);

			profilerUpdateQueue = new List<(string, float)>();

		}

		internal void UpdateVal(string metric, float value) {
			if (profilerVals.ContainsKey(metric) == false) {
				profilerVals.Add(metric, new float[numElements]);
			}
			profilerUpdateQueue.Add((metric, value));
		}

		internal void FrameDone(float frameTime) {

			if (updatePlotActual == false) {
				updatePlotActual = updatePlot;
				return;
			}

			lastFrameTime = Profiler.frameTime;

			float ms = frameTime * 1000;
			if (ms > msMax) {
				msMax = ms;
				msMaxView = ms;
			}
			msMax = (msMax - ms) * 0.994f + ms;

			if (ms < msMin) {
				msMin = ms;
				msMinView = ms;
			}
			msMin = (msMin - ms) * 0.994f + ms;

			for (int i = 0; i < totalFrameTimes.Length - 1; i++) {
				totalFrameTimes[i] = totalFrameTimes[i + 1];
			}
			totalFrameTimes[totalFrameTimes.Length - 1] = ms;

			// Buffer everything except Misc
			for (int i = 0; i < profilerVals.Count; i++) {
				if (profilerVals.ElementAt(i).Key == "Untracked") { continue; }
				for (int n = 0; n < profilerVals.ElementAt(i).Value.Length - 1; n++) {
					profilerVals.ElementAt(i).Value[n] = profilerVals.ElementAt(i).Value[n + 1];
				}
			}
			// Add values from the queue
			while (profilerUpdateQueue.Count > 0) {
				profilerVals[profilerUpdateQueue[0].Item1][profilerVals[profilerUpdateQueue[0].Item1].Length - 1] = profilerUpdateQueue[0].Item2 * 1000;
				profilerUpdateQueue.RemoveAt(0);
			}

			// Calculate new Misc
			float unaccountedMs = ms;
			for (int i = 0; i < profilerVals.Count; i++) {
				if (profilerVals.ElementAt(i).Key == "Untracked") { continue; }
				unaccountedMs -= profilerVals.ElementAt(i).Value.Last();
			}

			for (int n = 0; n < profilerVals["Untracked"].Length - 1; n++) {
				profilerVals["Untracked"][n] = profilerVals["Untracked"][n + 1];
			}
			profilerVals["Untracked"][profilerVals["Untracked"].Length - 1] = unaccountedMs;

			updatePlotActual = updatePlot;


		}

		internal override void Render() {
			
			#region FPS Table

			double ms = totalFrameTimes.Last();

			ImGuiTableFlags flags = ImGuiTableFlags.Borders;
			if (ImGui.BeginTable("Performance Table", 3, flags) == true) {
				
				ImGui.TableSetupColumn("Worst");
				ImGui.TableSetupColumn("Current");
				ImGui.TableSetupColumn("Best");
				ImGui.TableHeadersRow();
				ImGui.TableNextColumn();

				ImGui.Text(msMaxView.ToString("F") + " ms");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
				ImGui.TableNextColumn();
				ImGui.Text(ms.ToString("F") + " ms");
				ImGui.TableNextColumn();
				ImGui.Text(msMinView.ToString("F") + " ms");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));

				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.Text((1000.0 / msMaxView).ToString("F") + " fps");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0.8f, 0, 0, 0.25f)));
				ImGui.TableNextColumn();
				ImGui.Text((1000.0 / ms).ToString("F") + " fps");
				ImGui.TableNextColumn();
				ImGui.Text((1000.0 / msMinView).ToString("F") + " fps");
				ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0f, 0.8f, 0, 0.25f)));

				ImGui.EndTable();

			}

			


			#endregion


			float preWidth = ImGui.GetColumnWidth();

			Vector2 histStartScreen;
			Vector2 histEndScreen;

			#region Plot

			histStartScreen = ImGui.GetCursorScreenPos();
			histStartScreen += (Vector2)ImGui.GetStyle().FramePadding;

			System.Numerics.Vector2 histCurStart = ImGui.GetCursorPos();
			int histWidth = (int)ImGui.GetCursorPosX();

			float[][] histVals = new float[profilerVals.Count][];
			for (int i = 0; i < profilerVals.Count; i ++) {

				int wrapAroundMisc = (i + 1) % profilerVals.Count;

				histVals[i] = SquishPlot(profilerVals.ElementAt(wrapAroundMisc).Value, numElements);
				if (i > 0) {
					for (int n = 0; n < numElements; n++) {
						histVals[i][n] += histVals[i - 1][n];
					}
				}
			}

			for (int i = histVals.Length - 1; i >= 0; i --) {

				Vector4 plotCol = GetColorForSample(i);
				if (i == histVals.Length - 1) {
					plotCol = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
				}
				ImGui.PushStyleColor(ImGuiCol.PlotHistogram, plotCol);
				if (i != 0) {
					ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0f, 0f, 0f, 0f));
				}
				else {
					ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0.2f, 0.2f, 0.2f, 138f / 255f));
				}
				ImGui.SetCursorPos(histCurStart);

				ImGui.PlotHistogram("", ref histVals[i][0], numElements, 0, "", 0f, plotMaxMs, new Vector2(-1f, 150f));

				ImGui.PopStyleColor();
				ImGui.PopStyleColor();

			}

			ImGui.SameLine();
			histWidth = (int)(ImGui.GetCursorPosX() - histWidth - ImGui.GetStyle().FramePadding.X * 2 - ImGui.GetStyle().ItemSpacing.X);
			
			numElements = Math.Max(1, histWidth);
			ImGui.NewLine();
			System.Numerics.Vector2 histCurEnd = ImGui.GetCursorPos();

			histEndScreen = histStartScreen + new Vector2(histWidth, 150 - ImGui.GetStyle().FramePadding.Y * 2);

			if (ImGui.GetMousePos().X > histStartScreen.x && ImGui.GetMousePos().X < histEndScreen.x && ImGui.GetMousePos().Y > histStartScreen.y && ImGui.GetMousePos().Y < histEndScreen.y) {
				plotMaxMs *= MathF.Exp(ImGui.GetIO().MouseWheel * -0.2f);
				plotMaxMs = MathF.Min(MathF.Max(0.01f, plotMaxMs), 80f);
			}

			#endregion
			#region Draw ms lines on plot

			float[] msLines = new float[] { 1f, 2f, 4.17f, 8.33f, 16.67f };
			for (int i = 0; i < msLines.Length; i ++) {

				float height = MathUtil.Map(msLines[i], 0f, plotMaxMs, histEndScreen.y, histStartScreen.y);
				
				if (msLines[i] > plotMaxMs) { break; }
				if (msLines[i] / plotMaxMs < 0.1) { continue; }

				string msReadout = msLines[i].ToString("F1") + " ms";
				string fpsReadout = MathF.Round(1000f / msLines[i]).ToString("F0") + " FPS";


				
				Vector2 msTextAlignment = new Vector2(-ImGui.CalcTextSize(msReadout).X, -ImGui.CalcTextSize(msReadout).Y / 2f);
				Vector2 fpsTextAlignment = new Vector2(0f, -ImGui.CalcTextSize(fpsReadout).Y / 2f);

				float msReadoutAlpha = 0.4f;

				ImGui.GetWindowDrawList().AddLine(new Vector2(histStartScreen.x + ImGui.CalcTextSize(fpsReadout).X + 5, height), new Vector2(histEndScreen.x - ImGui.CalcTextSize(msReadout).X - 5, height), ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, msReadoutAlpha)));
				ImGui.GetWindowDrawList().AddText(new Vector2(histEndScreen.x, height) + msTextAlignment, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, msReadoutAlpha)), msReadout);
				ImGui.GetWindowDrawList().AddText(new Vector2(histStartScreen.x, height) + fpsTextAlignment, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, msReadoutAlpha)), fpsReadout);

				

			}

			#endregion

			ImGui.Checkbox("Update plot", ref updatePlot);
			ImGui.SameLine();
			ImGui.Checkbox("More options", ref showMoreOptions);

			if (showMoreOptions == true) {

				ImGui.PushID("Performance breakdown pie");


				#region Pie chart

				Vector2 circleCenter = (Vector2)ImGui.GetCursorScreenPos() + new Vector2(100f, 100f);
				ImGui.BeginChild((uint)"Performance pie child".GetHashCode(), new System.Numerics.Vector2(200f, 200f));

				float circleRadius = 100f;
				float pieStart = 0f;
				for (int i = 0; i < profilerVals.Count(); i ++) {

					int wrapForMisc = (i + 1) % profilerVals.Count();
					float pieEnd = pieStart + (profilerVals.ElementAt(wrapForMisc).Value.Last() / lastFrameTime / 1000f);
					int segments = (int)((pieEnd - pieStart) * 64);
					segments = Math.Max(segments, 3);

					uint currentCol = ImGui.ColorConvertFloat4ToU32(GetColorForSample(wrapForMisc - 1));
					if (wrapForMisc == 0) {
						currentCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
					}

					for (int n = 0; n < segments; n ++) {
						float currentPieStart = MathUtil.Lerp((float)n / segments, pieStart, pieEnd);
						float currentPieEnd = MathUtil.Lerp(((float)n + 1.1f) / segments, pieStart, pieEnd);

						currentPieStart *= MathF.PI * 2f;
						currentPieEnd *= MathF.PI * 2f;

						ImGui.GetWindowDrawList().AddTriangleFilled(circleCenter - Vector2.Angle(currentPieStart - MathF.PI / 2f, 0f), circleCenter + Vector2.Angle(currentPieStart - MathF.PI / 2f, circleRadius), circleCenter + Vector2.Angle(currentPieEnd - MathF.PI / 2f, circleRadius), currentCol);
					}
					pieStart = pieEnd;

				}
				ImGui.GetWindowDrawList().AddCircle(circleCenter, circleRadius, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)));


				ImGui.EndChild();


				#endregion


				ImGui.SameLine();

				ImGuiTableFlags tableFlags = ImGuiTableFlags.Sortable | ImGuiTableFlags.SortMulti | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersV | ImGuiTableFlags.ScrollY | ImGuiTableFlags.NoBordersInBody;
				if (ImGui.BeginTable("Profiler metrics", 4, tableFlags) == true) {
					ImGui.TableSetupColumn("ID", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthFixed);
					ImGui.TableSetupColumn("Col", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.WidthFixed);
					ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.NoSort);
					ImGui.TableSetupColumn("Time", ImGuiTableColumnFlags.DefaultSort);
					ImGui.TableSetupScrollFreeze(0, 1);
					ImGui.TableHeadersRow();

					ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();

					for (int i = 0; i < profilerVals.Count; i++) {
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						ImGui.Text(i.ToString("00"));
						ImGui.TableNextColumn();

						Vector4 colButtonCol = GetColorForSample(i - 1);
						if (i == 0) {
							colButtonCol = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
						}

						ImGui.ColorButton(profilerVals.ElementAt(i).Key + " plot colour", colButtonCol, ImGuiColorEditFlags.NoTooltip, new Vector2(15f, 15f));
						ImGui.TableNextColumn();
						ImGui.Text(profilerVals.ElementAt(i).Key);
						ImGui.TableNextColumn();
						ImGui.Text(profilerVals.ElementAt(i).Value.Last().ToString("F3") + " ms");

					}

					ImGui.EndTable();
				}
				



				ImGui.PopID();

			}

		}

		private Vector4 GetColorForSample(int index) {
			
			
			float ratio = 0.182f;
			double hue = (index * ratio) % 1f;
			System.Drawing.Color col;
			col = MathUtil.HsvToRgb(360.0 * hue, 0.6, 0.6);
			return new Vector4(col.R / 255f, col.G / 255f, col.B / 255f, 1.0f);
		}

		private float[] SquishPlot(float[] inVals, int targetNumVals) {

			float[] outVals = new float[targetNumVals];
			for (int i = 0; i < outVals.Length; i++) {
				outVals[i] = 0f;
			}
			float averageCount = (float)inVals.Length / targetNumVals;
			int currentDestIndex = 0;
			float destRemaining = averageCount;
			for (int i = 0; i < inVals.Length; i++) {

				float srcRemaining = 1f;

				while (srcRemaining > 0f && currentDestIndex < targetNumVals) {
					if (destRemaining > 0f) {

						float amntTransfer = MathF.Min(srcRemaining, destRemaining);
						outVals[currentDestIndex] += inVals[i] * amntTransfer / averageCount;
						srcRemaining -= amntTransfer;
						destRemaining -= amntTransfer;
					}
					else {
						currentDestIndex += 1;
						destRemaining = averageCount;
					}
				}
			}
			return outVals;

		}


	}
}
