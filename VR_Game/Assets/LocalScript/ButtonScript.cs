using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour {

    [SerializeField]
    private string loadName = "TitleScene";

    //クリックしたときの関数
    public void OnClick(){

        //シーン遷移
        SceneManager.LoadScene(loadName);
    }
}
