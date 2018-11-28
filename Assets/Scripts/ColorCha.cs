using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ColorCha : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Material mat = new Material(Shader.Find("Standard"));
        GameObject mod = gameObject.transform.Find("model").gameObject;
        GameObject ss = mod.transform.Find("color").gameObject;
        foreach (Transform child in ss.transform)
        {
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            rend.material = mat;
            rend.material.SetColor("_Color", new Color(0,0,0));
           // Debug.Log("color" + ;
        }
    }
	
    public void ChangeRed(Slider s)
    {
        foreach (Transform child in gameObject.transform.Find("model").gameObject.transform.Find("color").gameObject.transform)
        {
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            rend.material.SetColor("_Color", new Color(s.value, rend.material.color[1], rend.material.color[2]));
        }
        //   gameObject
    }

    public void ChangeGreen(Slider s)
    {
        foreach (Transform child in gameObject.transform.Find("model").gameObject.transform.Find("color").gameObject.transform)
        {
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            rend.material.SetColor("_Color", new Color(rend.material.color[0], s.value, rend.material.color[2]));
        }
    }

    public void ChangeBlue(Slider s)
    {
        foreach (Transform child in gameObject.transform.Find("model").gameObject.transform.Find("color").gameObject.transform)
        {
            Renderer rend = child.gameObject.GetComponent<Renderer>();
            rend.material.SetColor("_Color", new Color(rend.material.color[0], rend.material.color[1], s.value));
        }
    }
}
