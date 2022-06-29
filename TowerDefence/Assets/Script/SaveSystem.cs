using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static void SaveLevelProgression(LevelProgession enemyList)
    {
        string SaveFilePath = Application.persistentDataPath + "/Progress.json";
        string json = JsonUtility.ToJson(enemyList);
        Debug.Log(json);
        File.WriteAllText(SaveFilePath, json);
    }
    public static LevelProgession LoadLevelProgression()
    {
        LevelProgession list;
        string SaveFilePath = Application.persistentDataPath + "/Progress.json";
        string json = File.ReadAllText(SaveFilePath);
        list = JsonUtility.FromJson<LevelProgession>(json);
        return list;
    }

    #region dont use
    public static void Savedata(Data dat)
    {
        string SaveFilePath = Application.persistentDataPath + "/SaveFile.json";
        string json = JsonUtility.ToJson(dat);
        Debug.Log(json);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log(SaveFilePath); //For quick saving lol,never+ delete
        /*
        FileStream dataStream;
        Aes iAes = Aes.Create();

        byte[] savedKey = iAes.Key;

        string key = System.Convert.ToBase64String(savedKey);
        PlayerPrefs.SetString("DKey", key);

        dataStream = new FileStream(Application.persistentDataPath + "/SaveFile.json", FileMode.Create);
        byte[] inputIV = iAes.IV;
        dataStream.Write(inputIV, 0, inputIV.Length);

        CryptoStream iStream = new CryptoStream(
            dataStream,
            iAes.CreateEncryptor(iAes.Key, iAes.IV)
            , CryptoStreamMode.Write);
        StreamWriter sWriter = new StreamWriter(iStream);
        sWriter.Write(json);

        //Close shit

        sWriter.Close();
        iStream.Close();
        dataStream.Close();*/
    }
    public static void SaveEquipment(Equipment eq)
    {
        string SaveFilePath = Application.persistentDataPath + "/Equipment.json";
        string json = JsonUtility.ToJson(eq);
        Debug.Log(json);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log(SaveFilePath); //For quick saving lol,dont delete

        /*FileStream dataStream;
        Aes iAes = Aes.Create();

        byte[] savedKey = iAes.Key;

        string key = System.Convert.ToBase64String(savedKey);
        PlayerPrefs.SetString("EKey", key);

        dataStream = new FileStream(Application.persistentDataPath + "/SaveFile.json", FileMode.Create);
        byte[] inputIV = iAes.IV;
        dataStream.Write(inputIV, 0, inputIV.Length);

        CryptoStream iStream = new CryptoStream(
            dataStream,
            iAes.CreateEncryptor(iAes.Key, iAes.IV)
            , CryptoStreamMode.Write);
        StreamWriter sWriter = new StreamWriter(iStream);
        sWriter.Write(json);

        //Close shit

        sWriter.Close();
        iStream.Close();
        dataStream.Close();*/
    }
    public static void SaveLevelProgression(LevelUnlocked lv)
    {
        string SaveFilePath = Application.persistentDataPath + "/Level.json";
        string json = JsonUtility.ToJson(lv);
        Debug.Log(json);
        File.WriteAllText(SaveFilePath, json);
        Debug.Log(SaveFilePath); //For quick saving lol,dont delete
        /*FileStream dataStream;
        Aes iAes = Aes.Create();

        byte[] savedKey = iAes.Key;

        string key = System.Convert.ToBase64String(savedKey);
        PlayerPrefs.SetString("LVKey", key);

        dataStream = new FileStream(Application.persistentDataPath + "/SaveFile.json", FileMode.Create);
        byte[] inputIV = iAes.IV;
        dataStream.Write(inputIV, 0, inputIV.Length);

        CryptoStream iStream = new CryptoStream(
            dataStream,
            iAes.CreateEncryptor(iAes.Key, iAes.IV)
            , CryptoStreamMode.Write);
        StreamWriter sWriter = new StreamWriter(iStream);
        sWriter.Write(json);

        //Close shit

        sWriter.Close();
        iStream.Close();
        dataStream.Close();*/
    }
    public static Data LoadData()
    {
        //string SaveFilePath = Application.persistentDataPath + "/SaveFile.json";
        /* if (File.Exists(SaveFile) && PlayerPrefs.HasKey("Key"))
         {
             byte[] savedKey = System.Convert.FromBase64String(PlayerPrefs.GetString("Key"));
             FileStream dataStream = new FileStream(SaveFile, FileMode.Open);

             Aes oAes = Aes.Create();
             byte[] outPutIV = new byte[oAes.IV.Length];

             dataStream.Read(outPutIV, 0, outPutIV.Length);

             CryptoStream oStream = new CryptoStream(
                 dataStream,
                 oAes.CreateEncryptor(savedKey, outPutIV),
                 CryptoStreamMode.Read);

             StreamReader reader = new StreamReader(oStream);
             string txt = reader.ReadToEnd();

             Data dat = JsonUtility.FromJson<Data>(txt);
             return dat;
         }*/
        //string json = File.ReadAllText(SaveFilePath);
        //Data dat = JsonUtility.FromJson<Data>(json);
        //return dat;
        return null;
    }
    public static Equipment LoadEquipment()
    {
        string SaveFilePath = Application.persistentDataPath + "/Equipment.json";
        /*if (File.Exists(SaveFilePath) && PlayerPrefs.HasKey("Key"))
        {
            byte[] savedKey = System.Convert.FromBase64String(PlayerPrefs.GetString("Key"));
            FileStream dataStream = new FileStream(SaveFilePath, FileMode.Open);

            Aes oAes = Aes.Create();
            byte[] outPutIV = new byte[oAes.IV.Length];

            dataStream.Read(outPutIV, 0, outPutIV.Length);

            CryptoStream oStream = new CryptoStream(
                dataStream,
                oAes.CreateEncryptor(savedKey, outPutIV),
                CryptoStreamMode.Read);

            StreamReader reader = new StreamReader(oStream);
            string txt = reader.ReadToEnd();

            Equipment eq  = JsonUtility.FromJson<Equipment>(txt);
            return eq;
        }*/
        string json = File.ReadAllText(SaveFilePath);
        Equipment equipment = JsonUtility.FromJson<Equipment>(json);
        return equipment;//null;
    }
    public static LevelUnlocked LoadLevel()
    {
        string SaveFilePath = Application.persistentDataPath + "/Level.json";
        /* if (File.Exists(SaveFile) && PlayerPrefs.HasKey("Key"))
         {
             byte[] savedKey = System.Convert.FromBase64String(PlayerPrefs.GetString("Key"));
             FileStream dataStream = new FileStream(SaveFile, FileMode.Open);

             Aes oAes = Aes.Create();
             byte[] outPutIV = new byte[oAes.IV.Length];

             dataStream.Read(outPutIV, 0, outPutIV.Length);

             CryptoStream oStream = new CryptoStream(
                 dataStream,
                 oAes.CreateEncryptor(savedKey, outPutIV),
                 CryptoStreamMode.Read);

             StreamReader reader = new StreamReader(oStream);
             string txt = reader.ReadToEnd();

             Data dat = JsonUtility.FromJson<Data>(txt);
             return dat;
         }*/
        string json = File.ReadAllText(SaveFilePath);
        LevelUnlocked dat = JsonUtility.FromJson<LevelUnlocked>(json);
        return dat;
    }
}
#endregion
[System.Serializable]
public class LevelProgession
{
    public List<string> enemyList;
    public List<string> level;
    public List<int> star;
    public LevelProgession()
    {
        enemyList = new List<string>();
        level = new List<string>();
        star = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
    public LevelProgession(List<string> l)
    {
        enemyList = l;
        level = new List<string>();
        star = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
    public LevelProgession(List<string> l1, List<string> l2)
    {
        enemyList = l1;
        level = l2;
        star = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
    public void SetEnemyList(Dictionary<string, string> d)
    {
        enemyList.Clear();
        foreach (KeyValuePair<string, string> item in d)
        {
            enemyList.Add(item.Value);
        }
    }
    public LevelProgession(Dictionary<string, string> d1, Dictionary<string, string> d2)
    {
        enemyList = new List<string>();
        foreach (KeyValuePair<string, string> item in d1)
        {
            enemyList.Add(item.Value);
        }
        level = new List<string>();
        foreach (KeyValuePair<string, string> item in d2)
        {
            level.Add(item.Value);
        }
        star = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
}
[System.Serializable]
public class Equipment
{
    public List<int> dictKey;
    public List<string> _equipment;

    public Equipment(Dictionary<int, string> d)
    {
        foreach (var item in d)
        {
            dictKey.Add(item.Key);
            _equipment.Add(item.Value);
        }
    }
    public Equipment()
    {
        dictKey = new List<int>();
        _equipment = new List<string>();
    }
    public void AddEquipment(int k, string v)
    {
        dictKey.Add(k);
        _equipment.Add(v);
    }
    public void SetEquipment(List<int> a, List<string> b)
    {
        dictKey = a;
        _equipment = b;
    }
    public void SetEquipment(Dictionary<int, string> d)
    {
        foreach (var item in d)
        {
            dictKey.Add(item.Key);
            _equipment.Add(item.Value);
        }
    }
    public Dictionary<int, string> GetEquipment()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();
        for (int i = 0; i < dictKey.Count; i++)
        {
            dict.Add(dictKey[i], _equipment[i]);
        }
        return dict;
    }
}
public class LevelUnlocked
{
    public List<string> levelUnlocked;
    public List<int> dictKey;

    public LevelUnlocked(Dictionary<string, int> d)
    {
        foreach (var item in d)
        {
            levelUnlocked.Add(item.Key);
            dictKey.Add(item.Value);
        }
    }
    public LevelUnlocked()
    {
        levelUnlocked = new List<string>();
        dictKey = new List<int>();
    }
    public void AddLevel(string a, int b)
    {
        levelUnlocked.Add(a);
        dictKey.Add(b);

    }
    public void AddLevel(Dictionary<string, int> d)
    {
        foreach (var item in d)
        {
            levelUnlocked.Add(item.Key);
            dictKey.Add(item.Value);

        }
    }
    public void AddLevel(List<string> a, List<int> b)
    {
        if (dictKey.Count == 0)
        {
            levelUnlocked = a;
            dictKey = b;
        }
        else
        {
            for (int i = 0; i < a.Count; i++)
            {
                levelUnlocked.Add(a[i]);
                dictKey.Add(b[i]);
            }
        }
    }
    public void SetLevel(Dictionary<string, int> d)
    {
        foreach (var item in d)
        {
            levelUnlocked.Add(item.Key);
            dictKey.Add(item.Value);
        }
    }
    public Dictionary<string, int> GetLevelList()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        for (int i = 0; i < dictKey.Count; i++)
        {
            dict.Add(levelUnlocked[i], dictKey[i]);
        }
        return dict;
    }
}
[System.Serializable]
public class Data
{
    private int _money;
    public int money
    {
        get { return _money; }
        set
        {
            _money = value;
        }
    }
    public List<int> dictKey;
    public List<string> unlockedCharacter;

    public Data()
    {
        _money = 0;
        dictKey = new List<int>();
        unlockedCharacter = new List<string>();
    }
    public Data(int a)
    {
        _money = a;
        dictKey = new List<int>();
        unlockedCharacter = new List<string>();
    }
    public Data(Data player)
    {
        _money = player._money;
        dictKey = player.dictKey;
        unlockedCharacter = player.unlockedCharacter;
    }
    public void newCharacter(int key, string name)
    {
        if (!dictKey.Contains(key) || !unlockedCharacter.Contains(name))
        {
            dictKey.Add(key);
            unlockedCharacter.Add(name);
        }
    }
    public void newCharacter(int key1, string name1, int key2, string name2)
    {
        if (!dictKey.Contains(key1) || !unlockedCharacter.Contains(name1)) { dictKey.Add(key1); unlockedCharacter.Add(name1); }
        if (!dictKey.Contains(key2) || !unlockedCharacter.Contains(name2)) { dictKey.Add(key2); unlockedCharacter.Add(name2); }

        else { Debug.Log("it was there all along.You fucked up it seems"); }
    }
    public void newCharacter(Dictionary<int, string> name)
    {
        foreach (var item in name)
        {
            if (!dictKey.Contains(item.Key) || !unlockedCharacter.Contains(item.Value))
            {
                dictKey.Add(item.Key);
                unlockedCharacter.Add(item.Value);
            }
            else
            {
                Debug.Log("it was there all along " + item.Key + " " + item.Value);
            }
        }
    }
    public void MOREMONET(int m)
    {
        _money += m;
    }
    public void SetMoney(int m)
    {
        _money = m;
    }
    public void SetUnlockCharacter(Dictionary<int, string> l)
    {
        foreach (var item in l)
        {
            dictKey.Add(item.Key);
            unlockedCharacter.Add(item.Value);
        }
    }
    public int GetMoney()
    {
        return _money;
    }
    public Dictionary<int, string> GetUnlockedCharacters()
    {
        Dictionary<int, string> dict = new Dictionary<int, string>();
        for (int i = 0; i < dictKey.Count; i++)
        {
            dict.Add(dictKey[i], unlockedCharacter[i]);
        }
        return dict;
    }
}