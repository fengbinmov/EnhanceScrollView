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
    public float f_posX { get; private set; }
    private float f_processX;
    private int valueT;


    public void SetData(float _f_posX, EnhanceScrollView _scrollView) {
        f_posX = _f_posX;
        scrollView = _scrollView;
    }

    public void SetStatus(float ProcessXT,Vector2 anchoredPositionT,Vector3 localScaleT){

        f_processX = ProcessXT;
		rectTransf.anchoredPosition = anchoredPositionT;
		rectTransf.localScale = localScaleT;
    }


    public override void OnPointerClick(PointerEventData eventData)
    {
        if(!interactable||!b_CanClick || !scrollView.b_CanGetTo) return;

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

            // if (scrollView.b_IsDoubCount) {
            //     if (value == 0)
            //     {
            //         if (f_value > 0)
            //         {
            //             value += 1;
            //         }
            //     }
            // }

            scrollView.AutoToPageTurn(value, () => { base.OnPointerClick(eventData); });
        }
        else{
            base.OnPointerClick(eventData);
        }
    }

    //private Vector2 v2_StartPosition;
    //private Vector2 v2_EndPosition;

    private float v1_StartPosition;
    private float v1_EndPosition;

    private bool b_CanClick = true;


    private void OnDraging()
    {
        //v2_StartPosition = Input.mousePosition;
        v1_StartPosition = scrollView.horizontal ? Input.mousePosition.x : Input.mousePosition.y;
        float maxSize = scrollView.horizontal ? Screen.width : Screen.height;


        if (v1_StartPosition > maxSize)
            v1_StartPosition = maxSize;
        if(v1_StartPosition < 0)
            v1_StartPosition = 0;
        float processAdd = (v1_StartPosition - v1_EndPosition) * scrollView.f_Dragspeed / (scrollView.f_ItemWidth + scrollView.f_ItemSpace) * scrollView.f_ProMin * 2f;
        float dis = Mathf.Abs((v1_StartPosition - v1_EndPosition));

        if (dis >= scrollView.f_ProMax && b_CanClick) {
            b_CanClick = false;
        }
        if(!b_CanClick)
            scrollView.ToAddProcess(processAdd);

        v1_EndPosition = v1_StartPosition;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;

        base.OnPointerUp(eventData);
        CancelInvoke();
        if (!b_CanClick)
            scrollView.BackNormal();
    }


    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;

        base.OnPointerDown(eventData);
        b_CanClick = true;
        v1_EndPosition = v1_StartPosition = scrollView.horizontal ? Input.mousePosition.x : Input.mousePosition.y;
        InvokeRepeating("OnDraging", 0, Time.deltaTime);
    }
	
    protected override void OnDisable() {
        base.OnDisable();
        CancelInvoke();
    }
}
