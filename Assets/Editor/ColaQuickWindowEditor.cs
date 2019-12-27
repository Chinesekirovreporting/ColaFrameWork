﻿//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using ColaFramework;
using System.Text;
using ColaFramework.Foundation;
using System.Text.RegularExpressions;

public class ColaQuickWindowEditor : EditorWindow
{
    /// <summary>
    /// Lua业务逻辑代码的路径
    /// </summary>
    private const string LuaLogicPath = "Assets/Lua";

    [MenuItem("ColaFramework/Open Quick Window %Q")]
    static void Popup()
    {
        ColaQuickWindowEditor window = EditorWindow.GetWindow<ColaQuickWindowEditor>();
        window.titleContent = new GUIContent("快捷工具窗");
        window.position = new Rect(400, 100, 640, 480);
        window.Show();
    }

    public void OnGUI()
    {
        DrawColaFrameworkUI();
        GUILayout.Space(20);
        DrawAssetBundleUI();
        GUILayout.Space(20);
        DrawMiscUI();
        GUILayout.Space(20);
        DrawAssetUI();
    }


    public void DrawColaFrameworkUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== UI相关辅助 ==");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建NewUIView", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaGUIEditor.CreateColaUIView();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建C#版UIView脚本", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            CreateScriptsEditor.CreateCSharpUIView();
        }
        if (GUILayout.Button("创建C#版Module脚本", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            CreateScriptsEditor.CreateCSharpModule();
        }
        if (GUILayout.Button("创建C#版Templates(UIView和Module)", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            CreateScriptsEditor.CreateCSharpModule();
        }
        GUILayout.EndHorizontal();
    }

    public void DrawAssetBundleUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== Assetbundle相关 ==");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打包Assetbundle（增量）", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        if (GUILayout.Button("重新打包Assetbundle（先删除再重打）", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("为所有资源设置Assetbundle name", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        if (GUILayout.Button("清除所有资源的Assetbundle name", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("AssetBundle Browser", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            //AssetBundleBrowser.AssetBundleBrowserMain.ShowWindow();
            this.Close();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawMiscUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== 快捷功能 ==");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开AssetPath目录", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaEditHelper.OpenDirectory(CommonHelper.AssetPath);
        }
        if (GUILayout.Button("打开GameLog文件目录", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaEditHelper.OpenDirectory(Path.Combine(CommonHelper.AssetPath, "logs"));
        }
        GUILayout.EndHorizontal();
    }

    private void DrawAssetUI()
    {
        GUILayout.BeginHorizontal("HelpBox");
        EditorGUILayout.LabelField("== 快捷功能 ==");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Build Lua To StreamingAsset", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            ColaEditHelper.BuildLuaToStreamingAsset();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Zip Lua", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            var result = ZipHelper.Zip("Assets/Lua", Path.Combine(Application.dataPath, "../output/luaout.zip"));
            Debug.Log("Zip Lua结果:" + result);
        }
        if (GUILayout.Button("UnZip Lua", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            var filePath = Path.Combine("Assets", "../output/luaout.zip");
            if (File.Exists(filePath))
            {
                var result = ZipHelper.UnZip(filePath, Path.Combine("Assets", "../output"));
                Debug.Log("UnZip Lua结果:" + result);
            }
            else
            {
                Debug.LogError("解压错误！要解压的文件不存在！路径:" + filePath);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("筛选出MD5码变化的lua文件", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(30)))
        {
            var md5Dic = new Dictionary<string, string>();
            var luaMd5FilePath = ColaEditHelper.TempCachePath + "/LuaMD5.txt";
            if (File.Exists(luaMd5FilePath))
            {
                using (var sm = new StreamReader(luaMd5FilePath, Encoding.UTF8))
                {
                    var fileLines = sm.ReadToEnd().Split('\n');
                    foreach (var item in fileLines)
                    {
                        var lineContent = item.Split('|');
                        if (lineContent.Length == 2)
                        {
                            md5Dic[lineContent[0]] = lineContent[1];
                        }
                        else
                        {
                            Debug.LogError("LuaMD5.txt格式错误！内容为: " + lineContent);
                        }
                    }
                }
            }

            var luaFiles = new List<string>(Directory.GetFiles(LuaLogicPath, "*.lua", SearchOption.AllDirectories));
            var fLength = (float)luaFiles.Count;

            for (int i = 0; i < luaFiles.Count; i++)
            {
                var fileName = luaFiles[i];
                string curMd5 = FileHelper.GetMD5Hash(fileName);
                if (md5Dic.ContainsKey(fileName) && curMd5 == md5Dic[fileName])
                {
                    continue;
                }
                string destPath = Regex.Replace(fileName, "^Assets", "..output");
                FileHelper.EnsureParentDirExist(destPath);
                File.Copy(fileName, destPath,true);
                md5Dic[fileName] = curMd5;
                EditorUtility.DisplayProgressBar("正在分析Lua差异化..", fileName, i / fLength);

            }
            EditorUtility.ClearProgressBar();
        }
        GUILayout.EndHorizontal();
    }
}
