using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Entry {
	public static Entry fromJSONObject(JSONObject obj) {
		var res = new Entry();
		res.Name = obj["Name"].str;
		res.Points = (int)obj["Points"].n;
		return res;
	}
	public string Name;
	public int Points;
}

public class HighscoreServer {
	private const string ADDRESS = "http://pux.asdf-systems.de:8080";
	private const string PASSWORD = "Iof+:*NLx^%~+zu?-,|";
	private const string GETHIGHSCORE_CALL = "{\"MethodName\": \"GetHighscore\", \"Parameters\": []}";

	private const string ADDENTRY_CALL = "{\"MethodName\": \"AddEntry\", \"Parameters\": [{0}, \""+PASSWORD+"\"]}";

	private delegate void RestCallback(string data, bool success);
	public delegate void HighscoreCallback(Entry[] highscore, bool success);

	private static byte[] StringToByteArray(string str) {
		System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
		return enc.GetBytes(str);
	}

	private static IEnumerator callREST(string data, RestCallback cb) {
		WWW www = new WWW(ADDRESS, StringToByteArray(data));
		yield return www;
		if(www.error == null) {
			cb(www.text, true);
		} else {
			cb(null, false);
		}
	}

	public static IEnumerator GetHighscore(HighscoreCallback cb) {
		return callREST(GETHIGHSCORE_CALL, (data, success) => {
				if(!success) {
					cb(null, false);
					return;
				}
				var results = new JSONObject(data);
				if(results == null || results.Count <= 0) {
					cb(null, false);
					return;
				}
				LinkedList<Entry> entry_list = new LinkedList<Entry>();
				for(int i = 0; i < results[0].Count; i++) {
					JSONObject entry = results[0][i];
					entry_list.AddLast(Entry.fromJSONObject(entry));
				}
				cb(Enumerable.ToArray<Entry>(entry_list), success);
			});
	}

	public static IEnumerator AddEntry(string name, int points) {
		var obj = JSONObject.obj;
		obj.AddField("Name", name);
		obj.AddField("Points", points);
		var call = ADDENTRY_CALL.Replace("{0}", obj.ToString());
		EditorDebug.Log("CALL: " + call);
		return callREST(call, (data, success) => {});
	}
}
