using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System.Text;
using System.Security.Cryptography;
using System;
using LitJson;
public class BackendInfo
{
    public int playerId;
    public int skinId;
    public string playerName;
    public string premiumCheck;
    public bool hasPremium
    {
       get
       {
        if (premiumCheck == "vip")
        {
            return true;
        }
        return false;
       }
    }
    public bool hasMegaPremium
    {
        get
       {
        if (premiumCheck == "superVip")
        {
            return true;
        }
        return false;
       }
    }
}
public class CdzProvider : MonoBehaviour
{
    public delegate void RequestCallBack(string answer);
    public string serverUrl
    {
        get{
        return "https://st-sntr-cub2reborn.ru/cubz/";
        }
    }
    public string phpScript
    {
       get
       {
        return "cdzMain.php";
       }
    }
    public void Request(Dictionary<string,object> param,int code,RequestCallBack cb)
    {
        StartCoroutine(PostREQUEST(param,code,cb));
    }
    public IEnumerator PostREQUEST(Dictionary<string,object> param,int code,RequestCallBack call)
    {
        WWWForm form = new WWWForm();
        form.AddField("Cmd",code);
        foreach(var getParam in param)
        {
            form.AddField(getParam.Key,getParam.Value.ToString());
        }
        WWW newWWW = new WWW(serverUrl + phpScript,form);
        yield return newWWW;
        if (call != null){
            call(newWWW.text);
        }
    }
    public void GetProfileInfo()
    {
        StartCoroutine(GetProfileInfoRequest());
    }
    public IEnumerator GetProfileInfoRequest()
    {
        string str = "";
	    string text = serverUrl + phpScript + "?";
	    string text2 = "Cmd=1";
	    string str1 = str + text2;
	    text += text2;
	    text2 = "UID=" + SystemInfo.deviceUniqueIdentifier;
	    string str2 = str1 + text2;
	    text = text + "&" + text2;
		text2 = "secretKey=" + GetMD5(SystemInfo.deviceUniqueIdentifier);
        string str3 = str2 + text2;
        text = text + "&" + text2;
        var hashMD5 = GetMD5(str3);
        text2 = "hashtable=" + hashMD5;
        text=  text + "&" + text2;
        WWW newWWW = new WWW(text);
        yield return newWWW;
        ProfileOnLoaded(newWWW.text);
    }
    private void ProfileOnLoaded(string text)
    {
        JsonData jsonGet = JsonMapper.ToObject(text);
        BackendInfo backend = new BackendInfo();
        backend.playerId = int.Parse(jsonGet["id"].ToString());
        backend.skinId = int.Parse(jsonGet["skinNum"].ToString());
        backend.playerName = jsonGet["nickName"].ToString();
        DataKeeper.backendInfo = backend;
        Debug.Log("Profile on loaded!");
    }
    public static string GetMD5(string input)
    {     
        byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            stringBuilder.Append(array[i].ToString("x2"));
        }
        return stringBuilder.ToString();
    }
	public static string DecodeFromBase(string array){
       return Encoding.UTF8.GetString(Convert.FromBase64String(array));
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        //GetProfileInfo();
    }

}
