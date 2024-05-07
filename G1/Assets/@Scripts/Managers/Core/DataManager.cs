using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// 유니크한 Key의 타입을 아직 알 수 없으니 제네릭으로 제공
public interface ILoader<Key, Value> {
    Dictionary<Key, Value> MakeDict();
}
public class DataManager
{
    public Dictionary<int, Data.CreatureData> CreatureDic { get; private set; } = new Dictionary<int, Data.CreatureData>();
    public Dictionary<int, Data.EnvData> EnvDic { get; private set; } = new Dictionary<int, Data.EnvData>();

    public void Init() {
        CreatureDic = LoadJson<Data.CreatureDataLoader, int, Data.CreatureData>("CreatureData").MakeDict();
        EnvDic = LoadJson<Data.EnvDataLoader, int, Data.EnvData>("EnvData").MakeDict();
    }

    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value> {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        // Json 직렬화에 뉴턴소프트제이슨 사용하기
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}

// Init 후 Managers.Data.TestDic에서 key값을 이용하여 사용 할 수 있게 됨.
