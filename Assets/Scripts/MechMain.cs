﻿using UnityEngine;
using System.Collections;

public class MechMain : Photon.MonoBehaviour {


	[SerializeField]
	private GameObject enemyMech;

	//PAUSE MENU
	public Transform selector;
	public Transform pauseMenuTitle;
	public Transform[] menuOptions;
	public int selected;
	public bool pauseMenu = false;
	public bool menuCanMove = true;
	// END PAUSE MENU CODE

	[SerializeField]
	private float healthMax;
	[SerializeField]
	private float energyMax;
	[SerializeField]
	private float healthRegen;
	[SerializeField]
	private float energyRegen;
	[SerializeField]
	private float hoverCost = 1.5f;
	[SerializeField]
	private float dashCost = 1.0f;
	[SerializeField]
	private float health;
	[SerializeField]
	//[RPC]
	public float Health {
		get {
			return health;
		}
		set {
			health = value;
		}
	}
	
	private float energy;
	public float Energy {
		get {
			return energy;
		}
		set {
			energy = value;
		}
	}
	[SerializeField]
	private float enWait = 1.5f;
	public float EnWait {
				get {
						return enWait;
				}
	}
	private float enCount;
	[SerializeField]
	private float moveForce;
	[SerializeField]
	private float dashForce;
	[SerializeField]
	private float defaultForce;
	[SerializeField]
	private float hoverForce;
	[SerializeField]
	private float rotationSpeed;
	[SerializeField]
	private float jumpForce;
	
	[SerializeField]
	private string Vertical;
	[SerializeField]
	private string Horizontal;
	[SerializeField]
	private string dash;
	[SerializeField]
	private string locker;
	[SerializeField]
	private string jump;
	[SerializeField]
	private string FireLeft;
	[SerializeField]
	private string FireRight;
	[SerializeField]
	private string startButton;

	private int lives;

	private bool lockOn;
	private bool dashOn;
	private bool overHeat;
	private bool jumping;
	public bool Jumping {
		get {
			return jumping;
		}
	}
	public float jumpTime = 0.5f;
	public float jumpCount = 0.0f;

	private bool leftFiring;
	private bool rightFiring;
	private bool isControllable;

	[SerializeField]
	private Transform leftWeaponAttachPoint;
	[SerializeField]
	private Transform rightWeaponAttachPoint;

	[SerializeField]
	private Weapon leftWeaponPrefab;
	[SerializeField]
	private Weapon rightWeaponPrefab;

	[SerializeField]
	private string leftWeaponPath;
	[SerializeField]
	private string rightWeaponPath;
	
	[SerializeField]
	private string playerPrefLeftKey = "";
	[SerializeField]
	private string playerPrefRightKey = "";

	private Weapon leftWeapon;
	private Weapon rightWeapon;

	public bool Controllable {
		get{
			return isControllable;
		}
		set{
			isControllable = value;
		}
	}

	private bool isDead;
	private PoolingSystem poolingSystemL;
	private PoolingSystem poolingSystemR;
	public PoolingSystem poolL{
		get{
			return poolingSystemL;
		}
	}
	public PoolingSystem poolR{
		get{
			return poolingSystemR;
		}
	}

	// Use this for initialization
	void Start () {

		lives = GameObject.FindObjectOfType<NetworkLogic>().lives +1;
		health = 100;
		energy = 1;

		lockOn = false;
		dashOn = false;	
		overHeat = false;
		isControllable = true;
		health = healthMax;
		energy = energyMax;

		defaultForce = moveForce;
		enCount = enWait;

		if (PlayerPrefs.HasKey (playerPrefLeftKey)) {
			leftWeaponPath = PlayerPrefs.GetString(playerPrefLeftKey);
		}
		if (PlayerPrefs.HasKey (playerPrefRightKey)) {
			rightWeaponPath = PlayerPrefs.GetString(playerPrefRightKey);
		}
		Debug.Log ("Weapons/" + leftWeaponPath);
		leftWeapon = ((GameObject)PhotonNetwork.Instantiate ("Weapons/" + leftWeaponPath,transform.position, transform.rotation,0)).GetComponentInChildren<Weapon> ();
		rightWeapon = ((GameObject)PhotonNetwork.Instantiate ("Weapons/" + rightWeaponPath,transform.position, transform.rotation,0)).GetComponentInChildren<Weapon> ();
		//leftWeapon = (Weapon) Instantiate (leftWeaponPrefab);
		//rightWeapon = (Weapon) Instantiate (rightWeaponPrefab);

		leftWeapon.Parent = this;
		rightWeapon.Parent = this;

		Attach (leftWeapon, leftWeaponAttachPoint,-1);
		leftWeapon.GetComponent<Weapon> ().pool = poolingSystemL;
		Attach (rightWeapon, rightWeaponAttachPoint,1);

		//gameObject.GetComponent<PhotonView> ().RPC ("Attach", PhotonTargets.AllBuffered, leftWeapon, leftWeaponAttachPoint, -1);
		//gameObject.GetComponent<PhotonView> ().RPC ("Attach", PhotonTargets.AllBuffered, rightWeapon, rightWeaponAttachPoint, 1);
		rightWeapon.GetComponent<Weapon> ().pool = poolingSystemR;
		isDead = false;
	}
	[RPC]
	void Attach(Weapon prefab, Transform location, float dir) {
		Transform weaponAttachPoint = prefab.attachPoint;
		Transform prefabTransform = prefab.transform;
		prefabTransform.parent = null;
		weaponAttachPoint.parent = null;
		prefabTransform.parent = weaponAttachPoint;
		weaponAttachPoint.parent = location;
		Vector3 temp = new Vector3 (dir,1f,1f);
		weaponAttachPoint.localScale = temp;
		weaponAttachPoint.localPosition = Vector3.zero;
		weaponAttachPoint.localRotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () {
		//UpdateGUI ();
		State ();

	}

	void FixedUpdate() {
		Regen ();
		if (isControllable) 
		{
			Control ();
		}
		else 
		{
			Pause ();
		}

		if (Input.GetKeyDown(KeyCode.P))
						health = health - health;
	}
	void LateFixedUpdate(){

	}

	void Pause()
	{
		if (!pauseMenu) {
			pauseMenu = true;
			pauseMenuTitle.gameObject.SetActive(true);
			selector.gameObject.SetActive(true);
			for(int i = 0; i < menuOptions.Length;i++)
			{
				menuOptions[i].gameObject.SetActive(true);
			}
				}
		/*
		if (Input.GetButtonDown (startButton)) 
		{
			isControllable = true;
			Debug.Log("Can control: " + isControllable);
			pauseMenu = false;	
			pauseMenuTitle.gameObject.SetActive(false);
			selector.gameObject.SetActive(false);
			for(int i = 0; i < menuOptions.Length;i++)
			{
				menuOptions[i].gameObject.SetActive(false);
			}
		}
		*/
		if(Input.GetAxisRaw(Vertical) > 0 && menuCanMove)
		{
			if (selected > 0) 
			{
				menuCanMove = false;
				selected = selected - 1;
				Debug.Log ("Selected Option: " + selected);
			}
		}
		if(Input.GetAxisRaw(Vertical) < 0 && menuCanMove)
		{
			if (selected < menuOptions.Length - 1) 
			{
				menuCanMove = false;
				selected = selected + 1;
				Debug.Log ("Selected Option: " + selected);
			}
		}
		if (Input.GetAxisRaw (Vertical) == 0) {
			menuCanMove = true;
		}

		Vector3 newPos = menuOptions [selected].position;
		newPos.z += 0.01f;
		selector.transform.position = newPos;

		if (Input.GetButtonDown (jump) || Input.GetKeyDown(KeyCode.Return)) 
		{
			if(selected == 0)
			{

				Debug.Log("Can control: " + isControllable);
				pauseMenu = false;	
				pauseMenuTitle.gameObject.SetActive(false);
				selector.gameObject.SetActive(false);
				for(int i = 0; i < menuOptions.Length;i++)
				{
					menuOptions[i].gameObject.SetActive(false);
				}
				isControllable = true;
			}
			if(selected == 1)
			{
				if(GameObject.FindObjectOfType<NetworkLogic>().online)
					GameObject.FindObjectOfType<NetworkLogic>().Disconnect();
				else
					GameObject.FindObjectOfType<NetworkLogic>().LeaveRoom();
				Application.LoadLevel(0);
			}
		}

	}


	// CONTROLLER METHOD 
	void Control()
	{
		if (Input.GetButtonDown (startButton)) 
		{
			isControllable = false;
			Debug.Log("Can control: " + isControllable);
		}
		Quaternion AddRot = Quaternion.identity;
		float yaw = 0;

		if (Input.GetButtonDown (locker)) {
				lockOn = !lockOn;
		}
		if (lockOn) {
			if (enemyMech != null) {
			transform.LookAt (enemyMech.transform.position);
			}
		}
		//TESTING DASH CODE
		if (Input.GetButtonDown (dash)) 
		{
			if (energy > (0.0f+dashCost)) {
				GetComponent<AudioSource>().Play();
				dashOn = true;
				energy -= dashCost;
				if(Input.GetAxis(Horizontal) == 0 && Input.GetAxis(Vertical) == 0)
				{
					GetComponent<Rigidbody>().AddForce(transform.forward * -1*dashForce);
				}
				else
				{
					GetComponent<Rigidbody>().AddForce(((transform.forward * Input.GetAxis(Vertical)) + (transform.right*Input.GetAxis(Horizontal)))*dashForce);
				}
			}
		}
		if (Input.GetButtonUp (dash)) {
			dashOn = false;
		}
		//old dash code
		/*
		if (Input.GetButton (dash)) {
			dashOn = true;
			if (energy > 0.0f) {
				moveForce = Mathf.Lerp (moveForce, dashForce, 1.0f);
				energy -= dashCost;

			} 
			else 
			{
				moveForce = defaultForce;
			}
		} 
		else {
			dashOn = false;
			moveForce = defaultForce;
		}
		*/
				//WEAPON FIRING
		if ((Input.GetButton (FireLeft) || Input.GetMouseButton(0) )&& energy >= 0) {
						leftFiring = true;
						leftWeapon.isFiring = true;
						leftWeapon.fire ();
						//leftWeapon.GetComponent<PhotonView>().RPC("fire",PhotonTargets.AllBuffered);
				} else {
						leftFiring = false;
						leftWeapon.isFiring = false;
				}
		if ((Input.GetButton (FireRight)|| Input.GetMouseButton(1))  && energy >= 0) {
						rightFiring = true;						
						rightWeapon.isFiring = true;
						rightWeapon.fire ();
								//rightWeapon.GetComponent<PhotonView>().RPC("fire",PhotonTargets.AllBuffered);
						
				} else {
						rightFiring = false;
						rightWeapon.isFiring = false;

				}
				// END WEAPON FIRIN
				if (Input.GetButton (jump)) {
				
						
						jumping = true;
						jumpCount += Time.deltaTime * 1.0f;
						if (energy >= 0.0f) {			

								GetComponent<Rigidbody> ().AddForce (transform.up * Mathf.Lerp (10.0f, jumpForce, 0.8f), ForceMode.Acceleration);

								energy -= hoverCost;
						}
						
				} 
						else if (jumping==true && jumpCount < jumpTime && energy >= 0.0f) {
						GetComponent<AudioSource>().Play();
						jumping = true;
						GetComponent<Rigidbody> ().AddForce (transform.up * (jumpForce* 1.4f), ForceMode.VelocityChange);
						energy -= 5;
						jumping = false;
				
				} else {
						jumpCount = 0;
						jumping = false;
				}
	
			
		
			 
					
		




		if(lockOn)
		{
			//GetComponent<Rigidbody>().AddForce (Input.GetAxisRaw (Horizontal) * transform.right * moveForce,ForceMode.Acceleration);
		}
		else
		{
			yaw = Input.GetAxisRaw(Horizontal) * (Time.fixedDeltaTime * rotationSpeed);
			AddRot.eulerAngles = new Vector3(0, yaw,0);
		}
		GetComponent<Rigidbody>().rotation *= AddRot;
		GetComponent<Rigidbody>().AddForce (Input.GetAxisRaw (Vertical) * transform.forward * moveForce,ForceMode.Acceleration);
	}

	void Regen()
	{
		float currEn = 0.0f;
		if (energy <= 0) {
			if (!(leftFiring || rightFiring) && !dashOn && !jumping) {
					enWait -= Time.deltaTime;
					if (enWait < 0) {
							energy = 1.0f;
							enWait = enCount;
		
					}
	
			}
		} 
		else if (energy <= -5) {
			if (!(leftFiring || rightFiring) && !dashOn && !jumping) {
					enWait -= (Time.deltaTime / 5.0f);
					if (enWait < 0) {
							energy = 1.0f;
							enWait = enCount;
		
					}
	
			}
		} 
		else {
			if (!(leftFiring || rightFiring) && !dashOn && !jumping && energy < energyMax && energy > 0) {
				if (energy < 12.0f) {
					currEn = energyRegen * 1.5f;
				}
				else if (energy < 70.0f) {
					currEn = Mathf.Lerp (energyRegen / 1.1f, energyRegen * 10f, 0.5f);
				} else
							currEn = energyRegen;
					energy += currEn;
			}
		}
	}

	void State()
	{
		if (health <= 0) {
			//Destroy(gameObject);
			if (!isDead) {
				Death ();
				isDead = true;
				isControllable = false;
			}

			//if(isControllable)
				//GetComponent<ParticleSystem>().Emit(1000);


		}
	}
	void OnGui(){
		GUILayout.Label (health + "         " + energy);
	}
//	void UpdateGUI()
//	{				
//		if (health3DText != null) {
//			health3DText.GetComponent<TextMesh> ().text = "Health: " + health.ToString("n2") + "%";
//
//			energy3DText.GetComponent<TextMesh> ().text = "Energy: " + energy.ToString("n2") + "%";
//			recharge3DText.GetComponent<TextMesh> ().text = "Recharge: " + enWait;
//			lives3DText.GetComponent<TextMesh>().text = "Lives: " + lives;
//				
//		}
//	}
	//TRIGGER METHODS
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "firetrap") 
		{
			health -= 0.25f;
		}
		if (other.tag == "Platform") {
			Debug.Log("on platform");
				}
	}

//	void OnCollisionEnter(Collision collision)
//	{
//		if (collision.gameObject.tag == "Platform") {
//			Debug.Log("on platform");
//			transform.parent = collision.gameObject.transform;
//
//		}
//
//	}
//
//	void OnCollisionExit(Collision collision)
//	{
//		if (collision.gameObject.tag == "Platform") {
//			Debug.Log("off platform");
//			transform.parent = null;
//		}
//		
//	}
	[RPC]
	public void TakeDamage(float d){
		this.health -= d;
		if (health <= 0) {
			//Destroy(gameObject);
			if (!isDead) {
				Death ();
				isDead = true;
				isControllable = false;
			}
			
			//if(isControllable)
			//GetComponent<ParticleSystem>().Emit(1000);
			
			
		}
	}
	void Death() {
		// Get right weapon
		GameObject rightWeapon = transform.GetChild (0).GetChild (0).GetChild (0).GetChild (0).gameObject;
		rightWeapon.AddComponent<Rigidbody> ();

		// Get left weapon
		GameObject leftWeapon = transform.GetChild (0).GetChild (1).GetChild (0).GetChild (0).gameObject;
		leftWeapon.AddComponent<Rigidbody> ();

		// Detach everything
		rightWeapon.transform.parent = null;
		leftWeapon.transform.parent = null;
		rightWeapon.GetComponent<Rigidbody> ().AddExplosionForce (3000,transform.position, 10);
		leftWeapon.GetComponent<Rigidbody> ().AddExplosionForce (3000, transform.position, 10);
		if (photonView.isMine) {
			NetworkLogic nl = GameObject.FindObjectOfType<NetworkLogic>();
			transform.FindChild("Main Camera").gameObject.SetActive(true);
			nl.respawnTimer = 3.0f;
			PhotonNetwork.Destroy (gameObject);
		}
	}
}
