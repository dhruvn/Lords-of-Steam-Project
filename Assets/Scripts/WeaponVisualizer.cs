﻿using UnityEngine;
using System.Collections.Generic;

public class WeaponVisualizer : MonoBehaviour {
	
	[SerializeField]
	private Transform leftWeaponAttachPoint;
	[SerializeField]
	private Transform rightWeaponAttachPoint;

	private Dictionary<GameObject, string> names;
	private Object[] prefabs;
	private GameObject[] left;
	private GameObject[] right;
	public int leftSelected = 0;
	public int rightSelected = 0;
	private string leftName;
	private string rightName;

	// Use this for initialization
	void Start () {
		names = new Dictionary<GameObject, string> ();
		prefabs = Resources.LoadAll ("Weapons");
		left = new GameObject[prefabs.Length];
		right = new GameObject[prefabs.Length];
		for (int i = 0; i < prefabs.Length; i++) {
			left[i] = (GameObject)Instantiate(prefabs[i]);
			right[i] = (GameObject)Instantiate(prefabs[i]);
			names[left[i]] = prefabs[i].name;
			names[right[i]] = prefabs[i].name;
			Attach (left[i], leftWeaponAttachPoint, -1);
			Attach(right[i], rightWeaponAttachPoint, 1);
		}
	}

	void Attach(GameObject prefab, Transform location, float dir) {
		Weapon weapon = prefab.GetComponentInChildren<Weapon> ();
		Transform weaponAttachPoint = weapon.attachPoint;
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
		leftName = names[left [leftSelected]];
		rightName = names[right [rightSelected]];
		Debug.Log (Resources.Load("Weapons/"+leftName));
		PlayerPrefs.SetString ("LeftWeapon", leftName);
		PlayerPrefs.SetString ("RightWeapon", rightName);
		for(int i = 0; i < left.Length; i++) {
			left[i].SetActive(i == leftSelected);
			right[i].SetActive(i == rightSelected);
		}
	}
}
