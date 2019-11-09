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
		bool isConnecting = false;  // whether the connect is by the user

	#endregion

	#region Public Fields
		[SerializeField]
		private GameObject controlPanel;
		
		[SerializeField]
		private GameObject progressLabel;
	#endregion

	#region MonoBehaviour CallBacks

		void Awake(){
			// Critical
			// Makes sure PhotonNetwork.LoadLevel() make all clients in the same room sync their level
			PhotonNetwork.AutomaticallySyncScene = true;
			// PhotonNetwork = 50000;
		}
		// Use this for initialization
		void Start () {
			// Connect();
			progressLabel.SetActive(false);
			controlPanel.SetActive(true);
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
			isConnecting = true;
			progressLabel.SetActive(true);
			controlPanel.SetActive(false);
			if (PhotonNetwork.IsConnected){
				// Debug.Log("is connected");
				PhotonNetwork.JoinRandomRoom();
			}else{
				// Debug.Log("is not connected");
				PhotonNetwork.GameVersion = gameVersion;
				// #if CHINA
				// 	Debug.Log("ConnectToRegion cn");
				// 	PhotonNetwork.ConnectToRegion("cn");
				// #else
					// Debug.Log("ConnectUsingSettings");
					PhotonNetwork.ConnectUsingSettings();
				// #endif
			}
		}

	#endregion

	#region MonoBehaviourPunCallbacks Callbacks

		public override void OnConnectedToMaster(){
			Debug.LogFormat("\nConnected to Master in {0}, Current Ping: {1}",PhotonNetwork.CloudRegion, PhotonNetwork.GetPing());
			if (isConnecting){
				PhotonNetwork.JoinRandomRoom();
			}
		}

		public override void OnDisconnected(DisconnectCause cause){
			progressLabel.SetActive(false);
			controlPanel.SetActive(true);
			Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
		}

		public override void OnJoinRandomFailed(short returnCode, string message){
			Debug.Log("OnJoinRandomFailed was called by PUN");
			PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayersPerRoom });
		}

		public override void OnJoinedRoom(){
			Debug.Log("OnJoinedRoom was called by PUN");
			if (PhotonNetwork.CurrentRoom.PlayerCount == 1){
				// Debug.Log("We load the 'Room for 1' ");
				PhotonNetwork.LoadLevel("PlayLab");
			}
		}

	#endregion
	}

}
