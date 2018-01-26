using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShivaT84_Score : MonoBehaviour {

	private Text c_textScore;//スコアのTextコンポーネント

	// Use this for initialization
	void Start () {
		// スコア
		c_textScore = gameObject.GetComponent<Text> ();//Textコンポーネント取得

		// 今回獲得したスコア表示.
		c_textScore.text = "今回獲得した宝の合計 : " + PlayerPrefs.GetInt("CurrentScore") + "pt";
	}
	
	// Update is called once per frame
	void Update () {}
}
