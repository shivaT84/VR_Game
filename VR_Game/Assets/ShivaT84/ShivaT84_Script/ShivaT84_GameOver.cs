
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;//SceneManager
using System;//Action


public class ShivaT84_GameOver : MonoBehaviour {
	// プレイヤー
	public GameObject player;

	// Image_gameOver
	private Transform child_GameOverImage;

	// シーン遷移関係
	private string loadSceneName = "ShivaT84_EndScene";//シーン遷移名設定
	private float loadSceneWaitTime = 2.0f;//シーン遷移待機時間設定

	// サウンド
	public AudioClip sePlayerBreak;//破壊音
	private AudioSource audioSource;//AudioSource

	// Use this for initialization
	void Start () {
		// Imageの位置取得
		child_GameOverImage = gameObject.transform.Find ("Canvas_GameOver/Image_GameOver");
		// Audioコンポーネント
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		// プレイヤーの位置を反映
		Vector3 temp = player.transform.position;
		temp.z = player.transform.position.z;//z位置をプレイヤーの真下に固定
		temp.y = - 3;// Y位置固定
		transform.position = temp;//反映
	}

	/**
	 * @desc	衝突判定処理
	 * @param	衝突した相手
	*/
	void OnTriggerEnter(Collider other){
		// Floor生成のイベント
		if(other.tag == "Player"){
			GameOverFunc ();
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

	public void GameOverFunc(){
		Debug.Log("=====GameOver=====");

		// サウンド再生
		audioSource.PlayOneShot (sePlayerBreak);

		// EndSceneに表示用のスコア
		PlayerPrefs.SetInt ("CurrentScore", player.GetComponent<ShivaT84_Player>().Score);
		PlayerPrefs.Save ();//確定保存
		// High Score更新
		if(player.GetComponent<ShivaT84_Player>().Score >= PlayerPrefs.GetInt("HighScore")){
			PlayerPrefs.SetInt ("HighScore", player.GetComponent<ShivaT84_Player>().Score);
			PlayerPrefs.Save ();//確定保存
		}

		// フェードイン処理
		child_GameOverImage.gameObject.GetComponent<ShivaT84_FadeAction> ().FadeInAction(0.5f);

		// 指定時間後にシーン遷移
		StartCoroutine (DelayMethod (loadSceneWaitTime, () => {
			SceneManager.LoadScene (loadSceneName);
		}));
	}
}
