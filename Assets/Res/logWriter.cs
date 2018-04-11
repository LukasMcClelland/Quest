// taken from the unity support docs
// https://support.unity3d.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-

using UnityEngine;
using System.IO;


public static class HandleTextFile
{
    public static void WriteLog(string str, string nameoftextfile)
    {
        if (nameoftextfile == "Build")
        {
            return;
        }
        //"Assets/Resources/logFile.txt"
        string path = Application.dataPath + "/Res/" + nameoftextfile;
        Debug.Log(path);

        if (!(File.Exists(path)))
        {
            File.Create(path).Dispose();
        }
        Debug.Log(path);
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(str);
        writer.Close();

        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = (TextAsset)Resources.Load(nameoftextfile);

        //Print the text from the file
        //Debug.Log(asset.text);
    }
}