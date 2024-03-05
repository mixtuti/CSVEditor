using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class CSVEditor : EditorWindow
{
    private string csvFilePath;
    private List<List<string>> csvData = new List<List<string>>();
    private Vector2 scrollPosition = Vector2.zero;

    private int numRows = 0;
    private int numColumns = 0;

    [MenuItem("Window/CSV Editor")]
    public static void ShowWindow()
    {
        CSVEditor window = EditorWindow.GetWindow<CSVEditor>("CSV Editor");
        window.minSize = new Vector2(400, 300); // ウィンドウの最小サイズを設定
    }

    private void OnGUI()
    {
        GUILayout.Label("CSV Editor", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("CSV File:", GUILayout.Width(70));
        csvFilePath = EditorGUILayout.TextField(csvFilePath);

        if (GUILayout.Button("Browse"))
        {
            csvFilePath = EditorUtility.OpenFilePanel("Open CSV File", "", "csv");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create CSV File"))
        {
            CreateCSV();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Rows:", GUILayout.Width(70));
        numRows = EditorGUILayout.IntField(numRows);

        if (GUILayout.Button("+"))
        {
            AddRow();
        }

        if (GUILayout.Button("-"))
        {
            RemoveRow();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Columns:", GUILayout.Width(70));
        numColumns = EditorGUILayout.IntField(numColumns);

        if (GUILayout.Button("+"))
        {
            AddColumn();
        }

        if (GUILayout.Button("-"))
        {
            RemoveColumn();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Load/Reload CSV"))
        {
            LoadCSV();
        }

        if (csvData.Count > 0)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < numRows; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < numColumns; j++)
                {
                    if (i < csvData.Count && j < csvData[i].Count)
                        csvData[i][j] = EditorGUILayout.TextField(csvData[i][j]);
                    else
                        EditorGUILayout.TextField("");
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Save CSV"))
        {
            SaveCSV();
        }
    }

    private void CreateCSV()
    {
        csvFilePath = EditorUtility.SaveFilePanel("Save CSV File", "", "NewCSVFile", "csv");
        csvData.Clear();
        for (int i = 0; i < numRows; i++)
        {
            List<string> newRow = new List<string>();
            for (int j = 0; j < numColumns; j++)
            {
                newRow.Add("");
            }
            csvData.Add(newRow);
        }
    }

    private void LoadCSV()
    {
        csvData.Clear();
        if (string.IsNullOrEmpty(csvFilePath) || !File.Exists(csvFilePath))
        {
            Debug.LogError("CSVファイルのパスが無効です。");
            return;
        }

        using (StreamReader reader = new StreamReader(csvFilePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] cells = line.Split(',');
                List<string> row = new List<string>(cells);
                csvData.Add(row);
            }
        }

        numRows = csvData.Count;
        numColumns = csvData.Count > 0 ? csvData[0].Count : 0;
    }

    private void AddRow()
    {
        List<string> newRow = new List<string>();
        for (int i = 0; i < numColumns; i++)
        {
            newRow.Add("");
        }
        csvData.Add(newRow);
        numRows++;
    }

    private void RemoveRow()
    {
        if (numRows > 0)
        {
            csvData.RemoveAt(numRows - 1);
            numRows--;
        }
    }

    private void AddColumn()
    {
        for (int i = 0; i < numRows; i++)
        {
            csvData[i].Add("");
        }
        numColumns++;
    }

    private void RemoveColumn()
    {
        if (numColumns > 0)
        {
            for (int i = 0; i < numRows; i++)
            {
                csvData[i].RemoveAt(numColumns - 1);
            }
            numColumns--;
        }
    }

    private void SaveCSV()
    {
        if (string.IsNullOrEmpty(csvFilePath))
        {
            Debug.LogError("CSVファイルのパスが無効です。");
            return;
        }

        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            foreach (var row in csvData)
            {
                writer.WriteLine(string.Join(",", row.ToArray()));
            }
        }

        Debug.Log("CSVファイルが保存されました: " + csvFilePath);
    }
}
