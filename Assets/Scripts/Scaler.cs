using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore.Examples.HelloAR;

public class Scaler : MonoBehaviour {

    public GameObject ANDY;
    GameObject pg = null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        pg = HelloARController.SelectedModel;

    }
    public void Scale(Slider sl)
    {
        
        Vector3 sc = new Vector3(sl.value, sl.value,sl.value);
        Debug.Log("scA" + sl.value);
        pg.transform.localScale = sc;
        

    }

    public void Rotate(Slider sl)
    {
        Vector3 sc = new Vector3(0, sl.value,0);
        Debug.Log("scA" + sl.value);
        pg.transform.rotation= Quaternion.Euler(sc);
        //Debug.Log("scale" + transform.localScale);
    }
}
