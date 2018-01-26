using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class ShivaT84_CircleGauge : MonoBehaviour {

	// ボタンゲージとして扱うかどうか
	[SerializeField]
	private bool isButton = true;

	private Transform child_GuageImage;//子:円ゲージのImage
	Image c_guageImage;//円ゲージのImageのコンポーネント
	private Transform child_GuageText;//子:ゲージのテキスト
	Text c_guageText;//ゲージのテキストのコンポーネント

	private bool gaugeSwitch = false;//true...マークを見ている, false...マークを見ていない
	private bool isSuccess = false;//成功したかどうか
	public float fullGaugeTime = 3.0f;//円ゲージの溜まる時間
	private float destroyTime = 2.0f;//円ゲージを削除する時間

	// シーン遷移関係
	[SerializeField]
	private string loadSceneName = "";//シーン遷移名設定
	private float loadSceneWaitTime = 1.0f;//シーン遷移待機時間設定
	// プレイヤー
	private GameObject player;
	private ShivaT84_Player playerComponent;

	// サウンド
	public AudioClip seSelect;//選択完了音
	private AudioSource audioSource;


	// Use this for initialization
	void Start () {
		// 円ゲージのImage
		child_GuageImage = gameObject.transform.Find ("Canvas/Image_CircleGauge");
		c_guageImage = child_GuageImage.GetComponent<Image> ();//Imageのコンポーネント取得
		// 円ゲージのImageのメーターの初期化
		c_guageImage.fillAmount = 0.0f;
		// ゲージのテキスト
		child_GuageText = gameObject.transform.Find ("Canvas/Text_CircleGauge");
		c_guageText = child_GuageText.GetComponent<Text> ();//テキストのTextのコンポーネント取得
		if (isButton == false) {
			// テキストを非アクティブに設定
			child_GuageText.gameObject.SetActive (false);
		}
		// ゲームメイン時のみ
		if(Application.loadedLevelName == "ShivaT84_MainScene"){
			// プレイヤー取得
			player = GameObject.Find("Player_trokko");
			playerComponent = player.GetComponent<ShivaT84_Player> ();
		}

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
		// まだ宝を取得していない
		if (isSuccess == false) {
			// 宝を見続けている
			if (gaugeSwitch == true) {
				// 円ゲージを増加させる
				c_guageImage.fillAmount += 1.0f / fullGaugeTime * Time.deltaTime;

				// 円ゲージが0以下の時お宝取得(複数取得させないようにする)
				if (c_guageImage.fillAmount >= 1.0f && isSuccess == false) {
					Debug.Log ("FullGauge");
					// 複数取得させない
					isSuccess = true;

					// サウンド再生
					audioSource.PlayOneShot (seSelect);

					//========================================================
					// 成功処理
					//========================================================
					// シーン遷移があれば...
					if (string.IsNullOrEmpty(loadSceneName) == false) {
						// テキスト変更
						c_guageText.text = "NowLoading...";
						// 1.0秒後にシーン遷移
						StartCoroutine (DelayMethod (loadSceneWaitTime, () => {
							SceneManager.LoadScene (loadSceneName);
						}));
					} else if (loadSceneName == "GameEnd") {// ゲーム終了処理
						// テキスト変更
						c_guageText.text = "お疲れ様でした";
						// 指定時間後にゲームを終了する
						StartCoroutine (DelayMethod (loadSceneWaitTime, () => {
							Application.Quit();
						}));
					} else if (Application.loadedLevelName == "ShivaT84_MainScene") {
						// Player.csの関数を呼び出す
						playerComponent.GameStartSwitchOn ();
					}

					if (isButton == false) {
						// テキストをアクティブに設定して表示
						child_GuageText.gameObject.SetActive (true);
					}
					// 指定時間後に削除
					Destroy(gameObject, destroyTime);
				}
			}
		}
	}

	/**
	* @desc		渡された処理を指定時間後に実行する.
	* @param	遅延時間[ms]
	* @param	実行したい処理
	*/
	private IEnumerator DelayMethod(float waitTime, Action action){
		yield return new WaitForSeconds (waitTime);
		action ();
	}
}
