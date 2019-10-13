using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace com.instein98.game{
	public class Launcher : MonoBehaviourPunCallbacks {

	#region Private Serializable Fields

	[Tooltip("The maximum number of players per room.")]
	[SerializeField]
	private byte maxPlayersPerRoom = 4;

	#endregion
	
	#region Private Fields

		/// <summary>
		/// This client's version number.
		/// </summary>
		string gameVersion = "1";

	#endregion

	#region MonoBehaviour CallBacks

		void Awake(){
			// Critical
			// Makes sure PhotonNetwork.LoadLevel() make all clients in the same room sync their level
			PhotonNetwork.AutomaticallySyncScene = true;
		}
		// Use this for initialization
		void Start () {
			Connect();
		}
		
		// Update is called once per frame
		void Update () {
			
		}

	#endregion

	#region Public Methods

		/// <summary>
		/// Start the connection process.
		/// - If already connected, we attempt joining a random room
		/// - if not yet connected, Connect this application instance to Photon Cloud Network
		/// </summary>
		public void Connect(){
			if (PhotonNetwork.IsConnected){
				PhotonNetwork.JoinRandomRoom();
			}else{
				PhotonNetwork.GameVersion = gameVersion;
				PhotonNetwork.ConnectUsingSettings();
			}
		}

	#endregion

	#region MonoBehaviourPunCallbacks Callbacks

		public override void OnConnectedToMaster(){
			Debug.Log("OnConnectedToMaster() was called by PUN");
			PhotonNetwork.JoinRandomRoom();
		}

		public override void OnDisconnected(DisconnectCause cause){
			Debug.Log("OnDisconnected was called by PUN");
		}

		public override void OnJoinRandomFailed(short returnCode, string message){
			Debug.Log("OnJoinRandomFailed was called by PUN");
			PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayersPerRoom });
		}

		public override void OnJoinedRoom(){
			Debug.Log("OnJoinedRoom was called by PUN");
		}

	#endregion
	}

}
