using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

		public static List<Driver> GetSessionDrivers(SessionInfo sessionInfo, ICollection<Driver> currentDrivers)
		{
			YamlQuery weekendOptionsQuery = sessionInfo["WeekendInfo"]["WeekendOptions"];
			//var starters = weekendOptionsQuery["NumStarters"].GetValue();
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
						newDriver.Rating = int.Parse(query["IRating"].GetValue("0"));
					}

					// Add to drivers list
					sessionDrivers.Add(newDriver);
				}
			}

			return sessionDrivers;
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

					// Otherwise...
					if (newCam == null)
					{
						// If group name is found, create a new camera
						newCam = new Camera();
						newCam.GroupNum = id;
						newCam.GroupName = groupName;
					}
					sessionCameras.Add(newCam);

					id++;
				}
			}
			while (newCam != null);

			return sessionCameras;
		}

		//private static void ParseDrivers(SessionInfo sessionInfo)
		//{
		//	// Number of starters (Live only)
		//	YamlQuery weekendOptionsQuery = sessionInfo["WeekendInfo"]["WeekendOptions"];
		//	var starters = weekendOptionsQuery["NumStarters"].GetValue();
		//	int currentStarters = int.Parse(weekendOptionsQuery["NumStarters"].GetValue());

		//	var newDrivers = new List<Driver>();

		//	for (int i = 0; i < currentStarters; i++)
		//	{
		//		YamlQuery query = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", i];
		//		Driver newDriver;

		//		string driverName = query["UserName"].GetValue("");

		//		if (!string.IsNullOrEmpty(driverName) && driverName != "Pace Car")
		//		{
		//			// Get driver if driver is in previous list
		//			newDriver = Drivers.FirstOrDefault(d => d.Name == driverName);

		//			// If not...
		//			if (newDriver == null)
		//			{
		//				// Populate driver info
		//				newDriver = new Driver();
		//				newDriver.Id = i;
		//				newDriver.Name = driverName;
		//				newDriver.CustomerId = int.Parse(query["UserID"].GetValue("0")); // default value 0
		//				newDriver.Number = query["CarNumber"].GetValue("").TrimStart('\"').TrimEnd('\"'); // trim the quotes
		//				newDriver.Rating = int.Parse(query["IRating"].GetValue("0"));
		//			}

		//			// Add to drivers list
		//			newDrivers.Add(newDriver);
		//		}
		//	}

		//	// Cache current driver, needed for live sessions
		//	//var cachedCurrentDriver = CurrentDriver;
		//	// Replace old list of drivers with new list of drivers and update the grid
		//	Drivers.Clear();
		//	foreach (var newDriver in newDrivers)
		//	{
		//		Drivers.Add(newDriver);
		//	}
		//	// Replace previous driver once list is rebuilt.
		//	//CurrentDriver = cachedCurrentDriver;
		//}

		//private static void ParseCameras(SessionInfo sessionInfo)
		//{
		//	int id = 1;
		//	Camera newCam;

		//	var newCameras = new List<Camera>();

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
		//			newCam = Cameras.FirstOrDefault(c => c.GroupName == groupName);

		//			// Otherwise...
		//			if (newCam == null)
		//			{
		//				// If group name is found, create a new camera
		//				newCam = new Camera();
		//				newCam.GroupNum = id;
		//				newCam.GroupName = groupName;
		//			}
		//			newCameras.Add(newCam);

		//			id++;
		//		}
		//	}
		//	while (newCam != null);

		//	// Cache current camera, needed for live sessions
		//	//var cachedCurrentCamera = CurrentCamera;

		//	// Replace old list of drivers with new list of drivers and update the grid
		//	Cameras.Clear();
		//	foreach (var newCamera in newCameras)
		//	{
		//		Cameras.Add(newCamera);
		//	}

		//	// Replace previous camera once list is rebuilt.
		//	//CurrentCamera = cachedCurrentCamera;
		//}
	}
}
