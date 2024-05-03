using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using Data;
using System;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

public class DataTransformer : EditorWindow {
    // Tools라는 탭에서 ParseExcel이라는 기능 단축키는 컨트롤(커멘드)+쉬프트+K
    #if UNITY_EDITOR
    [MenuItem("Tools/ParseExcel %#K")]
    public static void ParseExcelDataToJson() {
        ParseExcelDataToJson<TestDataLoader, TestData>("Test");

        Debug.Log("DataTransformer Completed");
    }

    #region Helpers
	private static void ParseExcelDataToJson<Loader, LoaderData>(string filename) where Loader : new() where LoaderData : new()
	{
		Loader loader = new Loader();
		FieldInfo field = loader.GetType().GetFields()[0];
		field.SetValue(loader, ParseExcelDataToList<LoaderData>(filename));

		string jsonStr = JsonConvert.SerializeObject(loader, Formatting.Indented);
		File.WriteAllText($"{Application.dataPath}/@Resources/Data/JsonData/{filename}Data.json", jsonStr);
		AssetDatabase.Refresh();
	}

	private static List<LoaderData> ParseExcelDataToList<LoaderData>(string filename) where LoaderData : new() {
        // 불러올 데이터를 위한 리스트 형성
		List<LoaderData> loaderDatas = new List<LoaderData>();

        // 불러온 데이터를 엔터로 구분하여 담아낸다. 이때 위치 중요할 듯. (LF와 CRLF 차이는?)
		string[] lines = File.ReadAllText($"{Application.dataPath}/@Resources/Data/ExcelData/{filename}Data.csv").Split("\n");

		for (int l = 1; l < lines.Length; l++) {
            // 여기서 해소 \r 모두 제거 및 ,로 csv 셀 구분 진행
			string[] row = lines[l].Replace("\r", "").Split(',');
			if (row.Length == 0) {
				continue;
            }
			if (string.IsNullOrEmpty(row[0])) {
				continue;
            }

            // 한 row 담기위한 상수 선언
			LoaderData loaderData = new LoaderData();

            // Data에서 각 필드들을 호출해온다.
            // 미리 알고있는 타입이 아닌 런타임 중 값으로부터 타입을 추출할 수 있는 리플랙션 기능
			System.Reflection.FieldInfo[] fields = typeof(LoaderData).GetFields();
			for (int f = 0; f < fields.Length; f++) {
				FieldInfo field = loaderData.GetType().GetField(fields[f].Name);
				Type type = field.FieldType;

                // 타입이 제네릭 타입인 경우 즉 List<int> 등 리스트인 경우 
				if (type.IsGenericType) {
					object value = ConvertList(row[f], type);
					field.SetValue(loaderData, value);
				} else {
					object value = ConvertValue(row[f], field.FieldType);
					field.SetValue(loaderData, value);
				}
			}

			loaderDatas.Add(loaderData);
		}

		return loaderDatas;
	}

	private static object ConvertValue(string value, Type type) {
		if (string.IsNullOrEmpty(value)) {
			return null;
        }

		TypeConverter converter = TypeDescriptor.GetConverter(type);
		return converter.ConvertFromString(value);
	}

	private static object ConvertList(string value, Type type) {
		if (string.IsNullOrEmpty(value)) {
			return null;
        }

		// Reflection
        // 0번째 값을 추출해서 값의 타입을 알아오고 
		Type valueType = type.GetGenericArguments()[0];
        // 타입으로 List<알아온타입> 제네릭리스트타입을 정의하고
		Type genericListType = typeof(List<>).MakeGenericType(valueType);
        // IList로서 기본적인 리스트 메서드를 정의하되 제네릭리스트타입의 인스턴스를 생성
		var genericList = Activator.CreateInstance(genericListType) as IList;

		// Parse Excel 이때 '&'으로 구분중이니 통일 필요
		var list = value.Split('&').Select(x => ConvertValue(x, valueType)).ToList();

		foreach (var item in list) {
			genericList.Add(item);
        }

		return genericList;
	}
	#endregion

#endif
}
