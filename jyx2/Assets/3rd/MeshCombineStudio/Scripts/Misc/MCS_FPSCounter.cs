using UnityEngine;

namespace MeshCombineStudio
{
    public class MCS_FPSCounter : MonoBehaviour
    {

        static public MCS_FPSCounter instance;

        [Header("___ Settings ___________________________________________________________________________________________________________")]
        public float interval = 0.25f;
        public enum GUIType { DisplayRunning, DisplayResults, DisplayNothing }
        public GUIType displayType = GUIType.DisplayRunning;
        public Vector2 gradientRange = new Vector2(15, 60);
        public Font fontRun;
        public Font fontResult;
        public Texture logo;
        public bool showLogoOnResultsScreen = true;
        public KeyCode showHideButton = KeyCode.Backspace;
        public bool acceptInput = true;
        public bool reset;

        [Header("___ Results ___________________________________________________________________________________________________________")]
        public float currentFPS = 0;
        public float averageFPS = 0;
        public float minimumFPS = 0;
        public float maximumFPS = 0;

        // Privates
        int totalFrameCount, tempFrameCount;
        double tStamp, tStampTemp;

        // GUI-------------------------------------------
        string currentFPSText, avgFPSText, minFPSText, maxFSPText;

        GUIStyle bigStyle = new GUIStyle(); GUIStyle bigStyleShadow;
        GUIStyle smallStyle = new GUIStyle(); GUIStyle smallStyleShadow; GUIStyle smallStyleLabel;
        GUIStyle headerStyle = new GUIStyle();

        Rect[] rectsRun = { new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect() };
        Rect[] rectsResult = { new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect(), new Rect() };
        Gradient gradient = new Gradient();

        const float line1 = 4;
        const float line2 = line1 + 26;
        const float line3 = line2 + 14;
        const float line4 = line3 + 14;
        const float labelWidth = 26;
        const float paddingH = 8;
        const float lineHeight = 22;
        float columnRight;
        float columnLeft;

        Color fontShadow = new Color(0, 0, 0, 0.5f);
        Color label = new Color(0.8f, 0.8f, 0.8f, 1);
        Color colorCurrent, colorAvg;

        const string resultHeader = "BENCHMARK RESULTS";
        const string resultLabelAvg = "AVERAGE FPS:";
        const string resultLabelMin = "MINIMUM FPS:";
        const string resultLabelMax = "MAXIMUM FPS:";
        GUIContent resultHeaderGUI = new GUIContent(resultHeader);
        GUIContent reslutLabelAvgGUI = new GUIContent(resultLabelAvg);
        GUIContent avgTextGUI = new GUIContent();
        GUIContent instructions = new GUIContent("PRESS SPACEBAR TO RERUN BENCHMARK | PRESS ESCAPE TO RETURN TO MENU");

        const string runLabelAvg = "Avg:";
        const string runLabelMin = "Min:";
        const string runLabelMax = "Max:";

        Vector2 screenSize = new Vector2(0, 0);
        GUIType oldDisplayType = GUIType.DisplayNothing;
        //-----------------------------------------------

        private void Awake()
        {
            instance = this;

            gradient.colorKeys = new GradientColorKey[] { new GradientColorKey(new Color(1, 0, 0, 1), 0), new GradientColorKey(new Color(1, 1, 0, 1), 0.5f), new GradientColorKey(new Color(0, 1, 0, 1), 1f) };
        }

        void OnDestroy() { if (instance == this) instance = null; }

        void OnGUI()
        {
            if (displayType == GUIType.DisplayNothing) return;
            else if (displayType == GUIType.DisplayRunning)
            {
                if (Screen.width != screenSize.x || Screen.height != screenSize.y) { screenSize.x = Screen.width; screenSize.y = Screen.height; SetRectsRun(); }

                // TEXT DROPSHADOWS ----------------------------------------------------------
                GUI.Label(rectsRun[0], currentFPSText, bigStyleShadow); // Result Current FPS
                GUI.Label(rectsRun[1], avgFPSText, smallStyleShadow); // Result Average FPS
                GUI.Label(rectsRun[2], minFPSText, smallStyleShadow); // ReSult Minimum FPS
                GUI.Label(rectsRun[3], maxFSPText, smallStyleShadow); // Result Maximum FPS

                GUI.Label(rectsRun[4], runLabelAvg, smallStyleShadow); // Label Average FPS
                GUI.Label(rectsRun[5], runLabelMin, smallStyleShadow); // Label Minimum FPS
                GUI.Label(rectsRun[6], runLabelMax, smallStyleShadow); // Label Maximum FPS

                // TEXT ----------------------------------------------------------------------
                GUI.Label(rectsRun[7], currentFPSText, bigStyle); // Result Current FPS
                GUI.Label(rectsRun[8], avgFPSText, smallStyle); // Result Average FPS
                GUI.Label(rectsRun[9], minFPSText, smallStyle); // ReSult Minimum FPS
                GUI.Label(rectsRun[10], maxFSPText, smallStyle); // Result Maximum FPS

                GUI.Label(rectsRun[11], runLabelAvg, smallStyleLabel); // Label Average FPS
                GUI.Label(rectsRun[12], runLabelMin, smallStyleLabel); // Label Minimum FPS
                GUI.Label(rectsRun[13], runLabelMax, smallStyleLabel); // Label Maximum FPS
            }
            else
            {
                if (Screen.width != screenSize.x || Screen.height != screenSize.y) { screenSize.x = Screen.width; screenSize.y = Screen.height; SetRectsResult(); }

                if (showLogoOnResultsScreen) GUI.DrawTexture(rectsResult[8], logo);

                GUI.Label(rectsResult[0], resultHeaderGUI, headerStyle); // Header
                GUI.DrawTexture(rectsResult[1], Texture2D.whiteTexture); // Line

                GUI.Label(rectsResult[2], reslutLabelAvgGUI, smallStyle); // Label Average FPS
                GUI.Label(rectsResult[4], resultLabelMin, smallStyleLabel); // Label Minimum FPS
                GUI.Label(rectsResult[6], resultLabelMax, smallStyleLabel); // Label Maximum FPS

                GUI.Label(rectsResult[3], avgTextGUI, bigStyle); // Result Average FPS
                GUI.Label(rectsResult[5], minFPSText, smallStyle); // ReSult Minimum FPS
                GUI.Label(rectsResult[7], maxFSPText, smallStyle); // Result Maximum FPS

                GUI.Label(rectsResult[9], instructions, smallStyleLabel); // Instructions
            }
        } //==============================================================================================================
        void SetRectsRun()
        {
            columnRight = Screen.width - (labelWidth + paddingH);
            columnLeft = columnRight - (labelWidth + paddingH);

            // float editorOffset = Screen.height * 0.09f;
            float editorOffset = 0;


            // TEXT DROPSHADOWS ----------------------------------------------------------
            rectsRun[0].Set(Screen.width - (40 + paddingH) + 1, editorOffset + line1 + 2, 40, lineHeight); // Result Current FPS
            rectsRun[1].Set(columnRight + 1, editorOffset + line2 + 2, labelWidth, lineHeight); // Result Average FPS
            rectsRun[2].Set(columnRight + 1, editorOffset + line3 + 2, labelWidth, lineHeight); // ReSult Minimum FPS
            rectsRun[3].Set(columnRight + 1, editorOffset + line4 + 2, labelWidth, lineHeight); // Result Maximum FPS

            rectsRun[4].Set(columnLeft + 1, editorOffset + line2 + 2, labelWidth, lineHeight); // Label Average FPS
            rectsRun[5].Set(columnLeft + 1, editorOffset + line3 + 2, labelWidth, lineHeight); // Label Minimum FPS
            rectsRun[6].Set(columnLeft + 1, editorOffset + line4 + 2, labelWidth, lineHeight); // Label Maximum FPS

            // TEXT ----------------------------------------------------------------------
            rectsRun[7].Set(Screen.width - (45 + paddingH), editorOffset + line1, 45, lineHeight); // Result Current FPS
            rectsRun[8].Set(columnRight, editorOffset + line2, labelWidth, lineHeight); // Result Average FPS
            rectsRun[9].Set(columnRight, editorOffset + line3, labelWidth, lineHeight); // ReSult Minimum FPS
            rectsRun[10].Set(columnRight, editorOffset + line4, labelWidth, lineHeight); // Result Maximum FPS

            rectsRun[11].Set(columnLeft, editorOffset + line2, labelWidth, lineHeight); // Label Average FPS
            rectsRun[12].Set(columnLeft, editorOffset + line3, labelWidth, lineHeight); // Label Minimum FPS
            rectsRun[13].Set(columnLeft, editorOffset + line4, labelWidth, lineHeight); // Result Maximum FPS
        } //==============================================================================================================

        void SetRectsResult()
        {
            float totalHeight = 512 / 2;
            rectsResult[8].Set((Screen.width / 2) - (logo.width / 2), (Screen.height / 2) - totalHeight, logo.width, logo.height); // Drone Logo

            Vector2 size = headerStyle.CalcSize(resultHeaderGUI);
            rectsResult[0].Set((Screen.width / 2) - (size.x / 2), (Screen.height / 2) - (totalHeight - 256), size.x, size.y); // Header
            size.x += 10;
            rectsResult[1].Set((Screen.width / 2) - (size.x / 2), (Screen.height / 2) - (totalHeight - 256 - 30), size.x, 1); // Line

            rectsResult[2].Set((Screen.width / 2) - 200, (Screen.height / 2) - (totalHeight - 256 - 30 - 30), 200, 24); // Label Average FPS
            rectsResult[4].Set((Screen.width / 2) - 200, (Screen.height / 2) - (totalHeight - 256 - 30 - 30 - 20), 200, 24); // Label Minimum FPS
            rectsResult[6].Set((Screen.width / 2) - 200, (Screen.height / 2) - (totalHeight - 256 - 30 - 30 - 20 - 20), 200, 24); // Label Maximum FPS

            rectsResult[3].Set((Screen.width / 2), (Screen.height / 2) - (totalHeight - 256 - 30 - 18), 65, 24); // Result Average FPS
            rectsResult[5].Set((Screen.width / 2), (Screen.height / 2) - (totalHeight - 256 - 30 - 30 - 20), 65, 24); // ReSult Minimum FPS
            rectsResult[7].Set((Screen.width / 2), (Screen.height / 2) - (totalHeight - 256 - 30 - 30 - 20 - 20), 65, 24); // Result Maximum FPS
            size = smallStyleLabel.CalcSize(instructions);
            rectsResult[9].Set((Screen.width / 2) - (size.x / 2), (Screen.height / 2) - (totalHeight - 256 - 30 - 30 - 20 - 20 - 40), size.x, size.y); // Instructions
        } //==============================================================================================================


        void Start()
        {
            headerStyle.normal.textColor = label;
            headerStyle.fontSize = 24;
            headerStyle.font = fontResult;
            headerStyle.alignment = TextAnchor.UpperCenter;

            bigStyle.alignment = TextAnchor.UpperRight;
            bigStyle.font = fontRun;
            bigStyle.fontSize = 24;
            bigStyle.normal.textColor = Color.green;
            bigStyleShadow = new GUIStyle(bigStyle);
            bigStyleShadow.normal.textColor = fontShadow;

            smallStyle.alignment = TextAnchor.UpperRight;
            smallStyle.font = fontRun;
            smallStyle.fontSize = 12;
            smallStyle.normal.textColor = Color.white;
            smallStyleShadow = new GUIStyle(smallStyle);
            smallStyleShadow.normal.textColor = fontShadow;
            smallStyleLabel = new GUIStyle(smallStyle);
            smallStyleLabel.normal.textColor = label;

            Invoke("Reset", 0.5f);
        } //==============================================================================================================


        void Update()
        {
            if (displayType != oldDisplayType)
            {
                if (displayType == GUIType.DisplayResults)
                {
                    SetRectsResult();
                    colorAvg = EvaluateGradient(averageFPS);
                    bigStyle.normal.textColor = colorAvg;
                    avgTextGUI.text = avgFPSText;
                }
                else if (displayType == GUIType.DisplayRunning)
                {
                    Reset();
                    SetRectsRun();
                }
                oldDisplayType = displayType;
            }

            if (Input.GetKeyDown(showHideButton) && acceptInput && displayType != GUIType.DisplayResults)
            {
                if (displayType == GUIType.DisplayNothing) displayType = GUIType.DisplayRunning;
                else displayType = GUIType.DisplayNothing;
            }

            if (displayType == GUIType.DisplayNothing) return;
            else if (displayType == GUIType.DisplayRunning) GetFPS();
            if (reset) { reset = false; Reset(); }
        } //==============================================================================================================


        public void StartBenchmark()
        {
            Reset();
            SetRectsRun();
            displayType = GUIType.DisplayRunning;
        } //==============================================================================================================


        public void StopBenchmark()
        {
            SetRectsResult();
            displayType = GUIType.DisplayResults;
            colorAvg = EvaluateGradient(averageFPS);
            bigStyle.normal.textColor = colorAvg;
            //R_Console.Log("---------------------------------");
            //R_Console.Log("Average FPS: " + averageFPS.ToString("F2"));
            //R_Console.Log("Mininum FPS: " + minimumFPS.ToString("F2"));
            //R_Console.Log("Maximum FPS: " + maximumFPS.ToString("F2"));
            //R_Console.Log("---------------------------------");
        } //==============================================================================================================


        void GetFPS()
        {
            tempFrameCount++;
            totalFrameCount++;

            if (Time.realtimeSinceStartup - tStampTemp > interval)
            {
                currentFPS = (float)(tempFrameCount / (Time.realtimeSinceStartup - tStampTemp));
                averageFPS = (float)(totalFrameCount / (Time.realtimeSinceStartup - tStamp));
                if (currentFPS < minimumFPS) minimumFPS = currentFPS;
                if (currentFPS > maximumFPS) maximumFPS = currentFPS;

                tStampTemp = Time.realtimeSinceStartup;
                tempFrameCount = 0;

                currentFPSText = "FPS " + currentFPS.ToString("0.0");
                avgFPSText = averageFPS.ToString("0.0");
                minFPSText = minimumFPS.ToString("0.0");
                maxFSPText = maximumFPS.ToString("0.0");
                colorCurrent = EvaluateGradient(currentFPS);
                bigStyle.normal.textColor = colorCurrent;
            }
        } //==============================================================================================================


        public void Reset()
        {
            tStamp = Time.realtimeSinceStartup;
            tStampTemp = Time.realtimeSinceStartup;

            currentFPS = 0;
            averageFPS = 0;
            minimumFPS = 999.9f;
            maximumFPS = 0;

            tempFrameCount = 0;
            totalFrameCount = 0;
        } //==============================================================================================================


        Color EvaluateGradient(float f) { return gradient.Evaluate(Mathf.Clamp01((f - gradientRange.x) / (gradientRange.y - gradientRange.x))); }
    }
}