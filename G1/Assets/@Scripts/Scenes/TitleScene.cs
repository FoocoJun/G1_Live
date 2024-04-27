using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene {
    // 부모가 초기화 할 것 다 하고 나머지 씬 별 설정
    public override bool Init()
    {
        if (base.Init() == false) {
            return false;
        }
        SceneType = Define.EScene.TitleScene;

        return true;
    }

    public override void Clear() {
        // TODO
    }
}
