using System.Linq;
using System.Collections.Generic;
using iRacingSdkWrapper;


namespace ReplayTimeline
{
	public class SessionInfoHelper
	{
		public static int GetSessionID(SessionInfo sessionInfo)
		{
			YamlQuery weekendInfoQuery = sessionInfo["WeekendInfo"];
			return int.Parse(weekendInfoQuery["SubSessionID"].GetValue("-1"));
		}

		public static bool IsLiveSession(SessionInfo sessionInfo)
		{
			YamlQuery weekendInfoQuery = sessionInfo["WeekendInfo"];
			var simMode = weekendInfoQuery["SimMode"].GetValue();
			return simMode.ToLower().Contains("replay") ? false : true;
		}

		public static List<Driver> GetSessionDrivers(SessionInfo sessionInfo, ICollection<Driver> currentDrivers)
		{
			YamlQuery weekendOptionsQuery = sessionInfo["WeekendInfo"]["WeekendOptions"];
			int currentStarters = int.Parse(weekendOptionsQuery["NumStarters"].GetValue());

			var sessionDrivers = new List<Driver>();

			for (int i = 0; i < currentStarters; i++)
			{
				YamlQuery query = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", i];
				Driver newDriver;

				string driverName = query["UserName"].GetValue("");

				if (!string.IsNullOrEmpty(driverName) && driverName != "Pace Car")
				{
					// Get driver if driver is in previous list
					newDriver = currentDrivers.FirstOrDefault(d => d.Name == driverName);

					// If not...
					if (newDriver == null)
					{
						// Populate driver info
						newDriver = new Driver();
						newDriver.Id = i;
						newDriver.Name = driverName;
						newDriver.CustomerId = int.Parse(query["UserID"].GetValue("0")); // default value 0
						newDriver.Number = query["CarNumber"].GetValue("").TrimStart('\"').TrimEnd('\"'); // trim the quotes
						newDriver.NumberRaw = int.Parse(query["CarNumberRaw"].GetValue(""));
						newDriver.TeamName = query["TeamName"].GetValue("");

						//newDriver.Number = int.Parse(query["CarNumberRaw"].GetValue("").TrimStart('\"').TrimEnd('\"')); // trim the quotes
						newDriver.Rating = int.Parse(query["IRating"].GetValue("0"));
					}

					// Add to drivers list
					sessionDrivers.Add(newDriver);
				}
			}

			return sessionDrivers.OrderBy(d => d.NumberRaw).ToList();
		}

		public static List<Camera> GetSessionCameras(SessionInfo sessionInfo, ICollection<Camera> currentCameras)
		{
			int id = 1;
			Camera newCam;

			var sessionCameras = new List<Camera>();

			// Loop through cameras until none are found anymore
			do
			{
				newCam = null;
				YamlQuery query = sessionInfo["CameraInfo"]["Groups"]["GroupNum", id];

				// Get Camera Group name
				string groupName = query["GroupName"].GetValue("");

				if (!string.IsNullOrEmpty(groupName))
				{
					// Get Camera if it is in previous list
					newCam = currentCameras.FirstOrDefault(c => c.GroupName == groupName);

					// Create new camera if not in previous lsit
					if (newCam == null)
					{
						newCam = new Camera();
						newCam.GroupNum = id;
						newCam.GroupName = groupName;
					}
					else
					{
						// If it does exist, update the group number to ensure it matches
						newCam.GroupNum = id;
					}

					sessionCameras.Add(newCam);

					id++;
				}
			}
			while (newCam != null);

			return sessionCameras;
		}

		public static void UpdateDriverTelemetry(TelemetryInfo telemetryInfo, ICollection<Driver> driverList)
		{
			var laps = telemetryInfo.CarIdxLap.Value;
			var lapDistances = telemetryInfo.CarIdxLapDistPct.Value;

			// Loop through the list of current drivers
			foreach (Driver driver in driverList)
			{
				// Set the lap, distance belonging to this driver
				driver.Lap = laps[driver.Id];
				driver.LapDistance = lapDistances[driver.Id];
			}
		}
	}
}
