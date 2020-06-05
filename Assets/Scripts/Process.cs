using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Process : MonoBehaviour {

	public EnhanceScrollView scrollView;
	public Slider slider;


	public void Event_Change(){
		slider.value = (scrollView.Value + 3)%(scrollView.context.childCount);
	}
	public void Event_DropChange(){
		scrollView.Value = (int)slider.value;
	}
}
