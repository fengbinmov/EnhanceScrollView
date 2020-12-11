using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetVlaue : MonoBehaviour {

	public Slider slider;
	public Text text;

	public void U() {
		text.text = slider.value.ToString();
	}
}
