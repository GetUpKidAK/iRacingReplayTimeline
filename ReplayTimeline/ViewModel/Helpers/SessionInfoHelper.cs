using System.Linq;
using System.Collections.Generic;
using iRacingSdkWrapper;


namespace iRacingReplayDirector
{
	public class SessionInfoHelper
	{
		//public static int GetSessionID(SessionInfo sessionInfo)
		//{
		//	YamlQuery weekendInfoQuery = sessionInfo["WeekendInfo"];
		//	return int.Parse(weekendInfoQuery["SubSessionID"].GetValue("-1"));
		//}

		//public static string GetCurrentSessionType(SessionInfo sessionInfo, int currentSession)
		//{
		//	YamlQuery query = sessionInfo["SessionInfo"]["Sessions"]["SessionNum", currentSession];

		//	return query["SessionType"].GetValue("");
		//}

		//public static string GetSessionLapCount(SessionInfo sessionInfo, int currentSession)
		//{
		//	YamlQuery query = sessionInfo["SessionInfo"]["Sessions"]["SessionNum", currentSession];

		//	return query["SessionLaps"].GetValue("-1");
		//}

		//public static bool IsLiveSession(SessionInfo sessionInfo)
		//{
		//	YamlQuery weekendInfoQuery = sessionInfo["WeekendInfo"];
		//	var simMode = weekendInfoQuery["SimMode"].GetValue();
		//	return simMode.ToLower().Contains("replay") ? false : true;
		//}

		//public static List<Driver> GetSessionDrivers(SessionInfo sessionInfo, ICollection<Driver> currentDrivers)
		//{
		//	YamlQuery weekendOptionsQuery = sessionInfo["WeekendInfo"]["WeekendOptions"];
		//	int currentStarters = int.Parse(weekendOptionsQuery["NumStarters"].GetValue());

		//	var sessionDrivers = new List<Driver>();

		//	for (int i = 0; i < currentStarters; i++)
		//	{
		//		YamlQuery query = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", i];
		//		Driver newDriver;

		//		string driverName = query["UserName"].GetValue("");

		//		if (!string.IsNullOrEmpty(driverName))
		//		{
		//			// Get driver if driver is in previous list
		//			newDriver = currentDrivers.FirstOrDefault(d => d.Name == driverName);

		//			// If not...
		//			if (newDriver == null)
		//			{
		//				// Populate driver info
		//				newDriver = new Driver()
		//				{
		//					Id = i,
		//					Name = driverName,
		//					CustomerId = int.Parse(query["UserID"].GetValue("0")), // default value 0
		//					Number = query["CarNumber"].GetValue("").TrimStart('\"').TrimEnd('\"'), // trim the quotes
		//					NumberRaw = int.Parse(query["CarNumberRaw"].GetValue("")),
		//					TeamName = query["TeamName"].GetValue("")
		//				};
		//			}

		//			// Add to drivers list
		//			sessionDrivers.Add(newDriver);
		//		}
		//	}

		//	return sessionDrivers.OrderBy(d => d.NumberRaw).ToList();
		//}

		//public static List<Camera> GetSessionCameras(SessionInfo sessionInfo, ICollection<Camera> currentCameras)
		//{
		//	int id = 1;
		//	Camera newCam;

		//	var sessionCameras = new List<Camera>();

		//	// Loop through cameras until none are found anymore
		//	do
		//	{
		//		newCam = null;
		//		YamlQuery query = sessionInfo["CameraInfo"]["Groups"]["GroupNum", id];

		//		// Get Camera Group name
		//		string groupName = query["GroupName"].GetValue("");

		//		if (!string.IsNullOrEmpty(groupName))
		//		{
		//			// Get Camera if it is in previous list
		//			newCam = currentCameras.FirstOrDefault(c => c.GroupName == groupName);

		//			// Create new camera if not in previous lsit
		//			if (newCam == null)
		//			{
		//				newCam = new Camera();
		//				newCam.GroupNum = id;
		//				newCam.GroupName = groupName;
		//			}
		//			else
		//			{
		//				// If it does exist, update the group number to ensure it matches
		//				newCam.GroupNum = id;
		//			}

		//			sessionCameras.Add(newCam);

		//			id++;
		//		}
		//	}
		//	while (newCam != null);

		//	return sessionCameras;
		//}

		//public static void UpdateDriverTelemetry(TelemetryInfo telemetryInfo, ICollection<Driver> driverList)
		//{
		//	var laps = telemetryInfo.CarIdxLap.Value;
		//	var lapDistances = telemetryInfo.CarIdxLapDistPct.Value;
		//	var trackSurfaces = telemetryInfo.CarIdxTrackSurface.Value;

		//	// Loop through the list of current drivers
		//	foreach (Driver driver in driverList)
		//	{
		//		// Set the details belonging to this driver
		//		driver.Lap = laps[driver.Id];
		//		driver.LapDistance = lapDistances[driver.Id];
		//		driver.TrackSurface = (TrackSurfaces)trackSurfaces[driver.Id];
		//	}

		//	var orderedDriverList = driverList.OrderByDescending(d => d.Lap).ThenByDescending(d => d.LapDistance).ToList();
		//	for (int i = 0; i < orderedDriverList.Count; i++)
		//	{
		//		orderedDriverList[i].Position = (i + 1);

		//		if (orderedDriverList[i].NumberRaw == 0) // Pace Car always placed in last place
		//		{
		//			orderedDriverList[i].Position = 999;
		//		}
		//	}
			
		//}
	}
}
