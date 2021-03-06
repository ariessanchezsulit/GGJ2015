﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;

public class BallSpawner : MonoBehaviour 
{
	public static readonly float EASY_SPAWN 	= 100f;
	public static readonly float MEDIUM_SPAWN	= 15f;
	public static readonly float HARD_SPAWN 	= 10f;

	public int m_minThrown = 1;
	public int m_maxThrown = 3;

	public float m_minSpawnInterval = 10f;
	public float m_maxSpawnInterval = 20f;

	// spawner position
	public static readonly Vector2 SPAWN_POSITION = new Vector2(2.0f, 0.0f);

	// spawns for 1.137217, -0.6044912
	public static readonly Vector2[] SPAWNS = new Vector2[5]
	{
		new Vector2(-100f, 200f),
		new Vector2(-120f, 230f),
		new Vector2(-70f, 240f),
		new Vector2(-110f, 200f),
		new Vector2(-110f, 240f),
	};
	
	/*
	public static readonly Vector2 SPAWN_1 = new Vector2(-100f, 200f);
	public static readonly Vector2 SPAWN_2 = new Vector2(-120f, 230f);
	public static readonly Vector2 SPAWN_3 = new Vector2(-70f, 240f);
	public static readonly Vector2 SPAWN_4 = new Vector2(-110f, 200f);
	public static readonly Vector2 SPAWN_5 = new Vector2(-110f, 240f);
	*/

	[SerializeField]
	private CannonBall m_ballTemplate;
	[SerializeField]
	private Vector2 m_throwForce;
	[SerializeField]
	private float m_targetInterval = EASY_SPAWN;
	private List<CannonBall> m_balls;
	private float m_interval;
	private TapType[] HazardTypes = {TapType.Hammer, TapType.Hammer, TapType.Hammer, TapType.Stitch, TapType.Stitch, TapType.Stitch,TapType.Stitch, TapType.Pail, TapType.Pail};

	private void Start ()
	{
		this.Assert<CannonBall>(m_ballTemplate, "ERROR: m_ballTemplate must not be null.");

		// adjust the position of the spawner
		this.transform.position = SPAWN_POSITION;

		// initializations
		m_balls = new List<CannonBall>();
		this.RandomizeSpawns();
	}
	
	private void Update ()
	{
		m_interval += Time.deltaTime;

		if (m_interval >= m_targetInterval) 
		{
			// randomize spawn pos
			this.RandomizeSpawns();

			m_interval = 0f;
			int num = UnityEngine.Random.Range(m_minThrown, m_maxThrown);
			this.StartCoroutine(this.Throw(num));
		}

	}

	public IEnumerator Throw (int p_num)
	{
		float subInterval = UnityEngine.Random.Range(m_minSpawnInterval, m_maxSpawnInterval);
		//float subInterval = this.Random<float>(MIN_SPAWN_INTERVAL, MAX_SPAWN_INTERVAL);

		for (int i = 0; i < p_num; i++)
		{
			this.Spawn();
			yield return new WaitForSeconds(subInterval);
			this.RandomizeSpawns();
		}
	}

	private void Spawn ()
	{
		if(!UIManager.Instance.Paused && (GameLogic.Instance.GameHasStarted && GameLogic.Instance.InGame))
		{
			GameObject obj = (GameObject)GameObject.Instantiate(m_ballTemplate.gameObject);
			CannonBall ball = obj.GetComponent<CannonBall>();
			ball.OnDestroyBall += this.OnDestroyBall;
			ball.Throw(m_throwForce);
			ball.transform.parent = this.transform;
			ball.transform.position = this.transform.position;
			
			// setup type here
			ball.TapType = HazardTypes[UnityEngine.Random.Range(0, HazardTypes.Length)];//TapType.Hammer;
			
			m_balls.Add(ball);
		}
	}

	private void RandomizeSpawns ()
	{
		m_throwForce = SPAWNS[UnityEngine.Random.Range(0, SPAWNS.Length)];
		//m_throwForce = SPAWNS[this.Random<int>(0, SPAWNS.Length)];
	}

	private void OnDestroyBall (CannonBall p_ball)
	{
		m_balls.Remove(p_ball);
		GameObject.Destroy(p_ball.gameObject);
	}

	/*
	private T Random<T> (T p_min, T p_max)
	{
		//return (T)UnityEngine.Random.Range(p_min, p_max);
		return null;
	}
	*/
}
