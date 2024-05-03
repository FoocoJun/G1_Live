using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Scene
{
    // 0. 해당 씬에 속하는 각 타입에 따라 이넘 생성
    enum GameObjects {
        StartImage
    }

    enum Texts {
        DisplayText
    }

    public override bool Init()
    {
        if (base.Init() == false) {
            return false;
        }

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

        InitStartImage();
        InitDisplayText();

        StartLoadAssets();

        return true;
    }

    void InitStartImage() {
        // 호출 시 이넘으로 호출 가능
        GetObject((int)GameObjects.StartImage).BindEvent((evt) => {
            Debug.Log("Change Scene");
            Managers.Scene.LoadScene(Define.EScene.GameScene);
        });

        GetObject((int)GameObjects.StartImage).gameObject.SetActive(false);
    }

    void InitDisplayText() {
        GetText((int)Texts.DisplayText).text = $"";
    }

    void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) => {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount) {
                // Data 불러오기
                Managers.Data.Init();
                // Managers.Sound.Init();

                GetObject((int)GameObjects.StartImage).gameObject.SetActive(true);
                GetText((int)Texts.DisplayText).text = $"Touch To Start";

                Debug.Log($"{Managers.Data.TestDic[1].Name}");
            }
        });
    }
}
