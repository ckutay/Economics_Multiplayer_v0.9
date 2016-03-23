using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;
using UnityEngine.Networking;

using UnityEngine.UI;

public class ExperimentNetworking : NetworkBehaviour
{
	public bool urlReturn;
	string _message;
	public int resultCoins = -1;


	public CoinManager coinManager;

	//returns from url
	public string returnString;
	public int returnInt;
	public float returnFloat;


	[SyncVar] public string message = "";
	[SyncVar] public string resultMessage = "";

	void Start ()
	{
		//start at stage 0
		coinManager = GetComponent<CoinManager> ();
		urlReturn = true;
	}

	public void callUpdate(){
		

		if (message != _message) {
			//send update of result Message too for when it comes in
			//empy message not displayed
			coinManager.player.Cmd_broadcast (message, resultMessage);

		}
		_message = message;
	}
	public IEnumerator FetchStage (string _url, string find, string findInt, ExperimentController.runState _mode)
	{
		
			urlReturn = false;
			//Debug.LogWarning (url);

			yield return StartCoroutine (WaitForSeconds (.5f));
			WWW www = new WWW (_url);

			yield return StartCoroutine (WaitForRequest (www));
			//go to next step when done
			urlReturn = true;
			// StringBuilder sb = new StringBuilder();
			string result = www.text;
			JSONNode node = JSON.Parse (result);
		resultMessage = "";
			if (node != null) {
				try {
					//get stage message
					
					if (node ["type_stage"] == "End") {
						resultMessage = message;
					}else 
						message = node ["message"];
				} catch {
					//message = null;
					//yield return false;
				}

				//Debug.Log (message);

				if (find.Length != 0) {

					returnString = node [find];
					returnFloat = -1;
					//	Debug.LogWarning (node);
					if (find == "Results") {
						//hack to get results into message- the time delay
						//mens you cannot pick this up in the state machine


						if (float.TryParse (returnString, out returnFloat)) {
							//get back result from group
					
					
						if (!coinManager.result & returnFloat > 0 ) {
								//set to display result only
								resultCoins =	 (int)returnFloat;
							//display returned amount and no effort coins
								coinManager.result = true;
								coinManager.currentCoins -= (int)returnFloat;
							resultMessage+=(coinManager.maxCoins+1-coinManager.currentCoins).ToString();

							}

							yield return true;

							//message for localplayer/tokenbox only
						}

						yield return true;
					} else if (Int32.TryParse (node [findInt], out returnInt)) {

						//Debug.Log(returnInt);
						yield return true;
					}

					yield  return true;
				} else {

					if (Int32.TryParse (node [findInt], out returnInt))
						yield return true;

				}
			} else {
				//Debug.LogWarning ("No node on api read for " + find + " or " + findInt);
				//canvas.message = "Errer in stages for experiment: " + node;
				yield return true;

			}

		yield break;
	}

	IEnumerator setupWait (float num)
	{
		yield return WaitForSeconds (num);


	}

	public IEnumerator WaitForRequest (WWW www)
	{

		yield return www;

	}

	IEnumerator WaitForSeconds (float num)
	{

		yield return new WaitForSeconds (num);

	}

 
 } 