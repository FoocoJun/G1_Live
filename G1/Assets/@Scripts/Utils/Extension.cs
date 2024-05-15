using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Extension {
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component {
        return Util.GetOrAddComponent<T>(go);
    }

    // go에 this BindEvent를 추가해주는 방식
    public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, Define.EUIEvent type = Define.EUIEvent.Click) {
        UI_Base.BindEvent(go, action, type);
    }

    public static bool IsValid(this GameObject go) {
        return go != null && go.activeSelf;
    }

    // 오브젝트 풀링 시 단순 null 체크로는 먹통이 될 수 있음.
    public static bool IsValid(this BaseObject bo) {
        if (bo == null || bo.isActiveAndEnabled == false) {
            return false;
        }

        Creature creature = bo as Creature;
        if (creature != null) {
            return creature.CreatureState != Define.ECreatureState.Dead;
        }

        return true;
    }

    public static void DestroyChilds(this GameObject go) {
        foreach (Transform child in go.transform) {
            Managers.Resource.Destroy(child.gameObject);
        }
    }

    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;

        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n +1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
