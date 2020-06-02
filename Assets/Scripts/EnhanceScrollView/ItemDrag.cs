using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemDrag : Button
{

    public RectTransform rectTransf { get {

            if (_rectTransf == null)
                _rectTransf = GetComponent<RectTransform>();
            return _rectTransf;
        } }
    private RectTransform _rectTransf;
    private EnhanceScrollView scrollView;
    private float f_posX;
    public float f_processX;

    public float PosX{get{return f_posX;}}
    public float ProcessX{get{return f_processX;}set { f_processX = value; } }


    public void SetData(float _f_posX, EnhanceScrollView _scrollView) {
        f_posX = _f_posX;
        scrollView = _scrollView;
    }

    public void SetStatus(float ProcessXT,Vector2 anchoredPositionT,Vector3 localScaleT){

		ProcessX = ProcessXT;
		rectTransf.anchoredPosition = anchoredPositionT;
		rectTransf.localScale = localScaleT;
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        if(!b_CanClick || !scrollView.b_CanGetTo) return;

        if(scrollView.b_ToIndex){
            float f_value =0;
            int value = 0;

            if (scrollView.b_IsDoubCount)
            {
                f_value = (f_processX + scrollView.f_ProMin) / (scrollView.f_ProMin * 2);
            }
            else {
                f_value = f_processX / (scrollView.f_ProMin * 2);
            }
            value = Mathf.RoundToInt(f_value);

            if (scrollView.b_IsDoubCount) {
                if (value == 0)
                {
                    if (f_value > 0)
                    {
                        value += 1;
                    }
                }
            }

            scrollView.AutoToPageTurn(value, () => { base.OnPointerClick(eventData); });
        }
        else{
            base.OnPointerClick(eventData);
        }
    }

    private Vector2 v2_StartPosition;
    private Vector2 v2_EndPosition;
    private bool b_CanClick = true;


    private void OnDraging()
    {
        v2_StartPosition = Input.mousePosition;

        if(v2_StartPosition.x > Screen.width)
            v2_StartPosition.x = Screen.width;
        if(v2_StartPosition.x < 0)
            v2_StartPosition.x = 0;
        float processAdd = (v2_StartPosition.x - v2_EndPosition.x) * scrollView.f_Dragspeed / (scrollView.f_ItemWidth + scrollView.f_ItemSpace) * scrollView.f_ProMin * 2f;
        float dis = Mathf.Abs((v2_StartPosition.x - v2_EndPosition.x));

        if (dis >= scrollView.f_ProMax && b_CanClick) {
            b_CanClick = false;
        }
        if(!b_CanClick)
            scrollView.ToAddProcess(processAdd);

        v2_EndPosition = v2_StartPosition;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        CancelInvoke();
        if (!b_CanClick)
            scrollView.BackNormal();
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        b_CanClick = true;
        v2_EndPosition = v2_StartPosition = Input.mousePosition;
        InvokeRepeating("OnDraging", 0, Time.deltaTime);
    }

}
