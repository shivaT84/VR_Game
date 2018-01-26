using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ShivaT84_JumpUpPoint : MonoBehaviour {

	// プレイヤー
	private GameObject player;
	private ShivaT84_Player playerComponent;

	private Transform child_GuageImage;//子:円ゲージのImage
	Image c_guageImage;//円ゲージのImageのコンポーネント
	private Transform child_GuageText;//子:ゲージのテキスト
	Text c_guageText;//ゲージのテキストのコンポーネント

	private bool gaugeSwitch = false;//true...マークを見ている, false...マークを見ていない
	private bool isSuccess = false;//成功したかどうか
	private float waitTime = 0.1f;//円ゲージの溜まる時間
	private float destroyTime = 2.0f;//マークを削除する時間

	// サウンド
	public AudioClip seSelect;//選択完了音
	private AudioSource audioSource;


	// Use this for initialization
	void Start () {
		// プレイヤー
		player = GameObject.Find("Player_trokko");
		playerComponent = player.GetComponent<ShivaT84_Player> ();

		// 円ゲージのImage
		child_GuageImage = gameObject.transform.Find ("Canvas/Image_JumpUpGauge");
		c_guageImage = child_GuageImage.GetComponent<Image> ();//Imageのコンポーネント取得
		// 円ゲージのImageのメーターの初期化
		c_guageImage.fillAmount = 0.0f;
		// ゲージのテキスト
		child_GuageText = gameObject.transform.Find ("Canvas/Text_JumpUpGauge");
		c_guageText = child_GuageText.GetComponent<Text> ();//テキストのTextのコンポーネント取得

		// Audioコンポーネント
		audioSource = gameObject.GetComponent<AudioSource>();
	}

	// 円ゲージを溜めるためのスイッチ処理
	public void GageSwitch(bool isSwitch){
		if(isSwitch == true){
			// 溜める
			gaugeSwitch = true;
		} else {
			// 溜めない
			gaugeSwitch = false;
		}
	}

	// Update is called once per frame
	void Update () {
		// 常にプレイヤー方向を向かせる
		gameObject.transform.LookAt(player.transform);

		// まだマークを取得していない
		if (isSuccess == false) {
			// マークを見続けている
			if (gaugeSwitch == true) {
				// 円ゲージを増加させる
				c_guageImage.fillAmount += 1.0f / waitTime * Time.deltaTime;

				// 円ゲージが0以下の時お宝取得(複数取得させないようにする)
				if (c_guageImage.fillAmount >= 1.0f && isSuccess == false) {
					Debug.Log ("SuccessJumpUp");
					// 複数取得させない
					isSuccess = true;
					playerComponent.IsDecision = isSuccess;

					// サウンド再生
					audioSource.PlayOneShot (seSelect);

					//========================================================
					// 成功処理
					//========================================================
					c_guageText.text = "成功!!";

					playerComponent.DirectionMovement = "JumpUp";

					// 削除（2秒後）
					Destroy(gameObject, destroyTime);
				}
			}
		}
	}
}
