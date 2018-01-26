using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShivaT84_FloorManager : MonoBehaviour {
	//========================================================
	// Floor生成の流れ（削除,生成,削除,...の繰り返し）
	//		1.prevFloorに入っているFloorを削除する。
	//			prevFloorにcurrentFloorをセット。
	//		2.新しくFloorを生成する。
	//			currentFloorにnextFloorをセット。
	//			nextFloorに新しく生成したFloorをセット。
	//		3.1に戻る
	//========================================================
	public GameObject player;
	public GameObject[] floorPrefab;
	public List<Transform> wayPoints;//プレイヤーに設定するようの巡回ポジション
	public List<Transform> temp_wayPoints;//一時的に保存用の巡回ポジション
	private Transform wayPoint;
	private Transform floorCreatePoint;
	//[SerializeField]//中身確認用
	private string floorTag;//プレイヤーに設定するようのFloorのタグ
	//[SerializeField]//中身確認用
	private string temp_floorTag;//一時的に保存用のFloorのタグ
	// プロパティ
	public string FloorTag{
		get{return this.floorTag;}//取得
		private set{this.floorTag = value;}//値入力
	}
	public GameObject prevFloor;//前のFloor
	public GameObject currentFloor;//現在のFloor
	public GameObject nextFloor;//次のFloor

	// jumpUpPoints
	private Transform jumpUpPoints;
	private GameObject childJumpUpPoint;
	private GameObject temp_childJumpUpPoint;

	// treasurePoints
	private Transform treasurePoints;
	private GameObject childTreasurePoint;
	private GameObject temp_childTreasurePoint;


	// Use this for initialization
	void Start () {
		// 次に生成するFloorのポジションを取得
		floorCreatePoint = nextFloor.transform.Find("FloorCreatePoint");
	}
	
	// Update is called once per frame
	void Update () {}

	// Floorを自動生成するイベント処理
	public void CreateFloorEvent(){
		Debug.Log("【SM】--START--CreateFloorEvent");
		// FloorPrefabランダム生成
		int randInt_Floor = Random.Range(0, floorPrefab.Length);
		GameObject newNextFloor = GameObject.Instantiate (floorPrefab[randInt_Floor], floorCreatePoint.position, floorPrefab[randInt_Floor].transform.rotation);
		Debug.Log ("【SM】CreateFloor");
		// WayPointsを全て取得してプレイヤーに送信.
		wayPoint = newNextFloor.transform.Find ("WayPoints");
		SetChildObjects (wayPoint);
		// 一時的に保存用のTagセット
		temp_floorTag = newNextFloor.transform.tag;
		Debug.Log("【SM】temp_floorTag:"+temp_floorTag);

		// 生成したFloorのpoint取得
		floorCreatePoint = newNextFloor.transform.Find("FloorCreatePoint");

		// 現在のFloorに次のFloorをセット
		currentFloor = nextFloor;

		// 次のFloorに新しく生成したFloorをセット
		nextFloor = newNextFloor;
		Debug.Log("【SM】nextFloor = newNextFloor");

		// JumpUpPointsオブジェクトの位置を取得.
		if(newNextFloor.tag == "JumpUp"){
			jumpUpPoints = newNextFloor.transform.Find ("JumpUpPoints");
			// アクティブ化させる子オブジェクトをランダムで選択.
			int randInt_ActiveOnObj = Random.Range(0, jumpUpPoints.childCount);
			// ランダムで選ばれたJumpUpPointsオブジェクトの子を設定する.
			temp_childJumpUpPoint = jumpUpPoints.GetChild (randInt_ActiveOnObj).gameObject;
		}

		// 宝のオブジェクトの位置を取得.
		treasurePoints = newNextFloor.transform.Find ("TreasurePoints");
		// アクティブ化させる子オブジェクトをランダムで選択.
		int randInt_ActiveOnTreasure = Random.Range(0, treasurePoints.childCount);
		// ランダムで選ばれたTreasurePointsオブジェクトの子を設定する.
		temp_childTreasurePoint = treasurePoints.GetChild (randInt_ActiveOnTreasure).gameObject;

		Debug.Log("【SM】--END--CreateFloorEvent");
	}

	// Floorを自動削除するイベント処理
	public void DeleteFloorEvent(){
		Debug.Log("【SM】--START--DeleteFloorEvent");
		// 前のFloor削除
		Destroy(prevFloor);
		// 次に削除するFloorをセット
		prevFloor = currentFloor;
		Debug.Log("【SM】prevFloor削除");
		Debug.Log("【SM】prevFloor = currentFloor");

		Debug.Log("【SM】--END--DeleteFloorEvent");

		// タグセット
		this.FloorTag = temp_floorTag;

		// アクティブにするためにJumpupPointを設定
		childJumpUpPoint = temp_childJumpUpPoint;

		// アクティブにするためにTreasurePointを設定
		childTreasurePoint = temp_childTreasurePoint;
	}

	// WayPointsを全て取得
	void SetChildObjects(Transform obj){
		Debug.Log("【SM】--START--SetChildObjects");
		// 初期化処理
		temp_wayPoints.Clear();
		for(int i = 0;i < obj.childCount;i++){
			// 値挿入
			temp_wayPoints.Insert(i, obj.GetChild (i).gameObject.transform);
			Debug.Log("【SM】temp_wayPointsの" + i + "番目に値(" + obj.GetChild (i).gameObject + ")を代入");
		}
		Debug.Log("【SM】--END--SetChildObjects");
	}

	// WayPointsを全て取得
	public void SetWayPoints(){
		Debug.Log("【SM】--START--SetWayPoints");
		// 初期化処理
		wayPoints.Clear();
		Debug.Log ("【SM】temp_wayPoints : " + temp_wayPoints.Count);
		for (int i = 0; i < temp_wayPoints.Count; i++) {
			wayPoints.Insert(i, temp_wayPoints[i].transform);
		}
		Debug.Log("【SM】--END--SetWayPoints");
	}

	// WayPointsの指定要素削除
	public void RemoveAtElementWayPoints(int element){
		Debug.Log("【SM】--START--RemoveAtElementWayPoints");

		if(element == -1){
			// 全削除
			wayPoints.Clear();
		}else{
			// 指定要素削除
			wayPoints.RemoveAt(element);
		}

		Debug.Log("【SM】--END--RemoveAtElementWayPoints");
	}

	// JumpUpPointオブジェクトをアクティブにする.
	public void SetActiveFlagOnJumpUpPoint(){
		Debug.Log("【SM】--START--SetActiveFlagOnJumpUpPoint");

		childJumpUpPoint.SetActive(true);

		Debug.Log("【SM】--END--SetActiveFlagOnJumpUpPoint");
	}

	// TreasurePointオブジェクトをアクティブにする.
	public void SetActiveFlagOnTreasurePoint(){
		Debug.Log("【SM】--START--SetActiveFlagOnTreasurePoint");

		childTreasurePoint.SetActive(true);

		Debug.Log("【SM】--END--SetActiveFlagOnTreasurePoint");
	}
}
