using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 잡다한 쓰로틀링 변수 관리
public class InitBase : MonoBehaviour
{
    protected bool _init = false;

    public virtual bool Init() {
        if(_init) {
            return false;
        }
        _init = true;
        return true;
    }
    
    private void Awake() {
        Init();
    }
}
