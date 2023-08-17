
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kawaiiju.Traffic
{
	public class NavSection : MonoBehaviour 
	{
		// -------------------------------------------------------------------
		// Properties

		[Header("Nav Section")]
		public VehicleSpawn[] vehicleSpawns;
		public NavConnection[] connections;
		public int speedLimit = 20;

		// -------------------------------------------------------------------
		// Initialization

		public virtual void Start()
		{
			foreach(NavConnection connection in connections)
				connection.navSection = this;
		}

		// -------------------------------------------------------------------
		// Get Data

		public bool TryGetVehicleSpawn(out VehicleSpawn spawn)
		{
			if(m_CurrentVehicles.Count == 0 && vehicleSpawns.Length > 0)
			{
				int index = UnityEngine.Random.Range(0, vehicleSpawns.Length);
				spawn = vehicleSpawns[index];
				return true;
			}
			spawn = null;
			return false;
		}

		// -------------------------------------------------------------------
		// Vehicle Management

		private List<Vehicle> m_CurrentVehicles = new List<Vehicle>();

		public void RegisterVehicle(Vehicle input, bool isAdd)
		{
			if(isAdd)
				m_CurrentVehicles.Add(input);
			else
			{
				if(m_CurrentVehicles.Contains(input))
					m_CurrentVehicles.Remove(input);
				else
					Debug.LogWarning("Traffic: Attempted to remove non-existing vehicle from Road: "+gameObject.name);
			}
		}

	}

	[Serializable]
	public class VehicleSpawn
	{
		public Transform spawn;
		public NavConnection destination;
	}
}
