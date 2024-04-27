using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Popup은 타 UI와 다르게 순서 등 핸들링 필요
public class UI_Popup : UI_Base {
    public override bool Init() {
        if (base.Init() == false) {
            return false;
        }
        
        Managers.UI.SetCanvas(gameObject, true);
        return true;
    }

    public virtual void ClosePopupUI() {
        Managers.UI.ClosePopupUI(this);
    }
}

