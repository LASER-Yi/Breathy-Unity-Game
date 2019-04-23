using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public class DataManager
{
    private static UnityEngine.Object _object;
    private static DataManager _instance;
    // from: https://blog.csdn.net/yupu56/article/details/53668688
    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_object)
                {
                    if (_instance == null)
                    {
                        _instance = new DataManager();
                    }
                }
            }
            return _instance;
        }
    }
    
    [System.Serializable]
    public struct SaveData{
        public int timestamp;
        public int score;
    }

    [System.Serializable]
    private struct JsonContainer<T>{
        public T content;
    }
    private string m_DataPath{
        get{
            return Application.persistentDataPath + "/save.brsd";
        }
    }
    private Dictionary<string, SaveData> m_Score;

    DataManager(){
        loadDataFromFile();
    }

    private void loadDataFromFile(){
        var dirInfo = new DirectoryInfo(m_DataPath);
        if(dirInfo != null){
            try{
                var raws = Encoding.UTF8.GetBytes(m_DataPath);
                var data = Encoding.UTF8.GetString(raws);
                var container = JsonUtility.FromJson<JsonContainer<Dictionary<string, SaveData>>>(data);
                m_Score = container.content;
            }catch (System.Exception e){
                Debug.LogError(e);
                setupNewData();
                return;
            }
        }else{
            setupNewData();
            return;
        }
    }

    private bool saveDataToFile(){
        var originalData = new JsonContainer<Dictionary<string, SaveData>>();
        originalData.content = m_Score;
        var data = JsonUtility.ToJson(originalData);
        if(data != null){
            var raws = Encoding.UTF8.GetBytes(data);
            try{
                File.WriteAllBytes(m_DataPath, raws);
                return true;
            }catch(System.Exception e){
                Debug.LogError(e);
                return false;
            }
        }else{
            return false;
        }
    }
    private void setupNewData(){
        m_Score = new Dictionary<string, SaveData>();
        saveDataToFile();
    }

    public SaveData getDataByString(string str){
        SaveData data;
        if (m_Score.TryGetValue(str,out data)){
            return data;
        }else{
            data.score = 0;
            data.timestamp = 0;
            setScoreByString(str, data);
            return data;
        }
    }

    public void setScoreByString(string str, SaveData data){
        m_Score.Add(str, data);
    }
}
