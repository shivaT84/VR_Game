using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;//Action
using UnityEngine.SceneManagement;//SceneManager

public class ShivaT84_Player : MonoBehaviour {

	//========================================================
	// 基本情報
	//========================================================
	private float moveSpeed = 10.0f;//プレイヤーの移動速度
	private int maxHp = 10;//MaxHP
	private int Hp = 10;//現在のHP
	private Transform child_HpGaugeImage;//HPゲージImage
	Image c_hpGuageImage;//HPゲージのImageのコンポーネント

	private Transform child_BloodImage;//BloodImage
	Image c_bloodImage;//BloodのImageのコンポーネント

	Rigidbody rbPlayer;
	public GameObject systemManager;//システムマネージャー
	public GameObject obj_gameOver;//ゲームオーバーObj
	private Transform child_GameOverImage;// Image_gameOver
	private ShivaT84_FloorManager smComponent;
	public int currentRoot;
	private bool gameStartSwitch = false;//true...ゲーム開始, false...ゲーム開始前
	private bool autoBulletSwitch = false;//true...撃つ, false...撃たない
	// ゲームスタートUI
	private GameObject canvas_GamestartObj;

	//========================================================
	// スコア
	//========================================================
	// プロパティ
	private int highScore = 0;
	private int score = 0;//合計のスコア
	public int Score{
		get{return this.score;}//取得
		private set{this.score = value;}//値入力
	}
	// ハイスコア
	private GameObject highPointObj;
	private Text highPointText;
	// 現在のスコア
	private Transform child_pointText;//子:ポイントのテキスト
	private Transform child_scoreText;//子:スコアのテキスト
	Text c_pointText;//スコアのテキストのコンポーネント
	//========================================================
	// 敵関係
	//========================================================
	public GameObject enemyPrefab;//敵
	private Transform child_enemyPosition;//敵出現位置
	private ShivaT84_Enemy sEnemy;//スケルトンについているEnemyスクリプト
	//========================================================
	// 弾発射関係
	//========================================================
	private float bulletWaitTime = 0.5f;//弾の発射間隔
	private float bulletCounter = 0.0f;//弾のカウンター
	public GameObject bulletPrefab;//発射する弾
	private Transform child_muzzle;//銃口
	private float shotSpeed = 5000;//弾の速度
	//========================================================
	// 交通標識関係
	//========================================================
	private bool isRoadEvent = false;//true...イベント中, false...イベント外
	private bool isAutoMove = false;//true...自動移動, false...探索移動
	private bool isRight = false;//true...正解, false...不正解
	private bool isRoll = false;//true...横転させる
	private bool AutoMoveFlag = false;//true...不正解の道を選んだ, false...特になし
	private Transform child_roadMarkSprite;//子:交通標識の画像
	private Transform child_roadMarkText;//子:交通標識のテキスト
	Text c_roadMarkText;//交通標識のテキストのコンポーネント
	// 警告文の配列の宣言
	private string[] roadMarkTextArray = {
		"",//NONE
		"[Start]を\n見続けて開始しよう",//ゲームスタート前
		"さあ、冒険の始まりだ！\n沢山の宝を集めよう",//ゲーム開始後すぐ
		"トロッコに掴まれる前に倒せ！",//敵出現
		"トロッコを\n左に傾けろ！",//Y字[左]の分かれ道注意
		"トロッコを\n右に傾けろ！",//Y字[右]の分かれ道注意
		"岩を破壊するんだ！",//落石注意
		"ジャンプするために\n的を探せ！",//ジャンプ注意
		"頭を左右に振ると\n敵を振り落とせるぞ!",//敵を振り落とせる道
		"敵の気配が...\n敵の方向を見て攻撃だ！",//敵出現する道
		"右に行くぞ！",//右に決定
		"左に行くぞ！",//左に決定
		"...時間切れだ"//左右どちらにも決めなかったとき
	};
	//========================================================
	// 左右移動関係
	//========================================================
	// MainCamera
	private Transform child_mainCamera;//子:MainCamera
	private Transform child_curveGauge;
	// カーブゲージ
	private Transform child_rightGaugeImage;//子:右カーブのImage
	Image c_rightGaugeImage;//右カーブのImageのコンポーネント
	private Transform child_leftGaugeImage;//子:左カーブのImage
	Image c_leftGaugeImage;//左カーブのImageのコンポーネント

	private bool CurveGaugeSwitch = false;//true...Y字イベント中, false...Y字イベント外
	private bool isDecision = false;//道を決めたかどうか
	public bool IsDecision{
		get{return this.isDecision;}//取得
		set{this.isDecision = value;}//値入力
	}
	// 移動方向
	private string[] movementArray = {
		"None",//NONE
		"Right",//右方向
		"Left",//左方向
		"JumpUp"//ジャンプ
	};
	//[SerializeField]//中身確認用
	private string directionMovement = "None";//決定した移動方向
	public string DirectionMovement{
		get{return this.directionMovement;}//取得
		set{this.directionMovement = value;}//値入力
	}


	void Awake(){
		// HighScoreが設定されているかどうか
		if(!PlayerPrefs.HasKey("HighScore")){
			// 設定されていない場合初期化
			PlayerPrefs.SetInt("HighScore", 0);
		}else{
			// 設定されている場合ハイスコア取得
			highScore = PlayerPrefs.GetInt("HighScore");
		}
	}

	// Use this for initialization
	void Start () {
		// プレイヤーのRigidbody
		rbPlayer = GetComponent<Rigidbody>();
		// システムマネージャー
		smComponent = systemManager.GetComponent<ShivaT84_FloorManager>();
		// ゲームスタートUI
		canvas_GamestartObj = GameObject.Find ("Canvas_GameStart");
		// HPゲージImage
		child_HpGaugeImage = gameObject.transform.Find ("Canvas/HpGauge_ImageBg/HpGauge_Image");
		c_hpGuageImage = child_HpGaugeImage.GetComponent<Image> ();//Imageのコンポーネント取得
		// 血のImage
		child_BloodImage = obj_gameOver.transform.Find ("Canvas_GameOver/Image_Blood");
		c_bloodImage = child_BloodImage.GetComponent<Image> ();//Imageのコンポーネント取得
		// スコア
		highPointObj = GameObject.Find ("Text_HighPoint");
		highPointObj.GetComponent<Text> ().text = highScore.ToString();
		// 敵
		child_enemyPosition = gameObject.transform.Find ("SpawnEnemyPoint");// 敵出現位置
		// 銃口
		child_muzzle = gameObject.transform.Find ("MainCamera/Muzzle");
		// スコアのテキスト
		child_pointText = gameObject.transform.Find ("Canvas/Text_Point");
		c_pointText = child_pointText.GetComponent<Text> ();//Textのコンポーネント取得
		child_pointText.gameObject.SetActive(false);
		child_scoreText = gameObject.transform.Find ("Canvas/Text_Score");
		child_scoreText.gameObject.SetActive(false);
		// 交通標識
		child_roadMarkSprite = gameObject.transform.Find ("Canvas/RoadMark");
		child_roadMarkText = gameObject.transform.Find ("Canvas/RoadMark_Text");
		c_roadMarkText = child_roadMarkText.GetComponent<Text> ();//Textのコンポーネント取得
		c_roadMarkText.text = roadMarkTextArray [1];//ゲームスタート前
		// MainCamera
		child_mainCamera = gameObject.transform.Find("MainCamera");
		// カーブゲージ
		child_curveGauge = gameObject.transform.Find ("Canvas/CurveGauge");
		child_rightGaugeImage = gameObject.transform.Find ("Canvas/CurveGauge/Image_RightGauge");
		c_rightGaugeImage = child_rightGaugeImage.GetComponent<Image> ();//右カーブのImageのコンポーネント取得
		c_rightGaugeImage.fillAmount = 0.0f;//右ゲージの初期化
		child_leftGaugeImage = gameObject.transform.Find ("Canvas/CurveGauge/Image_LeftGauge");
		c_leftGaugeImage = child_leftGaugeImage.GetComponent<Image> ();//左カーブのImageのコンポーネント取得
		c_leftGaugeImage.fillAmount = 0.0f;//左ゲージの初期化
		child_curveGauge.gameObject.SetActive(false);//カーブゲージの最後に書く

		// 画面を明るくする
		child_GameOverImage = obj_gameOver.transform.Find ("Canvas_GameOver/Image_GameOver");
		child_GameOverImage.GetComponent<ShivaT84_FadeAction> ().FadeOutAction(1.0f);
	}

	// Update is called once per frame
	void Update () {
		// 左右の移動処理(傾きによる移動)
		LeftAndRightMovement ();

		// 自動移動に変更
		if(isAutoMove == true){
			AutoMove ();
		}

		// 自動弾発射処理
		if(autoBulletSwitch == true){
			if(bulletCounter <= 0){
				// 弾発射
				BulletShot ();
				// カウンターの初期化
				bulletCounter = bulletWaitTime;
			}
			bulletCounter -= Time.deltaTime;
			//Debug.Log (bulletCounter);
		}

		// 横転させる
		if(isRoll == true){
			gameObject.transform.Rotate(new Vector3(0, 0, 2));
		}
	}

	// 経路探索の移動処理
	void MoveToPath(){
		// 自動移動でないとき
		if (isAutoMove == false) {
			Debug.Log ("【Player】--START--MoveToPath");
			Debug.Log ("【Player】wayPointを取得して移動");
			Debug.Log ("【Player】currentRoot : " + currentRoot);
			Vector3 pos = smComponent.wayPoints [currentRoot].position;
			// 計算する距離によって動く時間を計算
			float moveTime = Vector3.Distance (transform.position, pos) / 2 / 5;
			iTween.MoveTo (gameObject, iTween.Hash (
				"position", smComponent.wayPoints [currentRoot],//移動位置指定
				"time", moveTime,//アニメーション完了までの時間指定
				"easetype", "linear",//アニメーションの緩急指定(直線的)
				"oncomplete", "MoveToPath",//アニメーションが終わった後に呼ぶメソッド名指定
				"Looktarget", smComponent.wayPoints [currentRoot].position,//指定方向に向く
				"looktime", 2.0//姿勢転換の早さを指定()
			));
			Debug.Log (smComponent.wayPoints [currentRoot]);

			currentRoot++;
			Debug.Log ("【Player】移動設定完了");
			Debug.Log ("【Player】currentRoot++");
			Debug.Log ("【Player】currentRoot : " + currentRoot); 

			if (currentRoot > smComponent.wayPoints.Count - 1) {
				// 位置の初期化.
				currentRoot = 0;
				Debug.Log ("【Player】currentRootの初期化");

				// 新しく移動ポイントを設定
				if(isDecision == false || (isDecision == true && isRight == true)){
					smComponent.SetWayPoints();
				}
			}
			//Debug.Log(currentRoot);
			Debug.Log ("【Player】--END--MoveToPath");
		}
	}

	// 自動移動処理
	void AutoMove(){
		Debug.Log("【Player】--START--AutoMove");
		// 移動方向を取得
		transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
		Debug.Log("【Player】--END--AutoMove");
	}

	/**
	 * @desc	衝突判定処理
	 * @param	衝突した相手
	*/
	void OnTriggerEnter(Collider other){

		Debug.Log("【Player】--START--OnTriggerEnter");

		// Floor生成のイベント
		if(other.tag == "FloorCreateEvent"){
			smComponent.CreateFloorEvent();
		}
			
		// Floor削除のイベント
		if(other.tag == "FloorDeleteEvent"){
			smComponent.DeleteFloorEvent();
			// 変数の初期化処理
			InitVariable ();
		}

		// 重力のイベント
		if (other.tag == "GravityOnEvent") {
			rbPlayer.useGravity = true;
		}

		// 横転イベント
		if (other.tag == "RollEvent") {
			if(isDecision == false){
				isRoll = true;
				// 時間切れを表示
				c_roadMarkText.text = roadMarkTextArray [12];

				// 指定時間(1秒)後にゲームオーバー処理
				StartCoroutine (DelayMethod (1.0f, () => {
					obj_gameOver.GetComponent<ShivaT84_GameOver> ().GameOverFunc ();
				}));
			}
		}

		// 自動移動に変更するイベント(真っ直ぐ直進してゲームオーバー)
		if (other.tag == "AutoMoveEvent" && (isDecision == false || isRight == false)) {
			Debug.Log("【Player】isDecision : " + isDecision + " , isRight : " + isRight);
			// 答えを出したが間違っているとき
			if (AutoMoveFlag == true || isDecision == false) {
				isAutoMove = true;
				// WayPointsの要素全削除
				smComponent.RemoveAtElementWayPoints(-1);
			} else if (isDecision == true && isRight == false) {
				AutoMoveFlag = true;
			}
		}

		// Y字の分かれ道のイベント処理
		if(other.tag == "Y_RoadLeftEvent" || other.tag == "Y_RoadRightEvent"){
			// イベントに入るたびに切り替える
			if(isRoadEvent == false) {
				isRoadEvent = true;//イベント中
			} else {
				isRoadEvent = false;//イベント外
			}
			// 標識の切り替え
			if (isRoadEvent == true) {
				// Y字イベント中
				CurveGaugeSwitch = true;
				// 「警告」表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 3);
				if(other.tag == "Y_RoadLeftEvent"){
					// Left
					c_roadMarkText.text = roadMarkTextArray [4];//Y字の分かれ道注
				}else if(other.tag == "Y_RoadRightEvent"){
					// Right
					c_roadMarkText.text = roadMarkTextArray [5];//Y字の分かれ道注
				}
				// curveGaugeオブジェクトをアクティブにする
				child_curveGauge.gameObject.SetActive (true);
				// TreasurePointを表示
				smComponent.SetActiveFlagOnTreasurePoint ();
			} else {
				// Y字イベント外
				CurveGaugeSwitch = false;
				// 「警告」非表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 0, SendMessageOptions.RequireReceiver);
				c_roadMarkText.text = roadMarkTextArray [0];//NONE
				// curveGaugeオブジェクトを非アクティブにする
				child_curveGauge.gameObject.SetActive (false);

				// 正解の方向を選んだかどうか確認
				isRight = CheckRightDirection();
			}
		}

		// ジャンプイベント処理
		if(other.tag == "JumpUpEvent"){
			// イベントに入るたびに切り替える
			if(isRoadEvent == false) {
				isRoadEvent = true;//イベント中
			} else {
				isRoadEvent = false;//イベント外
			}
			// 標識の切り替え
			if (isRoadEvent == true) {
				// 「警告」表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 5);
				c_roadMarkText.text = roadMarkTextArray [7];//ジャンプ注
				// JumpUpPointを表示
				smComponent.SetActiveFlagOnJumpUpPoint ();
				// TreasurePointを表示
				smComponent.SetActiveFlagOnTreasurePoint ();
			} else {
				// 「警告」非表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 0, SendMessageOptions.RequireReceiver);
				c_roadMarkText.text = roadMarkTextArray [0];//NONE

				// 正解の方向を選んだかどうか確認
				isRight = CheckRightDirection();
			}
		}

		// 敵を振り落とすイベント
		if (other.tag == "SpinOffEnemyEvent") {
			// イベントに入るたびに切り替える
			if(isRoadEvent == false) {
				isRoadEvent = true;//イベント中
			} else {
				isRoadEvent = false;//イベント外
			}
			// 標識の切り替え
			if (isRoadEvent == true) {
				// 「警告」表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 6);
				c_roadMarkText.text = roadMarkTextArray [8];//敵を振り落とせる道
				// TreasurePointを表示
				smComponent.SetActiveFlagOnTreasurePoint ();
			} else {
				// 「警告」非表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 0, SendMessageOptions.RequireReceiver);
				c_roadMarkText.text = roadMarkTextArray [0];//NONE
			}
		}

		// 敵出現イベント
		if (other.tag == "SpawnEnemyEvent") {
			// イベントに入るたびに切り替える
			if(isRoadEvent == false) {
				isRoadEvent = true;//イベント中
			} else {
				isRoadEvent = false;//イベント外
			}
			// 標識の切り替え
			if (isRoadEvent == true) {
				// 「警告」表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 7);
				c_roadMarkText.text = roadMarkTextArray [9];//敵出現する道
				// 指定時間(0.5秒)後に敵生成処理
				StartCoroutine (DelayMethod (1.0f, () => {
					// 隣のレーンの位置に敵を生成する.
					Instantiate (enemyPrefab, child_enemyPosition.position, child_enemyPosition.rotation);
				}));
				// TreasurePointを表示
				smComponent.SetActiveFlagOnTreasurePoint ();
			} else {
				// 「警告」非表示
				child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 0, SendMessageOptions.RequireReceiver);
				c_roadMarkText.text = roadMarkTextArray [0];//NONE
			}
		}

		// 敵の攻撃を受ける処理
		if (other.tag == "EnemyAttack") {
			// 衝突してきた敵のEnemyスクリプト参照
			sEnemy = other.transform.parent.parent.parent.GetComponent<ShivaT84_Enemy> ();
			// 敵が死んでいたら無視する
			if(sEnemy.Hp >= 1){
				// HPゲージ減少処理
				Hp -= sEnemy.AttackPoint;
				c_hpGuageImage.fillAmount = (float) Hp/ (float)maxHp;

				// 血の表示
				c_bloodImage.gameObject.GetComponent<ShivaT84_FadeAction> ().FadeInAction(0.2f);

				// 指定時間(0.5秒)後にフェードアウト処理
				StartCoroutine (DelayMethod (0.5f, () => {
					c_bloodImage.gameObject.GetComponent<ShivaT84_FadeAction> ().FadeOutAction(1.0f);
				}));
					
				// サウンド再生
				sEnemy.PlaySoundAttack();

				// ゲームオーバー処理
				if(Hp <= 0){
					obj_gameOver.GetComponent<ShivaT84_GameOver> ().GameOverFunc ();
				}
			}
		}

		Debug.Log("【Player】--END--OnTriggerEnter");
	}

	// ゲーム開始処理
	public void GameStartSwitchOn(){
		Debug.Log("【Player】--START--GameStartSwitchOn");
		gameStartSwitch = true;
		// ゲームスタートUI削除
		Destroy (canvas_GamestartObj);
		// アクティブON
		child_pointText.gameObject.SetActive(true);
		child_scoreText.gameObject.SetActive(true);
		// 「ゲーム開始時」の文字表示
		child_roadMarkSprite.SendMessage("ChangeRoadMarkSprite", 0, SendMessageOptions.RequireReceiver);
		c_roadMarkText.text = roadMarkTextArray [2];//ゲーム開始時

		// 経路探索の移動処理
		MoveToPath ();
		Debug.Log("【Player】--END--GameStartSwitchOn");
	}

	/**
	 * @desc	スコアの加点処理
	 * @param	加点数
	*/
	public void AddScore(int point){
		Debug.Log("【Player】--START--AddScore");
		this.Score += point;
		c_pointText.text = (this.Score).ToString ();

		Debug.Log("【Player】--END--AddScore");
	}

	/**
	 * @desc	自動で弾を発射するかどうか
	 * @param	true...発射する, false...発射しない
	*/
	public void AutoBulletSwitch(bool isSwitch){
		Debug.Log("【Player】--START--AutoBulletSwitch");

		autoBulletSwitch = isSwitch;

		Debug.Log("【Player】--END--AutoBulletSwitch");
	}

	// プレイヤーの弾を定期的に発射させる
	public void BulletShot(){
		Debug.Log("【Player】--START--BulletShot");

		// 銃口の位置に弾を生成する
		GameObject bullet = Instantiate (bulletPrefab, child_muzzle.position, child_muzzle.rotation);
		// プレイヤーの弾の物理特性を取得して、力を加える
		bullet.GetComponent<Rigidbody> ().AddForce(bullet.transform.forward * shotSpeed);
		Destroy(bullet, 1.5f);//1.5秒後に消去

		Debug.Log("【Player】--END--BulletShot");
	}

	/**
	 * @desc	左右の移動処理
	 * @tips	z軸(Yaw)回転（左:+, 右:-）
	*/
	void LeftAndRightMovement(){
		// Y字イベント中
		if (CurveGaugeSwitch == true && isDecision == false) {
			// 円ゲージを増加させる(MainCameraのZ回転分)
			if(child_mainCamera.transform.localRotation.z > 0 && (c_leftGaugeImage.fillAmount <= 1.0f || c_rightGaugeImage.fillAmount <= 1.0f)) {
				// 左ゲージ増加
				c_leftGaugeImage.fillAmount += child_mainCamera.transform.localRotation.z / 10;
				//Debug.Log ("LeftGaugeCharge");
			} else {
				// 右ゲージ増加
				c_rightGaugeImage.fillAmount -= child_mainCamera.transform.localRotation.z / 10;
				//Debug.Log ("RightGaugeCharge");
			}
			//Debug.Log (child_mainCamera.transform.rotation.z / 10);
				
			// ゲージが1以上の時、移動方向決定(複数回させないようにする)
			if ((c_rightGaugeImage.fillAmount >= 1.0f || c_leftGaugeImage.fillAmount >= 1.0f)) {
				Debug.Log ("【Player】Decision");
				// 複数回決定させない
				isDecision = true;

				// 右ゲージが1以上の時
				if (c_rightGaugeImage.fillAmount >= 1.0f) {
					c_roadMarkText.text = roadMarkTextArray [10];
					directionMovement = movementArray [1];//右方向
					// WayPointsの指定要素削除
					smComponent.RemoveAtElementWayPoints(2);
					smComponent.RemoveAtElementWayPoints(2);
					smComponent.RemoveAtElementWayPoints(2);
					smComponent.RemoveAtElementWayPoints(2);
					smComponent.RemoveAtElementWayPoints(2);
					smComponent.RemoveAtElementWayPoints(2);
					smComponent.RemoveAtElementWayPoints(2);
					smComponent.RemoveAtElementWayPoints(2);
					Debug.Log ("【Player】WayPointsの左道の指定要素削除");

				} else if (c_leftGaugeImage.fillAmount >= 1.0f) {
					c_roadMarkText.text = roadMarkTextArray [11];
					directionMovement = movementArray [2];//左方向
					// WayPointsの指定要素削除
					smComponent.RemoveAtElementWayPoints(10);
					smComponent.RemoveAtElementWayPoints(10);
					smComponent.RemoveAtElementWayPoints(10);
					smComponent.RemoveAtElementWayPoints(10);
					smComponent.RemoveAtElementWayPoints(10);
					smComponent.RemoveAtElementWayPoints(10);
					smComponent.RemoveAtElementWayPoints(10);
					smComponent.RemoveAtElementWayPoints(10);
					Debug.Log ("【Player】WayPointsの右道の指定要素削除");
				} else {
					directionMovement = movementArray [0];//NONE
				}
			}
		}
	}

	/**
	 * @desc	正解の方向を選んだかどうか確認
	 * @return	true...正解, false...不正解
	*/
	bool CheckRightDirection(){
		Debug.Log("【Player】--START--CheckRightDirection");

		Debug.Log("【Player】SELECT:" + directionMovement);
		Debug.Log("【Player】TAG:" + smComponent.FloorTag);
		// 選んだ方向が正解なら
		if(directionMovement == smComponent.FloorTag){
			Debug.Log("【Player】======TRUE======");
			return true;
		}else{
			Debug.Log("【Player】======FALSE======");
			return false;
		}
		Debug.Log("【Player】--END--CheckRightDirection");
	}

	// 変数の初期化処理
	void InitVariable(){
		Debug.Log("【Player】--START--InitVariable");
		// 道を決めたかどうか
		isDecision = false;
		// 正解かどうか
		isRight = false;
		// 不正解の道を選んだかどうか
		AutoMoveFlag = false;
		//決定した移動方向
		directionMovement = movementArray [0];//NONE
		// ゲージ
		c_rightGaugeImage.fillAmount = 0.0f;
		c_leftGaugeImage.fillAmount = 0.0f;
		Debug.Log("【Player】--END--InitVariable");
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

