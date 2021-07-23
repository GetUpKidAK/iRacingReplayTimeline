﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace iRacingReplayDirector
{
	public class SaveLoadHelper
	{
		private const string m_ProjectsSubFolder = "Saves";
		private const string m_ProjectExtension = ".timeline";
		private const string m_SettingsFilename = "settings";
		private const string m_SettingsExtension = ".ini";
		private const Formatting m_FileFormatting = Formatting.Indented;


		public static void SaveSettings(ReplayDirectorVM vm)
		{
			AppSettings appSettings = new AppSettings()
			{
				ShowVisualTimeline = vm.ShowVisualTimeline,
				ShowSessionLapSkipButtons = vm.ShowSessionLapSkipButtons,
				ShowDriverCameraPanels = vm.ShowDriverCameraPanels,
				ShowTimelineNodeList = vm.ShowTimelineNodeList,
				DisableSimUIOnPlayback = vm.DisableSimUIOnPlayback,
				DisableUIWhenRecording = vm.DisableUIWhenRecording,
				StopRecordingOnFinalNode = vm.StopRecordingOnFinalNode,
				UseInSimCapture = vm.UseInSimCapture,
				UseOBSCapture = vm.UseOBSCapture
			};

			var saveFilePath = GenerateSettingsFilePath();
			SaveToFile(saveFilePath, appSettings);
		}

		public static AppSettings LoadSettings()
		{
			var saveFilePath = GenerateSettingsFilePath();
			AppSettings loadedSettings = new AppSettings();

			if (File.Exists(saveFilePath))
			{
				try
				{
					string fileContents = File.ReadAllText(saveFilePath);
					loadedSettings = JsonConvert.DeserializeObject<AppSettings>(fileContents);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			return loadedSettings;
		}

		public static void SaveProject(List<TimelineNode> nodes, int sessionID)
		{
			TimelineProject newProject = new TimelineProject();
			newProject.SessionID = sessionID;

			foreach (var node in nodes)
			{
				NodeSaveFile newSaveNode = new NodeSaveFile(node.Enabled, node.Frame, 1, node.Driver.NumberRaw, node.Camera.GroupName);

				newProject.Nodes.Add(newSaveNode);
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
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			return loadedProject;
		}

		private static string GenerateSettingsFilePath()
		{
			var finalFilename = $"{m_SettingsFilename}{m_SettingsExtension}";
			var finalPath = AppDomain.CurrentDomain.BaseDirectory;

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
