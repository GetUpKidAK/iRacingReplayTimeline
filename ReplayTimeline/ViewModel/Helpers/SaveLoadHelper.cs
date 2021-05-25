using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;


namespace iRacingReplayDirector
{
	public class SaveLoadHelper
	{
		private const string m_SubFolder = "Saves";
		private const string m_ProjectExtension = ".timeline";
		private const Formatting m_FileFormatting = Formatting.Indented;


		public static void SaveProject(List<TimelineNode> nodes, int sessionID)
		{
			TimelineProject newProject = new TimelineProject();

			newProject.SessionID = sessionID;

			foreach (var node in nodes)
			{
				NodeSaveFile newSaveNode = new NodeSaveFile(node.Enabled, node.Frame, node.Driver.NumberRaw, node.Camera.GroupName);

				newProject.Nodes.Add(newSaveNode);
			}

			var saveFilePath = GenerateFilePath(sessionID);

			SaveToFile(saveFilePath, newProject);
		}

		public static TimelineProject LoadProject(int sessionID)
		{
			var saveFilePath = GenerateFilePath(sessionID);

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

		private static string GenerateFilePath(int sessionID)
		{
			var finalFilename = $"{sessionID}{m_ProjectExtension}";
			var finalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, m_SubFolder);
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
