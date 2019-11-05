using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;


namespace com.instein98.game{
	public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable {

		#region IPunObservable implementation
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
			if (stream.IsWriting){
				// We own this player: send the others our data
				stream.SendNext(isFiring);
				stream.SendNext(Health);
			} else{
				// Network player, receieve data
				this.isFiring = (bool)stream.ReceiveNext();
				this.Health = (float)stream.ReceiveNext();
			}
		}

		#endregion

		#region Private Fields
			[SerializeField]
			private GameObject beams;
			bool isFiring = false;
		#endregion

		#region Public Fields
			public float Health = 1f;

		#endregion


		#region MonoBehaviour Callbacks

		void Awake() {
			if (beams == null){
				Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
			}else{
				beams.SetActive(false);
			}
		}

		void Start () {
			CameraWork _canmeraWork = GetComponent<CameraWork>();
			if (_canmeraWork != null){
				if(photonView.IsMine){
					_canmeraWork.OnStartFollowing();
				}
			}else{
				Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
			}
		}
		
		// Update is called once per frame
		void Update () {
			//  process input only when it is local player
			Debug.Log(photonView.IsMine);
			if (photonView.IsMine){
				if (Health <= 0){
					GameManager.Instance.LeaveRoom();
				}
				processInputs();
			}

			if (beams != null && isFiring != beams.activeSelf){
				beams.SetActive(isFiring);
			}
		}

		void OnTriggerEnter(Collider other) {
			if(!photonView.IsMine){
				return;
			}	
			if (!other.name.Contains("Beam")){
				return;
			}
			Health -= 0.1f;
		}

		void OnTriggerStay(Collider other) {
			if(!photonView.IsMine){
				return;
			}	
			if (!other.name.Contains("Beam")){
				return;
			}
			Health -= 0.1f*Time.deltaTime;
		}

		#endregion

		#region Custom
			void processInputs(){
				if (Input.GetButtonDown("Fire1")){
					isFiring = true;
				}
				if (Input.GetButtonUp("Fire1")){
					isFiring = false;
				}
			}

		#endregion
	}
}
