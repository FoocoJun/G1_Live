using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static Define;

public class GameScene : BaseScene {
    public override bool Init()
    {
        if (base.Init() == false) {
            return false;
        }
        SceneType = Define.EScene.GameScene;

        GameObject map = Managers.Resource.Instantiate("BaseMap");
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";
        // TODO

        // 캐릭터 생성
        
        Vector3 heroTmpStartPosition = new Vector3Int(-10, -5, 0);
        Hero hero = Managers.Object.Spawn<Hero>(heroTmpStartPosition, HERO_KNIGHT_ID);
        

        {
            Vector3 monsterTmpStartPosition = new Vector3Int(0, 1, 0);
            Monster monster = Managers.Object.Spawn<Monster>(monsterTmpStartPosition, MONSTER_BEAR_ID);
            Monster monster2 = Managers.Object.Spawn<Monster>(monsterTmpStartPosition, MONSTER_SLIME_ID);
        }

        {
            Vector3 envTempStartPosition = new Vector3Int(0, 5, 0);
            Env env = Managers.Object.Spawn<Env>(envTempStartPosition, ENV_TREE1_ID);
            env.EnvState = EEnvState.Idle;
        }

        // 카메라 셋팅
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = hero;

        // 조이스틱 컨트롤러 생성
        Managers.UI.ShowBaseUI<UI_Joystick>();

        return true;
    }

    public override void Clear() {
        // TODO
    }
}
