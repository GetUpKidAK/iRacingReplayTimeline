using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace ReplayTimeline
{
	public class SaveLoadHelper
	{
		private const string m_ProjectExtension = "_timeline.timeline";
		private const Formatting m_FileFormatting = Formatting.Indented;


		public static void SaveProject(List<TimelineNode> nodes, int sessionID)
		{
			TimelineProject newProject = new TimelineProject();

			newProject.SessionID = sessionID;
			newProject.TimelineNodes = nodes;

			var finalFilename = $"{sessionID}{m_ProjectExtension}";

			string saveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, finalFilename);

			SaveToFile(saveFile, newProject);
		}

		public static TimelineProject LoadProject(int sessionID)
		{
			var filename = $"{sessionID}{m_ProjectExtension}";

			string saveFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

			TimelineProject loadedProject = new TimelineProject();

			if (File.Exists(saveFile))
			{
				try
				{
					string fileContents = File.ReadAllText(saveFile);
					loadedProject = JsonConvert.DeserializeObject<TimelineProject>(fileContents);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}

			return loadedProject;
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
