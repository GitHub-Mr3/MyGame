
//#if UNITY_EDITOR
using UnityEditor;
using System.IO;
//#endif
using UnityEngine;
using System;

public class MyFrameTools
{

    /********************************************************************
               _ooOoo_
              o8888888o
              88" . "88
              (| -_- |)
              O\  =  /O
           ____/`---'\____
         .'  \\|     |//  `.
        /  \\|||  :  |||//  \
       /  _||||| -:- |||||-  \
       |   | \\\  -  /// |   |
       | \_|  ''\---/''  |   |
       \  .-\__  `-`  ___/-. /
     ___`. .'  /--.--\  `. . __
  ."" '<  `.___\_<|>_/___.'  >'"".
 | | :  `- \`.;`\ _ /`;.`/ - ` : | |
 \  \ `-.   \_ __\ /__ _/   .-` /  /
======`-.____`-.___\_____/___.-`____.-'======
               `=---='
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
佛祖保佑		永无BUG		永不修改

*********************************************************************/

    [MenuItem("MrTool/Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
    [MenuItem("MrTool/复制导出包名到剪切板")]
    private static void CopyText()
    {
        var str = "MyFrameWork_" + DateTime.Now.ToString("yyyy_MM_dd_hh");
        GUIUtility.systemCopyBuffer = str;
    }
    [MenuItem("MrTool/导出UnityPackage %e")]
    private static void ExportPackage()
    {
        var assetPathName = "Assets";//需要导出的目录
        var fileName = "MyFrameWork_" + DateTime.Now.ToString("yyyy_MM_dd_hh")+ ".unitypackage";
        AssetDatabase.ExportPackage(assetPathName, fileName, ExportPackageOptions.Recurse);
        Application.OpenURL("file:///" + Path.Combine(Application.dataPath, "../"));//打开 所在文件 夹
    }
    //[MenuItem("MrTool/快捷键 %e")]
    //private static void MenuClicked()
    //{
    //    Debug.LogError("快捷键");
    //    EditorApplication.ExecuteMenuItem("MrTool/导出UnityPackage");
    //}


}
