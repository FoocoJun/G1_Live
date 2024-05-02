using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data {
    #region TestData
    [Serializable]
    public class TestData {
        // data의 한 row에 대하여 정의 (데이터 원본이 어떻게 생겨먹었는지에 대한 정의)
        public int Level;
        public int Exp;
        public List<int> Skills;
        public float Speed;
        public string Name;
    }

    [Serializable]
    public class TestDataLoader : ILoader<int, TestData> {
        public List<TestData> tests = new List<TestData>();
        public Dictionary<int, TestData> MakeDict() {
            Dictionary<int, TestData> dict = new Dictionary<int, TestData>();
            foreach (TestData testData in tests) {
                dict.Add(testData.Level, testData);
            }
            return dict;
        }
    }
    #endregion
}
