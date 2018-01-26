using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShivaT84_Floor : MonoBehaviour {

	[SerializeField]
	private string floorName = "None";
	// プロパティ
	public string FloorName{
		get{return this.floorName;}//取得
		private set{this.floorName = value;}//値入力
	}
	// 一時的に保存用のFloorName
	private string temp_floorName;
	// プロパティ
	public string Temp_FloorName{
		get{return this.temp_floorName;}//取得
		set{this.temp_floorName = value;}//値入力
	}
	// プレイヤーに送る用のFloorName
	private string playerSend_floorName;
	// プロパティ
	public string PlayerSend_floorName{
		get{return this.playerSend_floorName;}//取得
		set{this.playerSend_floorName = value;}//値入力
	}

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {}
}
