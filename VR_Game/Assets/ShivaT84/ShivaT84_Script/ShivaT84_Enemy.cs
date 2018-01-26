using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//Action


public class ShivaT84_Enemy : MonoBehaviour {
	// HP
	[SerializeField]
	private int hp = 5;//HP
	public int Hp{
		get{return this.hp;}//取得
		private set{this.hp = value;}//値入力
	}
	// 攻撃力
	private int attackPoint = 2;//攻撃力
	public int AttackPoint{
		get{return this.attackPoint;}//取得
		private set{this.attackPoint = value;}//値入力
	}
	// true...追跡する, false...追跡しない
	private bool isChase = true;
	// animator
	private Animator animator;
	private bool isPlayDeathAnim = false;
	public float waitTime = 5.0f;

	// プレイヤー情報
	private GameObject player;
	private ShivaT84_Player playerComponent;
	private Transform spawnPosition;//出現位置
	// ヒットエフェクト
	public GameObject particlePrefab;

	// サウンド
	public AudioClip seEnemyAttack01;//攻撃音01
	public AudioClip seEnemyAttack02;//攻撃音02
	private AudioSource audioSource;//AudioSource

	// Use this for initialization
	void Start () {
		// 敵の情報
		animator = GetComponent <Animator> ();//アニメーターコンポーネント
		// HPをセット
		animator.SetInteger ("HP", this.Hp);

		// プレイヤーの情報
		player = GameObject.Find("Player_trokko");
		playerComponent = player.GetComponent<ShivaT84_Player> ();
		spawnPosition = player.transform.Find ("SpawnEnemyPoint");// 敵出現位置
		// Audioコンポーネント
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		// 追跡するかどうか
		if(isChase == true){
			// 常にプレイヤーの隣のレーンの位置を固定する
			transform.parent.position = spawnPosition.transform.position;
		}

		// アニメーションが再生されているかどうか
		if(this.Hp <= 0 && isPlayDeathAnim == false){
			// 1度しか行わない
			isPlayDeathAnim = true;
			// 指定時間後に自身削除処理
			StartCoroutine (DelayMethod (waitTime, () => {
				// 弾を発射OFF
				playerComponent.AutoBulletSwitch(false);
				// 自身破壊処理
				Destroy(gameObject.transform.parent.gameObject);
			}));
		}
	}

	// 自動弾発射スイッチ処理
	public void AutoPlayerBulletSwitch(bool isSwitch){
		playerComponent.AutoBulletSwitch(isSwitch);
	}

	/**
	 * @desc	衝突判定処理
	 * @param	衝突した相手
	*/
	void OnTriggerEnter(Collider other){
		Debug.Log("【Enemy】--START--OnTriggerEnter");

		// 弾衝突処理
		if(other.tag == "Bullet"){
			// 衝突音再生
			other.GetComponent<ShivaT84_Bullet> ().PlaySoundBullet ();

			// HP減少処理
			this.Hp -= 1;
			animator.SetInteger ("HP", this.Hp);

			// Idleモーション中なら,ダメージモーション再生
			animator.SetBool ("isDamage", true);
			// 指定時間後にfalseに戻す
			StartCoroutine (DelayMethod (0.1f, () => {
				animator.SetBool ("isDamage", false);
			}));

			// ぶつかる座標を調べる
			Vector3 hitPosition = GetComponent<Collider>().ClosestPointOnBounds(other.transform.position);
			// ヒットエフェクト生成
			GameObject particle = Instantiate (particlePrefab, hitPosition, Quaternion.identity);
			// ヒットエフェクトを指定時間後に削除
			Destroy(particle, 0.5f);
		}

		// 敵削除イベント
		if (other.tag == "DeleteEnemyEvent") {
			// プレイヤーを追跡をさせなくする
			isChase = false;

			// 親オブジェクト削除
			Destroy (gameObject.transform.parent.gameObject, 5.0f);
		}

		Debug.Log("【Enemy】--END--OnTriggerEnter");
	}

	public void PlaySoundAttack(){
		// ランダム再生
		int randInt_Sound = UnityEngine.Random.Range(0, 2);

		// サウンド再生
		if(randInt_Sound == 0){
			audioSource.PlayOneShot (seEnemyAttack01);
		}else if(randInt_Sound == 1){
			audioSource.PlayOneShot (seEnemyAttack02);
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
