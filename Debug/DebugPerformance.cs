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
		List<(string, float)> profilerUpdateQueue;

		bool writing = false;
		bool reading = false;
		List<(string, float)> profilerValsBuffer;

		int numElements;
		float msLine;
		float plotMaxMs = 10f;
		bool updatePlot = true;
		bool updatePlotActual = true;

		

		internal override string name => "Performance";

		internal DebugPerformance() {

			numElements = 2000;
			msLine = 8.3f;

			totalFrameTimes = new float[numElements];
			profilerVals = new Dictionary<string, float[]>();
			profilerVals.Add("Untracked", new float[numElements]);

			profilerUpdateQueue = new List<(string, float)>();

			profilerValsBuffer = new List<(string, float)>();

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



			if (reading == false) {
				writing = true;
				profilerValsBuffer = new List<(string, float)>();
				for (int i = 0; i < profilerVals.Count; i++) {
					profilerValsBuffer.Add((profilerVals.ElementAt(i).Key, profilerVals.ElementAt(i).Value.Last()));
				}
				writing = false;
			}
			

			

		}

		List<(string, float)> pVals;
		internal override void Render() {

			#region FPS Table

			double ms = totalFrameTimes.Last();

			ImGuiTableFlags flags = ImGuiTableFlags.Borders;
			ImGui.BeginTable("Performance Table", 3, flags);

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

			#endregion
			#region Plot Options

			ImGui.Columns(2);
			ImGui.SliderFloat("Max ms", ref plotMaxMs, 1f, 40f);
			ImGui.NextColumn();
			ImGui.Checkbox("Update Plot", ref updatePlot);
			ImGui.Columns();

			#endregion

			float preWidth = ImGui.GetColumnWidth();
			ImGui.Columns(2);
			ImGui.SetColumnWidth(0, preWidth - 120);

			#region Plot

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

				ImGui.PlotHistogram("", ref histVals[i][0], numElements, 0, "", 0f, plotMaxMs, new System.Numerics.Vector2(-1f, 150f));

				ImGui.PopStyleColor();
				ImGui.PopStyleColor();

			}

			ImGui.SameLine();
			histWidth = (int)(ImGui.GetCursorPosX() - histWidth - ImGui.GetStyle().FramePadding.X * 2 - ImGui.GetStyle().ItemSpacing.X);
			numElements = histWidth;
			ImGui.NewLine();
			System.Numerics.Vector2 histCurEnd = ImGui.GetCursorPos();

			#endregion
			#region MS line

			if (msLine <= plotMaxMs) {
				ImGui.PushStyleColor(ImGuiCol.FrameBg, new System.Numerics.Vector4(0f, 0f, 0f, 0f));
				float[] lines = new float[] { msLine, msLine };
				ImGui.SetCursorPos(histCurStart);
				ImGui.PlotLines("", ref lines[0], 2, 0, "", 0f, plotMaxMs, new System.Numerics.Vector2(-1f, 150f));
				ImGui.PopStyleColor();
			}

			#endregion
			#region MS buttons

			ImGui.NextColumn();

			float[] msCheckVals = new float[] {8.3f, 16.7f, 33.3f};
			for (int i = 0; i < msCheckVals.Length; i ++) {
				if (msCheckVals[i] > plotMaxMs) { continue; }
				float buttonHeight = MathUtil.Map(msCheckVals[i], 0f, plotMaxMs, histCurEnd.Y - 8, histCurStart.Y);
				ImGui.SetCursorPos(new System.Numerics.Vector2(ImGui.GetCursorPosX(), buttonHeight - 8));
				if (ImGui.Button(msCheckVals[i] + " ms | " + MathF.Round(1000f / msCheckVals[i]) + " FPS") == true) {
					msLine = msCheckVals[i];
				}
			}

			ImGui.Columns();

			#endregion


			if (ImGui.CollapsingHeader("Breakdown") == true) {

				ImGui.PushID("Performance breakdown pie");

				#region Sort time samples


				if (writing == false) {
					reading = true;

					pVals = new List<(string, float)>();
					pVals.AddRange(profilerValsBuffer);

					reading = false;
				}

				int pValsCount = pVals.Count;
				float currentFrameTime = 0f;
				for (int i = 0; i < pVals.Count; i ++) {
					currentFrameTime += pVals[i].Item2;
				}

				(string name, int index)[] sortedNames = new (string name, int index)[pValsCount];
				int sortenNamesLen = sortedNames.Length;
				float[] sortedValues = new float[pValsCount];
				for (int i = 0; i < pVals.Count; i ++) {
					sortedNames[i] = (pVals[i].Item1, i);
					sortedValues[i] = pVals[i].Item2;

					// Make sure Misc is at the end of the array for drawing
					if (pVals[i].Item1 == "Untracked") {
						sortedValues[i] = float.MinValue;
					}
				}
				//Array.Sort(sortedValues, sortedNames);
				//Array.Reverse(sortedNames);
				//Array.Reverse(sortedValues);

				// Restore misc value
				float miscVal = 0f;
				for (int i = 0; i < pVals.Count; i ++) {
					if (pVals[i].Item1 == "Untracked") {
						miscVal = pVals[i].Item2;
						break;
					}
				}
				sortedValues[sortedValues.Length - 1] = miscVal;

				#endregion

				ImGui.Columns(2);

				#region Pie chart

				Vector2 childStart = ImGui.GetCursorScreenPos();
				ImGui.BeginChild((uint)"Performance pie child".GetHashCode(), new System.Numerics.Vector2(200f, 200f));


				Vector2 circleCenter = childStart + new Vector2(100f, 100f);
				float circleRadius = 100f;

				float pieStart = 0f;
				for (int i = 0; i < profilerVals.Count(); i ++) {

					int wrapForMisc = (i + 1) % profilerVals.Count();

					uint currentCol = ImGui.ColorConvertFloat4ToU32(GetColorForSample(wrapForMisc - 1));
					if (wrapForMisc == 0) {
						currentCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.8f, 0.8f, 0.8f, 1.0f));
					}
					float pieEnd = pieStart + (profilerVals.ElementAt(wrapForMisc).Value.Last() / currentFrameTime);


					int segments = (int)((pieEnd - pieStart) * 64);
					segments = Math.Max(segments, 3);
					for (int n = 0; n < segments; n ++) {
						float currentPieStart = MathUtil.Lerp((float)n / segments, pieStart, pieEnd);
						float currentPieEnd = MathUtil.Lerp(((float)n + 1.1f) / segments, pieStart, pieEnd);

						currentPieStart *= MathF.PI * 2f;
						currentPieEnd *= MathF.PI * 2f;


						ImGui.GetForegroundDrawList().AddTriangleFilled(circleCenter - Vector2.Angle(currentPieStart - MathF.PI / 2f, 0f), circleCenter + Vector2.Angle(currentPieStart - MathF.PI / 2f, circleRadius), circleCenter + Vector2.Angle(currentPieEnd - MathF.PI / 2f, circleRadius), currentCol);
					}
					pieStart = pieEnd;

				}
				

				ImGui.GetForegroundDrawList().AddCircle(circleCenter, circleRadius, ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f)));

				ImGui.EndChild();
				ImGui.NextColumn();

				#endregion

				for (int i = 0; i < profilerVals.Count(); i ++) {

					int wrapForMisc = (i + 1) % profilerVals.Count();

					Vector4 col = GetColorForSample(wrapForMisc - 1);
					if (wrapForMisc == 0) {
						col = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
					}

					ImGui.PushStyleColor(ImGuiCol.Text, col);
					ImGui.Text(profilerVals.ElementAt(wrapForMisc).Key + " - " + profilerVals.ElementAt(wrapForMisc).Value.Last() + " ms");
					ImGui.PopStyleColor();
				}
				ImGui.Columns();

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
