using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers s_instance;
    private static Managers Instance { get { Init(); return s_instance; }}

    // 게임 생성 시 @Managers 라는 게임 오브젝트 생성 및 컴포넌트 할당
    public static void Init() {
        if (s_instance == null) {
            GameObject go = GameObject.Find("@Managers");
            if (go == null) {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            // reset
            s_instance = go.GetComponent<Managers>();
        }
    }
}
