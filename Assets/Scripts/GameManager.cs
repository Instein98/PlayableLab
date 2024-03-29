﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;


namespace com.instein98.game{
	public class GameManager : MonoBehaviourPunCallbacks {
		[Tooltip("The prefab to use for representing the player")]
		public GameObject playerPrefab;

		public static GameManager Instance;
		#region Photon Callbacks
			public override void OnLeftRoom(){
				SceneManager.LoadScene(0);
			}

			public override void OnPlayerEnteredRoom(Player other){
				Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);
				if (PhotonNetwork.IsMasterClient){
					Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
					// LoadArena();
				}
			}

			public override void OnPlayerLeftRoom(Player other){
				Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
				if (PhotonNetwork.IsMasterClient){
					Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
					// LoadArena();
				}
			}
		#endregion

		#region Public Methods
			public void LeaveRoom(){
				PhotonNetwork.LeaveRoom();
			}
		#endregion

		#region Private Methods
			void LoadArena(){
				if (!PhotonNetwork.IsMasterClient){
					Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
				}
				Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
				PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
			}
		#endregion


	void Start () {	
		Instance = this;
		if (playerPrefab == null){
			Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
		}else{
			Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene());
			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
		}
	}

}
}

