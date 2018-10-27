using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFunctionality : MonoBehaviour {
    public GameObject Canvas;
    public GameObject Scalerb;
    public GameObject Rotateb;
    public GameObject Moveb;
    public GameObject Colorb;
    public GameObject Quitb;
    public GameObject photob;
    public GameObject editb;
    public GameObject colorSlider;
    public GameObject ScalerSlider;
    public GameObject RotateSlider;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

   public void SavePicture()
    {
        Canvas.SetActive(false);
        ScreenCapture.CaptureScreenshot("FR"+System.DateTime.Now.ToString("MMddyyyy")+System.DateTime.Now.ToString("hhmmss")+ ".png");
        StartCoroutine(Delay());
       // Canvas.SetActive(true);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);
        Canvas.SetActive(true);



    }
}
