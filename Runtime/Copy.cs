using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace com.gb.statemachine_toolkit
{
    public class Copy : MonoBehaviour
    {
        [Tooltip("The file name where to load the copy from. It can be created with Google Sheets or Excel. It is required it to be saved as a tsv. The file extension doesn't matter, can be tsv or txt." +
            "The first cell is the ID, and the next cells are the texts linked with that ID. Any number of text cells can be used.")]
        public string fileName = "copy.txt";
        public enum FileNameType { NAME, ABSOLUTE }
        public FileNameType fileNameType;

        public bool Initialised { get => _initialised; }

        private static Copy _instance;
        private static Dictionary<string, List<string>> _copy = new Dictionary<string, List<string>>();
        private static bool _initialised = false;

        public static Copy Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<Copy>();
                    if (_instance)
                    {
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }

        public List<string> GetCopy(string id)
        {
            return _copy.ContainsKey(id) ? _copy[id] : null;
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                if (this != _instance)
                {
                    Destroy(this.gameObject);
                }
            }
        }

        IEnumerator Start()
        {
            yield return Read(fileName);
        }

        IEnumerator Read(string path)
        {
            string filePath = "";
            if (fileNameType.Equals(FileNameType.NAME))
                filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, path);
            else
                filePath = path;
#if UNITY_WEBGL
        yield return StartCoroutine(LoadTextFile(filePath, (textContent) => ReadTextAsset(textContent)));
#else
            string textAsset = "";
            if (File.Exists(filePath))
            {
                Debug.Log($"Reading copy at {filePath}");
                textAsset = File.ReadAllText(filePath);
            }
            else
            {
                Debug.Log($"Copy not found at {filePath}, loading Copy from Resources folder");
                textAsset = LoadFromResources(fileName);
            }
            ReadTextAsset(textAsset);
            yield return null;
#endif
            Debug.Log("Copy initialised!");
            _initialised = true;
        }

        private void ReadTextAsset(string textAsset)
        {
            if (string.IsNullOrWhiteSpace(textAsset))
            {
                Debug.LogError("No copy to load, textAsset is empty!");
                return;
            }
            Debug.Log("Reading Text Asset");
            var lines = textAsset.Split(new[] { '\r', '\n' });
            var lindx = 0;
            foreach (var line in lines)
            {
                var cells = line.Split(new char[] { '\t' });
                var indx = 0;
                var curId = "";
                foreach (var cell in cells)
                {
                    // Debug.Log($"line[{lindx}] cell[{indx}]: " + cell);
                    //  skip first line: ID, COPY, ...
                    if (lindx == 0) break;
                    // then read the cells, the first is the id
                    if (indx == 0)
                    {
                        // skip if id is empty
                        if (string.IsNullOrWhiteSpace(cell)) continue;
                        if (_copy.ContainsKey(cell))
                        {
                            // duplicate id?
                            Debug.LogWarning("duplicate id? content will be skipped");
                            //skip
                            continue;
                        }
                        //Debug.Log($"adding a new copy id: {cell}");
                        curId = cell;
                        _copy.Add(cell, new List<string>());

                    }
                    // the others are the strings to add to the List
                    else
                    {
                        // skip if string is empty
                        if (string.IsNullOrWhiteSpace(cell)) continue;
                        if (string.IsNullOrWhiteSpace(curId)) continue;
                        //Debug.Log($"Adding cell: {cell} to copy[{curId}]");
                        _copy[curId].Add(cell);
                    }
                    indx++;
                }
                lindx++;
            }
        }

        private string LoadFromResources(string path)
        {
            TextAsset configResources = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(path));
            if (configResources == null)
            {
                Debug.LogError($"Cannot load file {path} from Resources");
            }
            return configResources.text;
        }

        IEnumerator LoadTextFile(string uri, Action<string> onComplete)
        {
            Debug.Log($"Loading Text File at uri: {uri}");
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogWarning($"Loading from {uri} failed, Error: " + webRequest.error);
                        // in the case of errors, fallback to loading from Resources
                        Debug.Log($"Loading {fileName} from Resources as Fallback");
                        onComplete(LoadFromResources(fileName));
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Test Asset loaded successfully.");
                        onComplete(webRequest.downloadHandler.text);
                        break;
                }
            }
        }
    }
}