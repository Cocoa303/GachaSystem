using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;
using System.Reflection;

#if UNITY_EDITOR
namespace UnityEditor
{
    public class DataLoader : EditorWindow
    {
        //== OnGUI Reference
        private GUILayoutOption[] buttonOption = new[] { GUILayout.Width(128), GUILayout.Height(32) };
        private TextAsset csvFile;

        //== Scriptable Object Reference
        private string savePath = "Assets/Scripts/Data";
        private string dataPath = "Assets/Resources/Data";
        private (string handle, string type)[] typeRules =
        {
            ("n_", "int"),
            ("s_", "string"),
            ("f_", "float"),
            ("l_", "long"),
            ("b_", "bool"),
            ("e_", "enum")
        };

        [MenuItem("Tool/Data Packing")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(DataLoader));
            window.minSize = new Vector2(500, 500);

        }

        public void OnGUI()
        {
            #region CSV File Reference
            EditorGUILayout.LabelField(" ▼ CSV 파일을 삽입해 주세요.");
            csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File : ", csvFile, typeof(TextAsset), false);

            if (csvFile == null) EditorGUILayout.LabelField("파일이 존재하지 않습니다.");
            else
            {
                if (!(csvFile is TextAsset)) EditorGUILayout.LabelField("해당 파일은 텍스트 파일이 아닙니다.");
                else
                {
                    //== 확장자 검사
                    string assetPath = AssetDatabase.GetAssetPath(csvFile);
                    string extension = System.IO.Path.GetExtension(assetPath);

                    if (extension.CompareTo(".csv") != 0)
                    {
                        EditorGUILayout.LabelField(" ※ 해당 파일은 CSV 파일이 아닙니다.");
                        EditorGUILayout.LabelField(" ※ 확장자를 확인해주시기 바랍니다.");
                    }
                    else
                    {
                        EditorGUILayout.Space(20);
                        if (GUILayout.Button("클래스 생성 및 초기화", buttonOption))
                        {
                            Create();
                        }
                        EditorGUILayout.Space(20);
                        EditorGUILayout.LabelField(" ▼ Infomation  ▼");
                        EditorGUILayout.Space(20);

                        ShowInfomation();
                    }
                }
            }
            #endregion
        }

        private (string[,] data, int row, int col) CSVLoad()
        {
            string[] lines = csvFile.text.Split('\n');
            int row = lines.Length;
            int col = lines[0].Split(',').Length;

            string[,] csvData = new string[row, col];

            //== Init
            for (int i = 0; i < row; i++)
            {
                string[] rowDatas = lines[i].Split(',');

                if (rowDatas.Length != col) continue;

                for (int j = 0; j < col; j++)
                {
                    csvData[i, j] = rowDatas[j];
                }
            }

            return (csvData, row, col);
        }

        private void Create()
        {
            StringBuilder csFile = new StringBuilder(string.Empty);

            csFile.AppendLine("using UnityEngine;\n");
            csFile.AppendLine("namespace Data");
            csFile.AppendLine("{");
            csFile.AppendLine($"\t[CreateAssetMenu(fileName = \"Data\", menuName = \"Data/{csvFile.name}\")]");
            csFile.AppendLine($"\tpublic class {csvFile.name} : ScriptableObject");
            csFile.AppendLine("\t{");
            csFile.AppendLine(MakeInfomation(2));
            csFile.AppendLine("\t}");
            csFile.AppendLine("}");

            //== CS 폴더 검증
            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            //== CS 파일 생성
            System.IO.File.WriteAllText($"{savePath}/{csvFile.name}.cs", csFile.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh();

            //== 데이터 폴더 검증 및 재생성
            if (!System.IO.Directory.Exists(dataPath))
            {
                System.IO.Directory.CreateDirectory(dataPath);
            }
            string scriptablePath = dataPath + "/" + csvFile.name;
            if (System.IO.Directory.Exists(scriptablePath))
            {
                var prevData = AssetDatabase.LoadAllAssetsAtPath(scriptablePath);
                for (int i = 0; i < prevData.Length; i++)
                {
                    System.IO.File.Delete(scriptablePath + "/" + prevData[i].ToString());
                }
            }
            System.IO.Directory.CreateDirectory(scriptablePath);

            var csv = CSVLoad();
            List<string> variables = ListPool<string>.Get();
            for (int col = 0; col < csv.col; col++)
            {
                variables.Add(GetVariableName(csv.data[0, col]));
            }

            Assembly assembly = Assembly.Load("Assembly-CSharp");
            System.Type type = assembly.GetType($"Data.{csvFile.name}");
            for (int row = 2; row < csv.row; row++)
            {
                if (csv.data[row, 0] == null || csv.data[row, 0].CompareTo(string.Empty) == 0) continue;

                ScriptableObject asset = ScriptableObject.CreateInstance(type);

                for (int col = 0; col < csv.col; col++)
                {
                    SetProperty(type, asset, variables[col], csv.data[row, col]);
                }

                asset.name = csv.data[row, 0];
                AssetDatabase.CreateAsset(asset, scriptablePath + "/" + asset.name + ".asset");
            }
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            AssetDatabase.Refresh();

            ListPool<string>.Release(variables);

            void SetProperty(System.Type type, ScriptableObject obj, string fieldName, object value)
            {
                var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

                if (field != null)
                {
                    if (field.FieldType.IsEnum)
                    {
                        object enumValue = System.Enum.Parse(field.FieldType, value.ToString());
                        field.SetValue(obj, enumValue);
                    }
                    else
                    {
                        field.SetValue(obj, System.Convert.ChangeType(value, field.FieldType));
                    }
                }
                else
                {
                    Debug.LogWarning($"Property {fieldName} not found or cannot be written.");
                }
            }
        }

        private void ShowInfomation()
        {
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.label);
            textAreaStyle.wordWrap = true;

            EditorGUILayout.LabelField(MakeInfomation(), textAreaStyle);
        }
        private string MakeInfomation(int tapCount = 0)
        {
            if (csvFile != null)
            {
                StringBuilder result = new StringBuilder(string.Empty);

                string[] dataLines = csvFile.text.Split('\n');
                string variables = dataLines[0].Trim();
                if (string.IsNullOrEmpty(variables)) return string.Empty;

                string[] variable = variables.Split(',');

                //== 변수명 및 열거형 저장
                List<int> enumIndex = ListPool<int>.Get();
                for (int i = 0; i < variable.Length; i++)
                {
                    string type = GetVariableType(variable[i]);
                    if (type == string.Empty) continue;

                    if (type == "enum")
                    {
                        enumIndex.Add(i);
                        result.AppendLine(($"{Tap(tapCount)}public {GetVariableName(variable[i], false)} {GetVariableName(variable[i])};"));
                    }
                    else
                    {
                        result.AppendLine(($"{Tap(tapCount)}public {type} {GetVariableName(variable[i])};"));
                    }
                }

                if (enumIndex.Count > 0)
                {
                    var csv = CSVLoad();
                    for (int i = 0; i < enumIndex.Count; i++)
                    {
                        HashSet<string> names = HashSetPool<string>.Get();

                        //== 변수명 / 설명 구간 제외
                        for (int j = 2; j < csv.row; j++)
                        {
                            names.Add(csv.data[j, enumIndex[i]]);
                        }

                        string enumDefinition = GetEnumDefinition(GetVariableName(csv.data[0, enumIndex[i]], false), names, tapCount);
                        result.AppendLine(enumDefinition);

                        HashSetPool<string>.Release(names);
                    }
                }

                ListPool<int>.Release(enumIndex);

                return result.ToString();
            }
            return string.Empty;
        }

        private string GetVariableType(string data)
        {
            for (int i = 0; i < typeRules.Length; i++)
            {
                if (data.Contains(typeRules[i].handle))
                {
                    return typeRules[i].type;
                }
            }

            return string.Empty;
        }
        private string GetVariableName(string data, bool toLower = true)
        {
            for (int i = 0; i < typeRules.Length; i++)
            {
                if (data.Contains(typeRules[i].handle))
                {
                    string result = data.Replace(typeRules[i].handle, string.Empty);
                    if (toLower)
                    {
                        return char.ToLower(result[0]) + result.Substring(1).Trim();
                    }
                    else
                    {
                        return result.Trim();
                    }
                }
            }

            return string.Empty;
        }
        private string GetEnumDefinition(string name, in HashSet<string> datas, int tapCount = 1)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{Tap(tapCount)}public enum {name}\n");
            builder.Append($"{Tap(tapCount)}{{\n");

            foreach (string data in datas)
            {
                if (data == null || data.CompareTo(string.Empty) == 0) continue;
                builder.Append(Tap(tapCount + 1));
                builder.Append(data);
                builder.Append(",\n");
            }

            //== 마지막 ',\n' 제거
            builder.Remove(builder.Length - 2, 2);
            builder.Append($"\n{Tap(tapCount)}}}");

            return builder.ToString();
        }

        private string Tap(int tapCount = 0)
        {
            StringBuilder tap = new StringBuilder();

            for (int i = 0; i < tapCount; i++)
            {
                tap.Append('\t');
            }

            return tap.ToString();
        }
    }
}
#endif