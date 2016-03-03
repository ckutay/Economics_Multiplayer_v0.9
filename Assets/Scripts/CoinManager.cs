﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

using System;

public class CoinManager : NetworkBehaviour
{

	public GameObject[] effort;
	public GameObject[] resource;
	bool isPressed = false;

	//should be updated onj server and client;
	[SyncVar (hook = "updateCoins")]public int currentCoins;
	public Material clear;
	public bool _isLocalPlayer = false;
	public int maxCoins = 19;
	[HideInInspector][SyncVar]public bool result;
	public bool isFinished=false;
	public GameObject player;

	// Use this for initialization
	void Start ()
	{
		//Debug.Log ("Start");
		for (int i = maxCoins; i >= 0; i--) {
			effort [i].SetActive (false);
			resource [i].SetActive (true);
		}
		currentCoins = 0;

	}
	
	// Update is called once per frame
	void Update ()
	{

		if (_isLocalPlayer){
		//Debug.Log (currentCoins);
		HandlePlayerInput ();

		//woudl like to do once

	
		}
	}

	void HandlePlayerInput ()
	{
		//increment and decrement coin number on local box
		//set by callback in Playernetworkcontroller

		//Debug.Log ("Is Local");
		if (Input.GetKeyUp (KeyCode.W)) {
				
			if (currentCoins >= 0 && currentCoins < maxCoins) {
					

				currentCoins++;
				//Debug.Log (currentCoins);
			}

		}

		if (Input.GetKeyUp (KeyCode.S) && currentCoins > 0) {
			currentCoins--;

			

		}
		if (Input.GetAxis ("D-PadX") == 1.0f | Input.GetAxis ("D-PadX") == -1.0f) {
			//how to say finished
			isFinished = true;
		}
		if (Input.GetAxis ("D-PadY") > 0.0f && isPressed == false) {
				

			if (currentCoins <= maxCoins) {
				currentCoins++;
				//Debug.Log (currentCoins);
			}

			isPressed = true;
		}

		if (Input.GetAxis ("D-PadY") < 0.0f && isPressed == false && currentCoins > 1) {
			currentCoins--;

			
			isPressed = true;

		}

		if (Input.GetAxis ("D-PadY") == 0.0f) {
			isPressed = false;
			
		}
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetAxis ("A") == -1f) {
				

				isFinished = true;
			}
			//send to server

			
		if(_isLocalPlayer)Cmd_updateCoins(currentCoins);

		

	}

	void FixedUpdate ()
	{
		if (currentCoins>=0){
		for (int i = maxCoins; i >= currentCoins; i--) {
			
			effort [i].SetActive (false);
			resource [i].SetActive (true);
		}

		for (int i = 0; i < currentCoins; i++) {
			if (result) {
				effort [i].SetActive (false);
			} else
				effort [i].SetActive (true);
			
			resource [i].SetActive (false);
		}
		}
	}
	//when returns from server

		void updateCoins(int _currentCoins){
		currentCoins=_currentCoins;
		}
	[Command]

	void Cmd_updateCoins(int _currentCoins){
		currentCoins=_currentCoins;

	}

	[ClientRpc] 
	void Rpc_ChangeCoins (int arrayValue, bool _isAdd)
	{
		try {
			if (result) {
				effort [arrayValue].SetActive (false);
			} else
				effort [arrayValue].SetActive (_isAdd);
			resource [arrayValue].SetActive (!_isAdd);
		} catch (Exception e){
			Debug.LogWarning(e);
		}
		Debug.Log ("Rpc called");

	}



	[Command] 
	void Cmd_Change_currentCoins (int _currentCoins)
	{
		
		currentCoins = _currentCoins;
	
		Debug.Log ("Cmd called");


	}



	public void SetToClear ()
	{
		if (GetComponent<MeshRenderer> ().material != clear)
			GetComponent<MeshRenderer> ().material = clear;
	}

	void OnGUI ()
	{
		//GUILayout.Label(currentCoins.ToString());
	}


}
