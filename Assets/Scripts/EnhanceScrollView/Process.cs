using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Process : MonoBehaviour {

	public EnhanceScrollView scrollView;
	public Slider slider;

	void Start() {
		slider.maxValue = scrollView.context.childCount - 1;
	}

	public void Event_Change(){
		slider.value = scrollView.Value_Index;
	}
	public void Event_DropChange(){
		scrollView.Value_Index = (int)slider.value;
	}
}
