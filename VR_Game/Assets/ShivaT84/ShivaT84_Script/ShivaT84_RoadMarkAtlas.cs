using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ShivaT84_RoadMarkAtlas : MonoBehaviour {

	// 道の標識のAtlas
	public SpriteAtlas roadMarkAtlas;
	Sprite roadMarkSprite;

	// 交通標識の画像名の配列の宣言
	private string[] roadMarkArray = {
		"None",//NONE
		"Instruction_Stop",//停止可
		"Regulation_RoadClosed",//通行止め
		"Warning_YRoad",//Y字の分かれ道注意
		"Warning_FallingRocks",//落石注意
		"Warning_Jump",//ジャンプ注意
		"Instruction_SpinOff",//敵を頭振って落とす
		"Warning_Enemy"//敵出現注意
	};


	// Use this for initialization
	void Start () {
		ChangeRoadMarkSprite(1);
	}
	
	// Update is called once per frame
	void Update () {}

	/**
	 * @desc	道路標識を変化させる
	 * @param	道路標識の配列番号
	 * 			0:NONE
	 * 			1:停止可
	 * 			2:通行止め
	 * 			3:Y字の分かれ道注意
	 * 			4:落石注意
	 * 			5:ジャンプ注意
	 * 			6:敵を頭振って落とす
	 * 			7:敵出現注意
	*/
	public void ChangeRoadMarkSprite(int roadArrayNo){
		// Atlusからsprite取得
		roadMarkSprite = roadMarkAtlas.GetSprite (roadMarkArray[roadArrayNo]);
		// 設定
		gameObject.GetComponent<SpriteRenderer> ().sprite = roadMarkSprite;
	}
}
