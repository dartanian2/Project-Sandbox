using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColourOptions : MonoBehaviour {

    public Button rbutton;
    public Button lbutton;
    public Text choice;
    public List<string> options = new List<string> { "20 HZ", "30 HZ" };

    public void rbuttonPress()
    {
        choice.text = options[1];
    }

    public void lbuttonPress()
    {
        choice.text = options[0];
    }
}
