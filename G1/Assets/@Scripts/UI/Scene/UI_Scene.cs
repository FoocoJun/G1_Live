using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scene에 고정되는 주요 UI들 핸들링
public class UI_Scene : UI_Base {
    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }

        Managers.UI.SetCanvas(gameObject, false);

        return true;
    }
}
