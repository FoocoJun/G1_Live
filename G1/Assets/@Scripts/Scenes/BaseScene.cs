using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

public abstract class BaseScene : InitBase {
    public EScene SceneType {get; protected set; } = EScene.Unknown;

    // 초기화 할 것들 초기화 (순번을 위해 start 미사용)
    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        // 최초 실행 시 이벤트 시스템이 없으면 UI가 먹통되므로 생성
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null) {
            GameObject go = new GameObject() { name = "@EventSystem" };
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        return true;
    }

    public abstract void Clear();
}
