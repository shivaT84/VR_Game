using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShivaT84_Treasure : MonoBehaviour {

	private Transform child_GuageImage;//子:円ゲージ
	Image c_guageImage;//円ゲージのImageのコンポーネント
	private Transform child_ScoreText;//子:スコアのテキスト
	Text c_scoreText;//スコアのTextのコンポーネント

	private GameObject player;//プレイヤー
	private bool gaugeSwitch = false;//true...宝を見ている, false...宝を見ていない
	private bool isGet = false;//宝を手に入れたかどうか
	private float waitTime = 1.0f;//円ゲージの溜まる時間
	public int getScore = 10;//手に入れるスコアの点数
	private float destroyTime = 2.0f;//宝を削除する時間

	// クローズトレジャー(入手前の宝)
	private Transform cCloseTreasure;
	// オープントレジャー(入手後の宝)
	private Transform cOpenTreasure;

	Animator animScoreText;//テキストのアニメーション

	// サウンド
	public AudioClip moneyDropLittle;//金額が少ない
	public AudioClip moneyDropMuch;//金額が多い
	private AudioSource audioSource;

	// Use this for initialization
	void Start () {
		// プレイヤー取得
		player = GameObject.Find("Player_trokko");
		// 円ゲージ
		child_GuageImage = gameObject.transform.Find ("Canvas/Image_Gauge");
		c_guageImage = child_GuageImage.GetComponent<Image> ();//Imageのコンポーネント取得
		// 円ゲージのメーターの初期化
		c_guageImage.fillAmount = 0.0f;
		// スコア
		child_ScoreText = gameObject.transform.Find ("Canvas/Text_Score");
		c_scoreText = child_ScoreText.GetComponent<Text> ();//テキストのTextのコンポーネント取得
		// 宝ごとのスコアの初期化
		c_scoreText.text = "";
		// テキストのAnimatorのコンポーネントの取得
		animScoreText = child_ScoreText.GetComponent<Animator>();

		// クローズトレジャー(入手前の宝)検索
		cCloseTreasure = gameObject.transform.Find ("chest_close");
		// オープントレジャー(入手後の宝)検索
		cOpenTreasure = gameObject.transform.Find ("chest_open");
		cOpenTreasure.gameObject.SetActive(false);

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
		if (isGet == false) {
			// 宝を見続けている
			if (gaugeSwitch == true) {
				// 円ゲージを増加させる
				c_guageImage.fillAmount += 1.0f / waitTime * Time.deltaTime;

				// 円ゲージが0以下の時お宝取得(複数取得させないようにする)
				if (c_guageImage.fillAmount >= 1.0f && isGet == false) {
					Debug.Log ("getTreasure");
					// 複数取得させない
					isGet = true;
					//========================================================
					// 宝を手に入れる処理
					//========================================================
					// 手に入れたスコアの表示
					c_scoreText.text = getScore.ToString();
					// テキストを徐々に上に移動させる
					animScoreText.SetBool("isSuccess", true);
					// スコアアップ(Player.csの関数を呼び出す)
					player.GetComponent<ShivaT84_Player>().AddScore(getScore);

					// 宝の見た目をオープンにする.
					cCloseTreasure.gameObject.SetActive(false);
					cOpenTreasure.gameObject.SetActive(true);

					// サウンド再生
					if (getScore <= 10) {
						audioSource.PlayOneShot (moneyDropLittle);
					} else {
						audioSource.PlayOneShot (moneyDropMuch);
					}

					// 削除（2秒後）
					Destroy(gameObject, destroyTime);
				}
			}
		}
	}
}
