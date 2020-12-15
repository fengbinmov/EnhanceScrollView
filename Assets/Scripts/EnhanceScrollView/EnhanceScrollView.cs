/*迭代版本：7.9
	开发者:彬 
	脚本解释：卷轴控件的核心控制器，
	通过XOff调整 a_itemDrag 的位置;
	通过YOff 调整 a_itemDrag在Y轴上的偏移;
	通过XYOff 调整 a_itemDrag在x&y轴上的缩放大小;
	通过autoDir调整 a_itemDrag 在整体卷轴中的内容进度，以内容步进的形式控制内容进度
	通过horizontal调整 a_itemDrag 的排列方向，‘向右’ 或 ‘向上’
*/
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine;

public class EnhanceScrollView : MonoBehaviour
{

	public RectTransform context;
	[SerializeField]
	private bool _horizontal = true;
	public bool horizontal { get { return _horizontal; } set { _horizontal = value; SetPositions(a_itemDrag); } }
	public AnimationCurve positionXOff;
	public AnimationCurve positionYOff;
	public AnimationCurve localScaleXYVaule;
	public bool b_Active = true;
	public bool b_ToIndex = true;
	public bool b_CanGetTo { get { return b_Active && !bIsRolling; } }

	private ItemDrag[] a_itemDrag = new ItemDrag[] { };
	public bool b_IsDoubCount { get { return a_itemDrag.Length % 2 == 0; } }
	private int i_ItemCount = 0;
	public float f_ItemWidth = 100;
	public float f_ItemSpace = 10;

	private float f_process = 0f;//pri
	private float f_targetProcess;//pri
	private float f_proMin, f_proMax;

	public delegate void TurnCallBack();
	public delegate void ChangeCallBack(float v);

	public float f_ProMin { get { return f_proMin; } }
	public float f_ProMax { get { return f_proMax; } }

	public float f_Rollspeed = 1;
	public float f_Dragspeed = 1;

	public int iOffDir = 0; //卷轴偏移方向（将自动归零，回归平衡状态)
	public int iOffDirLast = 0;

	public bool bIsRolling; //当前是否为滚动状态

	[SerializeField]
	private int _value = 0;
	public int Value
	{
		get { return _value; }
		set
		{
			if (bIsRolling) return;

			_value = value < 0 ? i_ItemCount + value : value;
			_value = _value % i_ItemCount;

			lastProcess = f_targetProcess = f_process = f_proMin + _value * f_proMin * 2;
			SetPositions(a_itemDrag);
		}
	}

	public int Value_Index
	{
		get
		{
			int half = i_ItemCount / 2 - (b_IsDoubCount ? 1 : 0);
			int v = (i_ItemCount - _value + half) % i_ItemCount;
			return v;
		}
		set
		{
			value = value < 0 ? i_ItemCount + value : value;
			value = value % i_ItemCount;

			int half = i_ItemCount / 2 - (b_IsDoubCount ? 1 : 0);
			int v = (i_ItemCount - value + half) % i_ItemCount;
			Value = v;
		}
	}

	private TurnCallBack onClick;
	public UnityEvent change;

	//自动翻动指定的矢量值的页数
	//在自动翻到指定的页数后，调用该Item按钮的时间
	public void AutoToPageTurn(int _turnDirection, TurnCallBack action = null)
	{
		if (!b_Active) return;

		iOffDir = _turnDirection;
		onClick = action;
	}

	public void ToAddProcess(float valueT)
	{
		if (!b_Active) return;

		f_targetProcess += valueT;

		if (f_targetProcess > f_proMax * 2)
		{
			f_targetProcess = f_targetProcess - f_proMax + f_proMin;
		}
	}

	public void BackNormal()
	{

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

	public void ChangeDirection()
	{

		horizontal = !horizontal;
	}

	void Awake()
	{
		InitData();
	}

	void LateUpdate()
	{

		if (!bIsRolling)
		{

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				iOffDir = -1;
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				iOffDir = 1;
			}
		}
		if (b_Active) UpdatePosition();
	}

	float lastProcess;
	void UpdatePosition()
	{

		if (!bIsRolling && iOffDir != 0)
		{
			if (iOffDir > 0)
			{
				for (int i = 0; i < Mathf.Abs(iOffDir); i++)
				{
					NextRightItem();
				}
			}
			else
			{
				for (int i = 0; i < Mathf.Abs(iOffDir); i++)
				{
					NextLeftItem();
				}
			}
			iOffDir = 0;
			//if (iOffDir > 0)
			//{
			//	--iOffDir;
			//	NextRightItem();
			//}
			//else
			//{
			//	++iOffDir;
			//	NextLeftItem();
			//}
		}

		if (f_process != f_targetProcess)
		{
			float speedT = Time.deltaTime * f_Rollspeed;

			if (f_process > f_targetProcess)
			{
				if ((f_process - speedT) > f_targetProcess)
					f_process -= speedT;
				else
					f_process = f_targetProcess;
			}
			else
			{
				if ((f_process + speedT) < f_targetProcess)
					f_process += speedT;
				else
					f_process = f_targetProcess;
			}

			if (f_process > f_proMax)
			{
				f_process = f_proMin;
				f_targetProcess = f_targetProcess - f_proMax + f_proMin;
			}
			if (f_process < f_proMin)
			{
				f_process = f_proMax - (f_proMin - f_process);
				f_targetProcess = f_proMax - (f_proMin - f_targetProcess);
			}
		}
		else
		{
			bIsRolling = false;
			if (iOffDir == 0)
			{

				if (onClick != null)
				{
					TurnCallBack t = onClick;
					onClick = null;
					t();
				}
			}
		}

		if (f_process != lastProcess)
		{
			bIsRolling = true;

			//_value = Mathf.RoundToInt((1 - (f_process - f_proMin) / 2f) * i_ItemCount) % i_ItemCount;
			_value = Mathf.RoundToInt((f_process - f_proMin) / 2f / f_proMin);
			SetPositions(a_itemDrag);

			if (change != null)
			{
				change.Invoke();
			}
		}

		lastProcess = f_process;
	}

	void SetPositions(ItemDrag[] a_itemDragT)
	{

		float contextWidth = (f_ItemWidth + f_ItemSpace) * i_ItemCount / 2f;
		float posXT;
		float localsXT;
		for (int i = 0; i < i_ItemCount; i++)
		{
			//float PosX = ((float)i / (i_ItemCount)) * 2f - 1f;
			posXT = (a_itemDragT[i].f_posX + f_process + 1f) % 2f - 1f;
			posXT = Mathf.RoundToInt(posXT * 100000f) / 100000f;
			localsXT = localScaleXYVaule.Evaluate(posXT);


			Vector2 anchored = new Vector2(contextWidth * posXT * positionXOff.Evaluate(posXT), contextWidth * positionYOff.Evaluate(posXT));
			if (!horizontal)
			{
				anchored = new Vector2(contextWidth * (1 - positionXOff.Evaluate(posXT)), contextWidth * posXT * (1 - positionYOff.Evaluate(posXT)));
			}
			a_itemDragT[i].SetStatus(posXT, anchored, new Vector2(localsXT, localsXT));
		}
	}

	public void InitData()
	{

		i_ItemCount = context.childCount;
		a_itemDrag = new ItemDrag[i_ItemCount];
		ItemDrag itemDragT = null;
		for (int i = 0; i < i_ItemCount; i++)
		{
			itemDragT = context.GetChild(i).GetComponent<ItemDrag>();
			float processT = Mathf.RoundToInt((((float)i / (i_ItemCount)) * 2f - 1f) * 100000f) / 100000f;
			itemDragT.SetData(processT, this);
			a_itemDrag[i] = itemDragT;
		}

		f_proMin = Normal(1f / i_ItemCount);
		f_proMax = 2 + f_proMin;
		f_targetProcess = f_process = Normal(f_proMin + _value * f_proMin * 2f);

		SetPositions(a_itemDrag);
	}

	public void OnClickLeft()
	{

		if (!bIsRolling)
			iOffDir = -1;
	}
	public void OnClickRight()
	{
		if (!bIsRolling)
			iOffDir = +1;
	}
	public void OnClickLeft(TurnCallBack action)
	{
		if (!bIsRolling)
		{

			OnClickLeft();
			onClick = action;
		}
	}
	public void OnClickRight(TurnCallBack action)
	{
		if (!bIsRolling)
		{

			OnClickRight();
			onClick = action;
		}
	}


	private void NextLeftItem()
	{

		f_targetProcess += f_proMin * 2f;
		f_targetProcess = Normal(f_targetProcess);
	}

	private void NextRightItem()
	{
		f_targetProcess -= f_proMin * 2f;
		f_targetProcess = Normal(f_targetProcess);
		//if (f_targetProcess <= 0)
		//{
		//	f_process = f_process == f_ProMin ? f_proMax : f_process;
		//	f_targetProcess = f_proMax - f_proMin * 2;
		//}

	}

	public float Normal(float v)
	{
		return Mathf.RoundToInt(v * 100000) / 100000f;
	}

#if UNITY_EDITOR

	private void OnValidate()
	{

		if (UnityEditor.EditorApplication.isPlaying) return;

		InitData();
	}

	private void OnDrawGizmos()
	{
		i_ItemCount = context.childCount;
		ItemDrag[] a_itemDragT = new ItemDrag[i_ItemCount];

		for (int i = 0; i < i_ItemCount; i++)
		{
			a_itemDragT[i] = context.GetChild(i).GetComponent<ItemDrag>();
		}
		Gizmos.color = Color.blue;


		float contextWidth = (f_ItemWidth + f_ItemSpace) * i_ItemCount / 2f;
		float PosX;
		float posXT;
		float localsXT;
		for (int i = 0; i < i_ItemCount; i++)
		{
			PosX = ((float)i / (i_ItemCount)) * 2f - 1f;
			posXT = (PosX + f_process + 1f) % 2f - 1f;
			localsXT = localScaleXYVaule.Evaluate(posXT) * f_ItemWidth;

			Vector2 anchored = new Vector2(contextWidth * posXT * positionXOff.Evaluate(posXT), contextWidth * positionYOff.Evaluate(posXT));
			if (!horizontal)
			{
				anchored = new Vector2(contextWidth * (1 - positionXOff.Evaluate(posXT)), contextWidth * posXT * (1 - positionYOff.Evaluate(posXT)));
			}
			Gizmos.DrawWireCube(context.TransformPoint(anchored), new Vector3(localsXT, localsXT, 0));
		}

		PosX = ((float)(i_ItemCount / 2 - (i_ItemCount % 2 == 0 ? 1 : 0)) / i_ItemCount) * 2f - 1f;
		posXT = (PosX + f_ProMin + 1f) % 2f - 1f;
		Vector2 anchored2 = new Vector2(contextWidth * posXT * positionXOff.Evaluate(posXT), contextWidth * positionYOff.Evaluate(posXT));
		if (!horizontal)
		{
			anchored2 = new Vector2(contextWidth * (1 - positionXOff.Evaluate(posXT)), contextWidth * posXT * (1 - positionYOff.Evaluate(posXT)));
		}
		Gizmos.color = Color.yellow;
		Vector3 size = new Vector3(f_ItemWidth / (horizontal ? 1 : 0.2f), f_ItemWidth / (horizontal ? 0.2f : 1f), 0);
		Gizmos.DrawWireCube(context.TransformPoint(anchored2), size);
	}
#endif

	//Test Event
	public void OnClick(GameObject obj)
	{

		Debug.Log("OnClickItem --result[" + obj.name + "]");
	}
}
