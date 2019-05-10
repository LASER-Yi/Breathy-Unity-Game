using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using LGameStructure;
using LDataLoader;

namespace LDataLoader
{
    public class JsonLoader
    {
        [System.Serializable]
        private struct Container<T>
        {

            public T content;
        }
        public static T LoadJsonInResource<T>(string path)
        {
            TextAsset asset = Resources.Load<TextAsset>(path);
            Container<T> tempory = JsonUtility.FromJson<Container<T>>(asset.text);
            T items = tempory.content;
            return items;
        }

        public static T LoadJsonInPersist<T>(string path)
        {
            try
            {
                var raws = Encoding.UTF8.GetBytes(path);
                var data = Encoding.UTF8.GetString(raws);
                var container = JsonUtility.FromJson<Container<T>>(data);
                return container.content;
            }
            catch (IOException e)
            {
                throw e;
            }
        }

        public static void SaveJsonInPersist<T>(T data, string path)
        {
            var container = new Container<T>();
            container.content = data;
            var jsonContainer = JsonUtility.ToJson(container);
            if (jsonContainer != null)
            {
                var raws = Encoding.UTF8.GetBytes(jsonContainer);
                try
                {
                    File.WriteAllBytes(path, raws);
                }
                catch (IOException e)
                {
                    throw e;
                }
            }
        }

    }
}
public class DataManager
{
    private static System.Object _object = new Object();
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
    private string m_DataPath
    {
        get
        {
            return Application.persistentDataPath + "/save.brsd";
        }
    }

    DataManager()
    {
        //loadSaveFromFile();
        loadGameShopList();
    }

    // private void loadSaveFromFile(){
    //     var dirInfo = new DirectoryInfo(m_DataPath);
    //     if(dirInfo != null){
    //         try{

    //         }catch (System.Exception e){
    //             Debug.LogError(e);
    //             setupNewSave();
    //             return;
    //         }
    //     }else{
    //         setupNewSave();
    //         return;
    //     }
    // }

    // private bool saveToFile(){
    //     var originalData = new JsonContainer<Dictionary<int, SaveData>>();
    //     originalData.content = m_SaveData;
    //     var data = JsonUtility.ToJson(originalData);
    //     if(data != null){
    //         var raws = Encoding.UTF8.GetBytes(data);
    //         try{
    //             File.WriteAllBytes(m_DataPath, raws);
    //             return true;
    //         }catch(System.Exception e){
    //             Debug.LogError(e);
    //             return false;
    //         }
    //     }else{
    //         return false;
    //     }
    // }
    // private void setupNewSave(){
    //     m_SaveData = new Dictionary<int, SaveData>();
    //     saveToFile();
    // }

    private List<ShopItem> m_ShopItemList;

    public List<ShopItem> getShopItemsClone()
    {
        return m_ShopItemList;
    }

    private void loadGameShopList()
    {
        m_ShopItemList = JsonLoader.LoadJsonInResource<List<ShopItem>>("Json/shopItem");
        foreach (var item in m_ShopItemList)
        {
            if (item.prefabPath == null || item.prefabPath == "")
            {
                m_ShopItemList.Remove(item);
            }
        }
        Debug.Log("Load avaliable shop item, count: " + m_ShopItemList.Count);
    }

    // public SaveData getDataByIndex(){
    //     SaveData data;
    //     if (m_Score.TryGetValue(str,out data)){
    //         return data;
    //     }else{
    //         data.dayCount = 0;
    //         data.sceneIndex = GSceneController.ESceneIndex.Menu;
    //         setScoreByString(str, data);
    //         return data;
    //     }
    // }
}
