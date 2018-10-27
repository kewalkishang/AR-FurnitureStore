using UnityEngine;

public class ExampleColorReceiver : MonoBehaviour {
	
    Color color;
    public GameObject Model;

	void OnColorChange(HSBColor color) 
	{
        this.color = color.ToColor();
        Debug.Log("color changes" + this.color);

        Renderer rend = Model.GetComponent<Renderer>();

        //Set the main Color of the Material to green
        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", this.color);
       

        //Find the Specular shader and change its Color to red
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", this.color);


    }

    void OnGUI()
    {
		var r = Camera.main.pixelRect;
		var rect = new Rect(r.center.x + r.height / 6 + 50, r.center.y, 100, 100);
		GUI.Label (rect, "#" + ToHex(color.r) + ToHex(color.g) + ToHex(color.b));	
    }

	string ToHex(float n)
	{
		return ((int)(n * 255)).ToString("X").PadLeft(2, '0');
	}
}
