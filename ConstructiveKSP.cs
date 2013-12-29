using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ConstructiveKSP
{
	public class CKSPStage {
		public List<Part> parts;
		public float totalMass;
		public float fuelMass;
		public float thrust;
		public float minDeltaV;
		public float maxDeltaV;
	}

	[KSPAddon(KSPAddon.Startup.EditorVAB, false)]
	public class ConstructiveKSP : MonoBehaviour
	{
		private float g;
		private Part selectedPart;
		private bool shouldUpdateStages = false;
		private Rect windowPosition;
		private List<CKSPStage> stages;
		private GUIStyle rightAlignedText;

		public void Awake ()
		{
			this.enabled = true;
		}

		void Start()
		{
			Debug.Log ("ConstructiveKSP started up in VAB mode");
			RenderingManager.AddToPostDrawQueue (0, DrawGUI);
			this.windowPosition = new Rect (Screen.width - 230, (Screen.height / 2) - 100, 200, 400);
			rightAlignedText = new GUIStyle (HighLogic.Skin.label);
			rightAlignedText.alignment = TextAnchor.UpperLeft;
			if (EditorLogic.startPod != null) {
				shouldUpdateStages = true;
			}
		}

		void Update()
		{
			Part newlySelectedPart = EditorLogic.SelectedPart;
			if (newlySelectedPart != null && selectedPart != newlySelectedPart) {
				newlySelectedPart.OnEditorAttach -= PartAttached;
				newlySelectedPart.OnEditorDetach -= PartDetached;
				newlySelectedPart.OnEditorAttach += PartAttached;
				newlySelectedPart.OnEditorDetach += PartDetached;
				selectedPart = newlySelectedPart;
			}

			if (shouldUpdateStages) {
				GenerateStages ();
			}
		}

		void PartAttached() {
			shouldUpdateStages = true;
		}

		void PartDetached() {
			shouldUpdateStages = true;
		}

		void GenerateStages() {
			stages = new List<CKSPStage> ();
			assignChilrenOfPartToStages (EditorLogic.startPod);
			calculateStatsForStages (stages);
			shouldUpdateStages = false;
		}

		void assignChilrenOfPartToStages(Part parent) {
			if (stages.Count == parent.defaultInverseStage) {
				CKSPStage newStage = new CKSPStage ();
				newStage.parts = new List<Part> ();
				newStage.parts.Add (parent);
				stages.Add (newStage);
			} else {
				stages [parent.defaultInverseStage].parts.Add (parent);
			}
			foreach (Part child in parent.children) {
				assignChilrenOfPartToStages (child);
			}
		}

		void calculateStatsForStages(List<CKSPStage> stages) {
			float totalCraftMass = 0;
			foreach (CKSPStage stage in stages) {
				float fuelMass = 0;
				float totalMass = 0;
				float totalMinIsp = 0;
				float totalMaxIsp = 0;
				float totalThrust = 0;
				foreach (Part part in stage.parts) {
					totalMass += part.mass;
					foreach (PartModule module in part.Modules) {
						ModuleEngines engine = module as ModuleEngines;
						if (engine) {
							float engineThrust = engine.maxThrust * engine.thrustPercentage / 100;
							double normalizedLimitingFuel = double.MaxValue;
							foreach (Propellant fuel in engine.propellants) {
								fuel.UpdateConnectedResources (part);
								double normalizedFuel = fuel.totalResourceCapacity / fuel.ratio;
								if (normalizedFuel < normalizedLimitingFuel) {
									normalizedLimitingFuel = normalizedFuel;
								}
							}	

							float atmIsp = engine.atmosphereCurve.Evaluate (1);
							float vacIsp = engine.atmosphereCurve.Evaluate (0);

							g = engine.g;
							totalThrust += engineThrust;
							fuelMass += (float)normalizedLimitingFuel * engine.mixtureDensity;
							totalMass += fuelMass;
							totalMinIsp += engineThrust / atmIsp;
							totalMaxIsp += engineThrust / vacIsp;
						}
					}
				}
				float minAverageIsp = totalThrust / totalMinIsp;
				float maxAverageIsp = totalThrust / totalMaxIsp;
				totalCraftMass += totalMass;
				stage.totalMass = totalCraftMass;
				stage.fuelMass = fuelMass;
				stage.minDeltaV = minAverageIsp * (float)Math.Log (totalCraftMass / (totalCraftMass - fuelMass)) * g;
				stage.maxDeltaV = maxAverageIsp * (float)Math.Log (totalCraftMass / (totalCraftMass - fuelMass)) * g;
				stage.thrust = totalThrust;
			}
		}

		void DrawGUI () {
			GUI.skin = HighLogic.Skin;
			GUILayout.Window (-555645, this.windowPosition, CraftStatsGUI, "ConstructiveKSP", windowOptions());
		}

		GUILayoutOption[] windowOptions() {
			return new GUILayoutOption[] { GUILayout.Width (200), GUILayout.Height (400) };
		}

		void CraftStatsGUI (int windowId) {
			foreach (CKSPStage stage in stages) {
					GUILayout.BeginVertical ();
					AddLabel ("Thr: " + stage.thrust + " Mass: " + stage.totalMass + " Fuel: " + stage.fuelMass + " atmV:" + stage.minDeltaV + " vacV:" + stage.maxDeltaV);
					GUILayout.EndVertical ();
			}
		}

		void AddLabel(string text) {
			GUILayout.Label(text, rightAlignedText);
		}
	}
}

