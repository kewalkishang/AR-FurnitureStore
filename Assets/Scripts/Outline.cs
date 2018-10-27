using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore.Examples.HelloAR;

public class Outline : MonoBehaviour {

	// Use this for initialization
	void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
        if (HelloARController.SelectedModel==gameObject)
        { gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0.05f);
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.SetFloat("_Outline", 0f);
        }
        }
}
