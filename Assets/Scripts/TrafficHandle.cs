using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficHandle : MonoBehaviour
{
	public Phase[] phases;
	public float phaseInterval = 5;


	public void Start()
	{		
		if (phases.Length > 0)
			phases[0].Enable();
	}

	private void Update()
	{

			m_PhaseTimer += Time.deltaTime;
			if (!m_PhaseEnded && m_PhaseTimer > (phaseInterval * 0.5f))
				EndPhase();
			if (m_PhaseTimer > phaseInterval)
				ChangePhase();
		
	}



	float m_PhaseTimer;
	bool m_PhaseEnded;
	private int m_CurrentPhase;

	private void EndPhase()
	{
		m_PhaseEnded = true;
		phases[m_CurrentPhase].End();
	}

	public void ChangePhase()
	{
		m_PhaseTimer = 0;
		m_PhaseEnded = false;
		if (m_CurrentPhase < phases.Length - 1)
			m_CurrentPhase++;
		else
			m_CurrentPhase = 0;
		phases[m_CurrentPhase].Enable();
	}

	[Serializable]
	public class Phase
	{
		public GameObject[] positiveZones;
		public GameObject[] negativeZones;
		public GameObject[] positiveLights;
		public GameObject[] negativeLights;

		public void Enable()
		{
			foreach (GameObject positiveZone in positiveZones)
				positiveZone.gameObject.SetActive(true);
			foreach (GameObject positiveLight in positiveLights)
				positiveLight.gameObject.SetActive(true);
			foreach (GameObject negativeZone in negativeZones)
				negativeZone.gameObject.SetActive(false);
			foreach (GameObject negativeLight in negativeLights)
				negativeLight.gameObject.SetActive(false);
		}

		public void End()
		{
			foreach (GameObject positiveZone in positiveZones)
				positiveZone.gameObject.SetActive(true);
		}
	}
}

