using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShivaT84_Bullet : MonoBehaviour {

	// サウンド
	public AudioClip seBullet;//衝突音
	private AudioSource audioSource;//AudioSource

	// Use this for initialization
	void Start () {
		// Audioコンポーネント
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {}

	public void PlaySoundBullet(){
		// サウンド再生
		audioSource.PlayOneShot (seBullet);
	}
}
