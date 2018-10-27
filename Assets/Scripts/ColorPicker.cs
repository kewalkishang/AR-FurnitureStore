using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour {
    float red;
    float green;
    float blue;
   public Slider reds;
    public Slider greens;
    public Slider blues;
    public RawImage img;
    public GameObject andy;
    Renderer rend;
    void Start()
    {
        rend = andy.GetComponent<Renderer>();
        blues.value = rend.material.color.b;
       reds.value = rend.material.color.r;
       greens.value = rend.material.color.g;
    }
	// Use this for initializatio

    public void GetGreen(Slider s)
    {
        green = s.value;
    }

    public void GetRed(Slider s)
    {
        red = s.value;
    }

    public void GetBlue(Slider s)
    {
        blue = s.value;
    }

    public void Update()
    {
    
        img.color = new Color(red, green, blue, 1);

        

        //Set the main Color of the Material to green
        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", new Color(red, green, blue, 1));


        //Find the Specular shader and change its Color to red
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", new Color(red, green, blue, 1));
       
    }
}
