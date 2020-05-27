﻿/*迭代版本：7.3
	开发者:彬 
	脚本解释：卷轴控件的核心控制器，
	通过XOff调整 a_itemDrag 的位置;
	通过YOff 调整 a_itemDrag在Y轴上的偏移;
	通过XYOff 调整 a_itemDrag在x&y轴上的缩放大小;
	通过autoDir调整 a_itemDrag 在整体卷轴中的内容进度，以内容步进的形式控制内容进度
*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhanceScrollView : MonoBehaviour {

	public Transform context;
	public AnimationCurve positionXOff;
	public AnimationCurve positionYOff;
	public AnimationCurve localScaleXYVaule;
	public bool b_Active = true;
	public bool b_CanGetTo{get{return b_Active && !bIsRolling;}}


	private ItemDrag[] a_itemDrag = new ItemDrag[]{};
	public bool b_IsDoubCount { get { return a_itemDrag.Length % 2 == 0; } }
	private int i_ItemCount = 0;
	public float f_ItemWidth = 100;
	public float f_ItemSpace = 10;

	public float f_process = 0f;//pri
	public float f_targetProcess;//pri
	public float f_proMin,f_proMax;

	public Action onClick;

	public float f_ProMin { get { return f_proMin; } }
	public float f_ProMax { get { return f_proMax; } }

	public float f_Rollspeed = 1;

	private int iOffDir = 0;	//卷轴偏移方向（将自动归零，回归平衡状态)

	public bool bIsRolling;	//当前是否为滚动状态

	public void ToIndex(int _value, Action action = null) {

		if (!b_Active) return;

		iOffDir = _value;
		onClick = action;
	}
	public void ToAddProcess(float _value)
	{
		if (!b_Active) return;

		f_targetProcess += _value;
		
		if (f_targetProcess > f_proMax * 2)
		{
			f_targetProcess = f_targetProcess - f_proMax + f_proMin;
		}
	}

	public void BackNormal() {

		if (!b_Active) return;

		int n = Mathf.RoundToInt(f_targetProcess / f_proMin);
		n = n % 2 == 0 ? n + 1 : n;
		float targetV = n * f_proMin;

		if (Mathf.Abs(targetV - f_ProMax) < 0.01f)
		{
			f_targetProcess = f_ProMax;
		}
		else
		{
			f_targetProcess = targetV;
		}
	}


	void Start () {

		Input.multiTouchEnabled = false;

		InitData();

		UpdatePosition();
	}

	void LateUpdate()
	{		

		if (!bIsRolling) {

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				iOffDir = -1;
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				iOffDir = 1;
			}
		}
		if(b_Active) UpdatePosition();
	}

	float lastProcess;
	void UpdatePosition() {
		
		if (f_process != f_targetProcess)
		{
			float speedT = Time.deltaTime * f_Rollspeed;

			if (f_process > f_targetProcess)
			{
				if ((f_process - Time.deltaTime * f_Rollspeed) > f_targetProcess)
					f_process -= Time.deltaTime * f_Rollspeed;
				else
					f_process = f_targetProcess;
			}
			else
			{
				if ((f_process + Time.deltaTime * f_Rollspeed) < f_targetProcess)
					f_process += Time.deltaTime * f_Rollspeed;
				else
					f_process = f_targetProcess;
			}
		}
		else
		{
			if (iOffDir != 0)
			{
				if (iOffDir > 0)
				{
					iOffDir -= 1;
					NextRightItem();
				}
				else
				{
					iOffDir += 1;
					NextLeftItem();
				}
			}
			else{
				
				bIsRolling = false;
				if (onClick != null) {
					Action t = onClick;
					onClick = null;
					t();
				}
			}
		}
		if (f_process > f_proMax)
		{
			f_process = f_proMin;
			f_targetProcess = f_targetProcess - f_proMax + f_proMin;
		}
		if (f_process < f_proMin)
		{
			f_process = f_proMax;
			f_targetProcess = f_proMax;
		}

		if(f_process != lastProcess){

			bIsRolling = true;
        	float contextWidth = (f_ItemWidth + f_ItemSpace) * i_ItemCount;
			float posXT;
			float localsXT;
			for (int i = 0; i < i_ItemCount; i++)
			{
				posXT = (a_itemDrag[i].PosX + f_process + 1f) % 2f - 1f;
				localsXT = localScaleXYVaule.Evaluate(posXT);

				a_itemDrag[i].SetStatus(posXT,
				new Vector2(contextWidth * posXT * positionXOff.Evaluate(posXT), positionYOff.Evaluate(posXT)),
				new Vector2(localsXT, localsXT));
			}
		}

		lastProcess = f_process;
	}

	void InitData() {

		i_ItemCount = context.childCount;
		a_itemDrag = new ItemDrag[i_ItemCount];
		ItemDrag itemDragT = null;
		for (int i = 0; i < i_ItemCount; i++)
		{
			itemDragT = context.GetChild(i).GetComponent<ItemDrag>();
			itemDragT.SetData(((float)i / (i_ItemCount)) * 2f - 1f, this);
			a_itemDrag[i] = itemDragT;
		}

		f_proMin = 1f / i_ItemCount;
		f_proMax = 2 + f_proMin;
		f_targetProcess = f_process = f_proMin;
	}

	public void OnClickLeft() {

		iOffDir = -1;
	}
	public void OnClickRight()
	{
		iOffDir = +1;
	}

	private void NextLeftItem() {

		f_targetProcess += f_proMin * 2f;
	}

	private void NextRightItem()
	{
		if (f_targetProcess == f_proMin)
		{
			f_targetProcess = f_process = f_proMax;
		}

		f_targetProcess -= f_proMin * 2f;
	}

	#if UNITY_EDITOR
    private void OnValidate() {

		if(UnityEditor.EditorApplication.isPlaying) return;

		i_ItemCount = context.childCount;
		ItemDrag[] a_itemDragT = new ItemDrag[i_ItemCount];
		
		for (int i = 0; i < i_ItemCount; i++)
		{
			a_itemDragT[i] = context.GetChild(i).GetComponent<ItemDrag>();
		}

		f_proMin = 1f / i_ItemCount;
		f_proMax = 2 + f_proMin;
		f_targetProcess = f_process = f_proMin;

        float contextWidth = (f_ItemWidth + f_ItemSpace) * i_ItemCount;
		float posXT;
		float localsXT;
		for (int i = 0; i < i_ItemCount; i++)
		{
			float PosX = ((float)i / (i_ItemCount)) * 2f - 1f;
			posXT = (PosX + f_process + 1f) % 2f - 1f;
			localsXT = localScaleXYVaule.Evaluate(posXT);

			a_itemDragT[i].SetStatus(posXT,
			new Vector2(contextWidth * posXT * positionXOff.Evaluate(posXT), positionYOff.Evaluate(posXT)),
			new Vector2(localsXT, localsXT));
		}
    }

	#endif

	//Test Event
	public void OnClick(int i){
		
        Debug.Log("OnClickItem --result["+i+"]");
	}

	private void OnDrawGizmos() {

		
	}
}
