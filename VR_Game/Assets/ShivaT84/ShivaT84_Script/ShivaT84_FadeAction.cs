using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//UI関連(Image)

public class ShivaT84_FadeAction : MonoBehaviour {

	// Use this for initialization
	void Start () {}
		
	// Update is called once per frame
	void Update () {}

	// フェードアウト処理
	public void FadeOutAction(float fadeTime){
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 1.0f,
			"to", 0.0f,
			"time", fadeTime,
			"onupdate", "SetAlphaValue"
		));
	}

	// フェードイン処理
	public void FadeInAction(float fadeTime){
		iTween.ValueTo (gameObject, iTween.Hash(
			"from", 0.0f,
			"to", 1.0f,
			"time", fadeTime,
			"onupdate", "SetAlphaValue"
		));
	}

	// アルファ値を変更する処理
	void SetAlphaValue(float alpha){
		gameObject.GetComponent<Image> ().color = new Color (255,255,255,alpha);
		//Debug.Log("alpha : " + alpha);
	}
}
