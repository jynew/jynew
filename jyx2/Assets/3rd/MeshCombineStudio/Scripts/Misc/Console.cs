using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshCombineStudio
{
    public class Console : MonoBehaviour
    {
        static public Console instance;
        public KeyCode consoleKey = KeyCode.F1;
        public bool logActive = true;
        public bool showConsole;
        public bool showOnError;
        public bool combineAutomatic = true;
        bool showLast = true;
        bool setFocus;

        GameObject selectGO;

        public List<LogEntry> logs = new List<LogEntry>();

        Rect window, inputRect = new Rect(), logRect = new Rect(), vScrollRect = new Rect();
        string inputText;
        float scrollPos = 0;
        int lines = 20;
        bool showUnityLog = true, showInputLog = true;
        MeshCombiner[] meshCombiners;
        MeshCombiner selectedMeshCombiner;
        
        void Awake()
        {
            instance = this;
            FindMeshCombiners();

            window = new Rect();
            inputText = string.Empty;

            ReportStartup();
        }

        void ReportStartup()
        {
            Log("---------------------------------");
            Log("*** MeshCombineStudio Console ***");
            Log("---------------------------------");
            Log("");
            ReportMeshCombiners(false);
            Log("combine automatic " + (combineAutomatic ? "on" : "off"));

            if (meshCombiners != null && meshCombiners.Length > 0) SelectMeshCombiner(meshCombiners[0].name);
            Log("");
            Log("Type '?' to show commands");
            
            // ExecuteCommand("?");
        }

        void FindMeshCombiners()
        {
            meshCombiners = GameObject.FindObjectsOfType<MeshCombiner>();
        }

        void ReportMeshCombiners(bool reportSelected = true)
        {
            for (int i = 0; i < meshCombiners.Length; i++) ReportMeshCombiner(meshCombiners[i], true);
            if (selectedMeshCombiner != null)
            {
                Log("Selected MCS -> " + selectedMeshCombiner.name);
            } 
        }

        void ReportMeshCombiner(MeshCombiner meshCombiner, bool foundText = false)
        {
            Log((foundText ? "Found MCS -> " : "") + meshCombiner.name + " (" + (meshCombiner.combined ? "*color-green#Combined" : "*color-blue#Uncombined" ) + ")" + " -> Cell Size " + meshCombiner.cellSize + (meshCombiner.searchOptions.useMaxBoundsFactor ? " | Max Bounds Factor " + meshCombiner.searchOptions.maxBoundsFactor : "")
                    + (meshCombiner.searchOptions.useVertexInputLimit ? " | Vertex Input Limit " + (meshCombiner.searchOptions.useVertexInputLimit ? meshCombiner.searchOptions.vertexInputLimit : 65534) : ""),
                    0, null, meshCombiner);
        }

        public int SelectMeshCombiner(string name)
        {
            if (meshCombiners == null && meshCombiners.Length == 0) return 0;

            for (int i = 0; i < meshCombiners.Length; i++)
            {
                MeshCombiner meshCombiner = meshCombiners[i];
                if (meshCombiner.name == name)
                {
                    Log("Selected MCS -> " + meshCombiner.name + " (" + (meshCombiner.combined ? "*color-green#Combined" : "*color-blue#Uncombined") + ")", 0, null, meshCombiner); 
                    selectedMeshCombiner = meshCombiner; return 2;
                }
            }
            return 0;
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        static public void Log(string logString, int commandType = 0, GameObject go = null, MeshCombiner meshCombiner = null)
        {
            instance.logs.Add(new LogEntry(logString, "", LogType.Log, false, commandType, go, meshCombiner));
        }

        void HandleLog(string logString, string stackTrace, LogType logType)
        {
            if (logActive)
            {
                logs.Add(new LogEntry(logString, stackTrace, logType, true));
                if (showOnError && (logType == LogType.Error || logType == LogType.Warning))
                {
                    SetConsoleActive(true);
                    showLast = true;
                    showUnityLog = true;
                }
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(consoleKey))
            {
                SetConsoleActive(!showConsole);
            }
        }

        void SetConsoleActive(bool active)
        {
            showConsole = active;
            if (showConsole)
            {
                setFocus = true;
            }
        }
        
        void ExecuteCommand(string cmd)
        {
            logs.Add(new LogEntry(cmd, "", LogType.Log, false, 1));
            LogEntry log = logs[logs.Count - 1];

            if (cmd == "?")
            {
                Log("'F1' to show/hide console");
                Log("'dir', 'dirAll', 'dirSort', 'cd', 'show', 'showAll', 'hide', 'hideAll'");
                Log("'components', 'lines', 'clear', 'gc collect'");
                Log("'select (MeshCombineStudio name)', ");
                Log("'report MeshCombineStudio'");
                Log("'combine', 'uncombine', 'combine automatic on/off'");
                Log("'max bounds factor (float)/off'");
                Log("'vertex input limit (float)/off'");
                Log("'vertex input limit lod (float)/off'");
                Log("'cell size (float)'");
                log.commandType = 2;
            }
            else if (cmd == "gc collect")
            {
                System.GC.Collect();
                log.commandType = 2;
            }
            else if (cmd == "dir") { Dir(); log.commandType = 2; }
            else if (cmd == "components") { Components(log); }
            else if (cmd.Contains("lines "))
            {
                int.TryParse(cmd.Replace("lines ", ""), out lines);
                lines = Mathf.Clamp(lines, 5, 50);
                log.commandType = 2;
            }
            else if (cmd == "cd..") { CD(log, ".."); }
            else if (cmd == "cd\\") { CD(log, "\\"); }
            else if (cmd.Contains("cd ")) { CD(log, cmd.Replace("cd ", "")); }
            else if (cmd.Contains("show "))
            {
                Transform t = Methods.Find<Transform>(selectGO, cmd.Replace("show ", ""));
                if (t != null) { t.gameObject.SetActive(true); log.commandType = 2; }
            }
            else if (cmd == "show")
            {
                if (selectGO != null) { selectGO.SetActive(true); log.commandType = 2; }
            }
            else if (cmd.Contains("showAll "))
            {
                SetActiveContains(cmd.Replace("showAll ", ""), true);
                log.commandType = 2;
            }
            else if (cmd.Contains("hide "))
            {
                GameObject go = GameObject.Find(cmd.Replace("hide ", ""));
                if (go != null) { go.SetActive(false); log.commandType = 2; }
            }
            else if (cmd.Contains("hideAll "))
            {
                SetActiveContains(cmd.Replace("hideAll ", ""), false);
                log.commandType = 2;
            }
            else if (cmd == "hide")
            {
                if (selectGO != null) { selectGO.SetActive(false); log.commandType = 2; }
            }
            else if (cmd.Contains("clear")) { Clear(log, cmd.Replace("clear ", "")); }
            else if (cmd.Contains("dir ")) { DirContains(cmd.Replace("dir ", "")); log.commandType = 2; }
            else if (cmd == "dirAll") { DirAll(); log.commandType = 2; }
            else if (cmd.Contains("dirSort ")) { DirSort(cmd.Replace("dirSort ", "")); log.commandType = 2; }
            else if (cmd == "dirSort") { DirSort(); log.commandType = 2; }
            else if (cmd.Contains("cell size "))
            {
                int cellSize;
                int.TryParse(cmd.Replace("cell size ", ""), out cellSize);
                if (cellSize < 4)
                {
                    Log("cell size should be bigger than 4");
                    return;
                }
                if (selectedMeshCombiner != null)
                {
                    selectedMeshCombiner.cellSize = cellSize;
                    selectedMeshCombiner.AddObjectsAutomatically();
                    if (combineAutomatic) selectedMeshCombiner.CombineAll();
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
            else if (cmd == "report MeshCombineStudio")
            {
                ReportMeshCombiners();
                log.commandType = 2;
            }
            else if (cmd == "combine")
            {
                if (selectedMeshCombiner != null)
                {
                    selectedMeshCombiner.octreeContainsObjects = false;
                    selectedMeshCombiner.CombineAll();
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
            else if (cmd == "uncombine")
            {
                if (selectedMeshCombiner != null)
                {
                    selectedMeshCombiner.DestroyCombinedObjects();
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
            else if (cmd == "combine automatic off")
            {
                combineAutomatic = false;
                log.commandType = 2;
            }
            else if (cmd == "combine automatic on")
            {
                combineAutomatic = true;
                log.commandType = 2;
            }
            else if (cmd.Contains("select "))
            {
                if (SelectMeshCombiner(cmd.Replace("select ", "")) == 2)
                {
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
            else if (cmd == "max bounds factor off")
            {
                if (selectedMeshCombiner != null)
                {
                    selectedMeshCombiner.searchOptions.useMaxBoundsFactor = false;
                    selectedMeshCombiner.AddObjectsAutomatically();
                    if (combineAutomatic) selectedMeshCombiner.CombineAll();
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
            else if (cmd.Contains("max bounds factor "))
            {
                float maxBoundsFactor;
                float.TryParse(cmd.Replace("max bounds factor ", ""), out maxBoundsFactor);
                if (maxBoundsFactor < 1)
                {
                    Log("max bounds factor needs to be bigger than 1");
                    return;
                }
                if (selectedMeshCombiner != null)
                {
                    selectedMeshCombiner.searchOptions.useMaxBoundsFactor = true;
                    selectedMeshCombiner.searchOptions.maxBoundsFactor = maxBoundsFactor;
                    selectedMeshCombiner.AddObjectsAutomatically();
                    if (combineAutomatic) selectedMeshCombiner.CombineAll();
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
            else if (cmd == "vertex input limit off")
            {
                if (selectedMeshCombiner != null)
                {
                    selectedMeshCombiner.searchOptions.useVertexInputLimit = false;
                    selectedMeshCombiner.AddObjectsAutomatically();
                    if (combineAutomatic) selectedMeshCombiner.CombineAll();
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
            else if (cmd.Contains("vertex input limit "))
            {
                int vertexInputLimit;
                int.TryParse(cmd.Replace("vertex input limit ", ""), out vertexInputLimit);
                if (vertexInputLimit < 1)
                {
                    Log("vertex input limit needs to be bigger than 1");
                    return;
                }
                if (selectedMeshCombiner != null)
                {
                    selectedMeshCombiner.searchOptions.useVertexInputLimit = true;
                    selectedMeshCombiner.searchOptions.vertexInputLimit = vertexInputLimit;
                    selectedMeshCombiner.AddObjectsAutomatically();
                    if (combineAutomatic) selectedMeshCombiner.CombineAll();
                    ReportMeshCombiner(selectedMeshCombiner);
                    log.commandType = 2;
                }
            }
        }

        void DirSort()
        {
            GameObject[] gos = Methods.Search<GameObject>(selectGO);

            SortLog(gos, true);
        }

        void DirSort(string name)
        {
            GameObject[] gos = Methods.Search<GameObject>();
            List<GameObject> sortedGos = new List<GameObject>();

            for (int i = 0; i < gos.Length; i++)
            {
                if (Methods.Contains(gos[i].name, name)) sortedGos.Add(gos[i]);
            }
            SortLog(sortedGos.ToArray());
        }

        public void SortLog(GameObject[] gos, bool showMeshInfo = false)
        {
            List<GameObject> list = new List<GameObject>();
            List<int> amountList = new List<int>();

            int count = 0;
            int meshCount = 0;

            for (int i = 0; i < gos.Length; i++)
            {
                GameObject go = gos[i];
                GetMeshInfo(go, ref meshCount);
                string name = go.name;

                int index = -1;
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].name == name) { index = j; break; }
                }

                if (index == -1)
                {
                    list.Add(go);
                    amountList.Add(1);
                    count++;
                }
                else
                {
                    amountList[index]++;
                    count++;
                }
            }

            int temp = 0;
            for (int i = 0; i < list.Count; i++)
            {
                string text = list[i].name + " -> " + amountList[i] + " " + GetMeshInfo(list[i], ref temp);
                Log(text);
            }
            Log("Total amount " + count + " Total items " + list.Count + " Total shared meshes " + meshCount);
        }

        string GetMeshInfo(GameObject go, ref int meshCount)
        {
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf != null)
            {
                Mesh m = mf.sharedMesh;
                if (m != null)
                {
                    ++meshCount;
                    return "(vertices " + m.vertexCount + ", combine " + Mathf.FloorToInt(65000 / m.vertexCount) + ")";
                }
            }
            return "";
        }

        void TimeStep(string cmd)
        {
            float time;
            float.TryParse(cmd, out time);

            Time.fixedDeltaTime = time;
        }

        void TimeScale(string cmd)
        {
            float time;
            float.TryParse(cmd, out time);
            Time.timeScale = time;
        }

        void Clear(LogEntry log, string cmd)
        {
            if (cmd == "clear") { logs.Clear(); log.commandType = 2; }
            else if (cmd == "input")
            {
                for (int i = 0; i < logs.Count; i++)
                {
                    if (!logs[i].unityLog) logs.RemoveAt(i--);
                }
                log.commandType = 2;
            }
            else if (cmd == "unity")
            {
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs[i].unityLog) logs.RemoveAt(i--);
                }
                log.commandType = 2;
            }
        }

        void DirAll()
        {
            GameObject[] gos = Methods.Search<GameObject>(selectGO);
            int meshCount = 0;
            for (int i = 0; i < gos.Length; i++) Log(GetPath(gos[i]) + "\\" + gos[i].transform.childCount + " " + GetMeshInfo(gos[i], ref meshCount), 0, gos[i]);
            Log(gos.Length + " (meshes " + meshCount + ")\\..");
        }

        void Dir()
        {
            int meshCount = 0;
            if (selectGO == null)
            {
                GameObject[] gos = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                for (int i = 0; i < gos.Length; i++) Log(gos[i].name + "\\" + gos[i].transform.childCount + " " + GetMeshInfo(gos[i], ref meshCount), 0, gos[i]);
                Log(gos.Length + " (meshes " + meshCount + ")\\..");
            }
            else
            {
                ShowPath();
                Transform selectT = selectGO.transform;
                for (int i = 0; i < selectT.childCount; i++)
                {
                    Transform child = selectT.GetChild(i);
                    Log(child.name + "\\" + child.childCount + " " + GetMeshInfo(child.gameObject, ref meshCount), 0, child.gameObject);
                }
                Log(selectT.childCount + " (meshes " + meshCount + ")\\..");
            }
        }

        void Components(LogEntry log)
        {
            if (selectGO == null) { log.commandType = 1; return; }

            Component[] components = selectGO.GetComponents<Component>();

            ShowPath(true);
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] != null) Log(components[i].GetType().Name);
            }
            log.commandType = 2;
        }

        void ShowPath(bool showLines = true)
        {
            string path = GetPath(selectGO);
            if (path != "") Log(path); else Log("Root\\");
            if (showLines) Log("---------------------------------");
        }

        string GetPath(GameObject go)
        {
            if (go != null)
            {
                string path = go.name;
                Transform t = go.transform;

                while (t.parent != null)
                {
                    path = path.Insert(0, t.parent.name + "\\");
                    t = t.parent;
                }
                return path;
            }
            return "";
        }

        void CD(LogEntry log, string name)
        {
            if (name == "..")
            {
                if (selectGO != null)
                {
                    if (selectGO.transform.parent != null) selectGO = selectGO.transform.parent.gameObject;
                    else selectGO = null;

                    log.commandType = 2;
                    ShowPath(false);
                    return;
                }
            }
            else if (name == "\\") { selectGO = null; log.commandType = 2; return; }

            Transform t = Methods.Find<Transform>(selectGO, name);

            if (t != null)
            {
                selectGO = t.gameObject;
                ShowPath(false);
                log.commandType = 2;
            }
        }

        public void SetActiveContains(string textContains, bool active)
        {
            GameObject[] gos = Methods.Search<GameObject>(selectGO);

            int count = 0;
            for (int i = 0; i < gos.Length; i++)
            {
                if (Methods.Contains(gos[i].name, textContains))
                {
                    // we shouldn't hide GUI elements :)
                    if (gos[i].transform.parent.name.IndexOf("GUI") == 0 || gos[i].transform.parent.parent == null || gos[i].transform.parent.parent.name.IndexOf("GUI") == 0)
                    {
                        gos[i].SetActive(active);
                        ++count;
                    }
                }
            }
            Log("Total amount set to " + active + " : " + count);
        }

        public void DirContains(string textContains)
        {
            GameObject[] gos = Methods.Search<GameObject>(selectGO);

            int count = 0;
            for (int i = 0; i < gos.Length; i++)
            {
                if (Methods.Contains(gos[i].name, textContains)) { Log(gos[i].name, 0, gos[i]); ++count; }
            }
            Log("Total amount: " + count);
        }

        private void OnGUI()
        {
            if (!showConsole) return;
            window.x = 225;
            window.y = 5;
            window.yMax = (lines * 20) + 30;
            window.xMax = Screen.width - (window.x);

            GUI.Box(window, "Console");

            inputRect.x = window.x + 5;
            inputRect.y = window.yMax - 25;
            inputRect.xMax = window.xMax - 10;
            inputRect.yMax = window.yMax - 5;

            if (showInputLog)
            {
                if (GUI.GetNameOfFocusedControl() == "ConsoleInput")
                {
                    if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
                    {
                        showLast = true;

                        ExecuteCommand(inputText);
                        inputText = string.Empty;
                    }
                }

                GUI.SetNextControlName("ConsoleInput");
                GUI.changed = false;
                inputText = GUI.TextField(inputRect, inputText);
                if (GUI.changed)
                {
                    if (inputText.Contains("`"))
                    {
                        inputText = inputText.Replace("`", "");
                        SetConsoleActive(!showConsole);
                    }
                }
                if (setFocus)
                {
                    setFocus = false;
                    GUI.FocusControl("ConsoleInput");
                }
            }

            if (showInputLog) GUI.color = Color.green; else GUI.color = Color.grey;
            if (GUI.Button(new Rect(window.xMin + 5, window.yMin + 5, 75, 20), "Input Log"))
            {
                showInputLog = !showInputLog;
            }
            if (showUnityLog) GUI.color = Color.green; else GUI.color = Color.grey;
            if (GUI.Button(new Rect(window.xMin + 85, window.yMin + 5, 75, 20), "Unity Log"))
            {
                showUnityLog = !showUnityLog;
            }
            GUI.color = Color.white;

            if (!showInputLog && !showUnityLog) showInputLog = true;

            logRect.x = window.x + 5;
            logRect.y = window.y + 25;
            logRect.xMax = window.xMax - 20;
            logRect.yMax = logRect.y + 20;

            vScrollRect.x = window.xMax - 15;
            vScrollRect.y = logRect.y;
            vScrollRect.xMax = window.xMax - 5;
            vScrollRect.yMax = window.yMax - 45;

            float size = Mathf.Ceil(vScrollRect.height / 20);

            if (showLast && Event.current.type != EventType.Repaint) scrollPos = logs.Count;

            GUI.changed = false;
            scrollPos = GUI.VerticalScrollbar(vScrollRect, scrollPos, size > logs.Count - 1 ? logs.Count - 1 : size - 1, 0, logs.Count - 1);
            if (GUI.changed)
            {
                showLast = false;
            }

            int start = (int)scrollPos;
            if (start < 0) start = 0;

            int end = start + (int)size;
            if (end > logs.Count) end = logs.Count;

            int amount = end - start;
            int i = start;
            int index = 0;

            while (index != amount && i < logs.Count)
            {
                LogEntry log = logs[i];

                if ((log.unityLog && showUnityLog) || (!log.unityLog && showInputLog))
                {

                    if (log.logType == LogType.Warning) AnimateColor(Color.yellow, log, 0.75f);
                    else if (log.logType == LogType.Error) AnimateColor(Color.red, log, 0.75f);
                    else if (log.logType == LogType.Exception) AnimateColor(Color.magenta, log, 0.75f);
                    else if (log.unityLog) AnimateColor(Color.white, log, 0.75f);
                    else if (log.commandType == 1) GUI.color = new Color(0, 0.5f, 0);
                    else if (log.commandType == 2) GUI.color = Color.green;
                    else if (log.go != null) GUI.color = log.go.activeSelf ? Color.white : Color.white * 0.7f;

                    string text = log.logString;

                    if (text.Contains("*color-"))
                    {
                        if (text.Contains("*color-green#")) { text = text.Replace("*color-green#", ""); GUI.color = Color.green; }
                        else if (text.Contains("*color-blue#")) { text = text.Replace("*color-blue#", ""); GUI.color = Color.blue; }
                    }

                    GUI.Label(logRect, i + ") ");
                    logRect.xMin += 55;
                    GUI.Label(logRect, text + ((log.stackTrace != "") ? (" (" + log.stackTrace + ")") : ""));
                    logRect.xMin -= 55;

                    GUI.color = Color.white;
                    logRect.y += 20;
                    index++;
                }

                ++i;
            }
        }

        void AnimateColor(Color col, LogEntry log, float multi)
        {
            GUI.color = Color.Lerp(col * multi, col, Mathf.Abs(Mathf.Sin(Time.time)));
        }

        public class LogEntry
        {
            public string logString;
            public string stackTrace;
            public LogType logType;
            public int commandType;
            public bool unityLog;
            public float tStamp;
            public GameObject go;
            public MeshCombiner meshCombiner;

            public LogEntry(string logString, string stackTrace, LogType logType, bool unityLog = false, int commandType = 0, GameObject go = null, MeshCombiner meshCombiner = null)
            {
                this.logString = logString;
                this.stackTrace = stackTrace;
                this.logType = logType;
                this.unityLog = unityLog;
                this.commandType = commandType;
                this.go = go;
                this.meshCombiner = meshCombiner;
                // tStamp = Time.time;
            }
        }

    }
}