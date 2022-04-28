using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace iRacingReplayDirector
{
	public class SaveLoadHelper
	{
		private const string m_ProjectsSubFolder = "Saves";
		private const string m_ProjectExtension = ".timeline";

		private const Formatting m_FileFormatting = Formatting.Indented;


		public static void SaveSettings(ReplayDirectorVM vm)
		{
			Properties.Settings.Default.Save();
		}

		public static void LoadSettings(ReplayDirectorVM vm)
		{
			vm.WindowWidth = Properties.Settings.Default.WindowWidth;
			vm.WindowHeight = Properties.Settings.Default.WindowHeight;
			vm.WindowAlwaysOnTop = Properties.Settings.Default.WindowOnTop;

			vm.ShowVisualTimeline = Properties.Settings.Default.ShowVisualTimeline;
			vm.ShowRecordingControls = Properties.Settings.Default.ShowRecordingControls;
			vm.ShowSessionLapSkipControls = Properties.Settings.Default.ShowSessionLapSkipControls;
			vm.FrameSkipInfoShown = Properties.Settings.Default.FrameSkipInfoShown;

			vm.DisableSimUIOnPlayback = Properties.Settings.Default.DisableSimUIOnPlayback;
			vm.DisableUIWhenRecording = Properties.Settings.Default.DisableUIWhenRecording;
			vm.StopRecordingOnFinalNode = Properties.Settings.Default.StopRecordingOnFinalNode;
			vm.UseInSimCapture = Properties.Settings.Default.UseInSimCapture;
			vm.UseOBSCapture = Properties.Settings.Default.UseOBSCapture;
		}

		public static void SaveProject(List<Node> nodes, int sessionID)
		{
			TimelineProject newProject = new TimelineProject();
			newProject.SessionID = sessionID;

			foreach (var node in nodes)
			{
				if (node is CamChangeNode)
				{
					CamChangeNode camChangeNode = node as CamChangeNode;
					NodeSaveFile newSaveNode = new NodeSaveFile(camChangeNode.Enabled, camChangeNode.Frame, NodeType.CamChange, 1, camChangeNode.Driver.NumberRaw, camChangeNode.Camera.GroupName);

					newProject.Nodes.Add(newSaveNode);
				}
				else
				{
					FrameSkipNode frameSkipNode = node as FrameSkipNode;
					NodeSaveFile newSaveNode = new NodeSaveFile(frameSkipNode.Enabled, frameSkipNode.Frame, NodeType.FrameSkip);

					newProject.Nodes.Add(newSaveNode);
				}
			}

			var saveFilePath = GenerateProjectFilePath(sessionID);
			SaveToFile(saveFilePath, newProject);
		}

		public static TimelineProject LoadProject(int sessionID)
		{
			var saveFilePath = GenerateProjectFilePath(sessionID);
			TimelineProject loadedProject = new TimelineProject();

			if (File.Exists(saveFilePath))
			{
				try
				{
					string fileContents = File.ReadAllText(saveFilePath);
					loadedProject = JsonConvert.DeserializeObject<TimelineProject>(fileContents);

					if (loadedProject == null)
						loadedProject = new TimelineProject();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			return loadedProject;
		}

		private static string GenerateProjectFilePath(int sessionID)
		{
			var finalFilename = $"{sessionID}{m_ProjectExtension}";
			var finalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, m_ProjectsSubFolder);

			if (!Directory.Exists(finalPath))
			{
				try
				{
					Directory.CreateDirectory(finalPath);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			return Path.Combine(finalPath, finalFilename);
		}

		private static void SaveToFile<T>(string fullSavePath, T saveData)
		{
			string jsonOutput = JsonConvert.SerializeObject(saveData, m_FileFormatting);

			try { File.WriteAllText(fullSavePath, jsonOutput); }
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
