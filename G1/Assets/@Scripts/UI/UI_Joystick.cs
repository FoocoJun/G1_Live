using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public class UI_Joystick : UI_Base {
    enum GameObjects {
        JoystickBG,
        JoystickCursor,
    }

    private GameObject _background;
    private GameObject _cursor;
    private float _radius;
    private Vector2 _touchPos;

    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        BindObjects(typeof(GameObjects));

        _background = GetObject((int)GameObjects.JoystickBG);
        _cursor = GetObject((int)GameObjects.JoystickCursor);
        _radius = _background.GetComponent<RectTransform>().sizeDelta.y / 5;

        gameObject.BindEvent(OnPointerDown, type: Define.EUIEvent.PointerDown);
        gameObject.BindEvent(OnPointerUp, type: Define.EUIEvent.PointerUp);
        gameObject.BindEvent(OnDrag, type: Define.EUIEvent.Drag);

        return true;
    }

    #region Event
    public void OnPointerDown(PointerEventData eventData) {
        _background.transform.position = eventData.position;
        _cursor.transform.position = eventData.position;
        _touchPos = eventData.position;

        Managers.Game.JoystickState = EJoystickState.PointerDown;
    }

    public void OnPointerUp(PointerEventData eventData) {
        _cursor.transform.position = _touchPos;

        Managers.Game.MoveDir = Vector2.zero;
        Managers.Game.JoystickState = EJoystickState.PointerUp;
    }

    public void OnDrag(PointerEventData eventData) {
        Vector2 touchDir = eventData.position - _touchPos;

        // 커서의 최대 이탈범위는 반지름 (touchDir의 길이와 반지름 중 작은 것)
        float moveDist = Mathf.Min(touchDir.magnitude, _radius);
        // touchDir의 단위벡터 방향으로 이동
        Vector2 moveDir = touchDir.normalized;
        // 처음 커서의 위치에 이동한 방향 * 길이만큼 이동(그래도 radius 내에 존재)
        Vector2 newPosition = _touchPos + moveDir * moveDist;
        // 커서 재배치
        _cursor.transform.position = newPosition;

        // 변화가 생기면 게임 매니저에 전달하고 게임 매니저가 브로드캐스팅
        Managers.Game.MoveDir = moveDir;
        Managers.Game.JoystickState = EJoystickState.Drag;
    }
    #endregion
}
