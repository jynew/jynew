// Copyright (c) 2015 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#if UNITY_2017 || UNITY_2018 || UNITY_2019 || UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
#define UNITY_2017_PLUS
#endif

#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_PLUS
#define UNITY_4_6_PLUS
#endif

#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649

namespace Rewired.Data {

    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Rewired;
    using Rewired.Utils.Libraries.TinyJson;

    /// <summary>
    /// Class for saving data to PlayerPrefs. Add this as a component to your Rewired Input Manager to save and load data automatically to PlayerPrefs.
    /// Copy this class and customize it to your needs to create a new custom data storage system.
    /// </summary>
    public class UserDataStore_PlayerPrefs : UserDataStore {

        private const string thisScriptName = "UserDataStore_PlayerPrefs";
        private const string logPrefix = "Rewired: ";
        private const string editorLoadedMessage = "\n***IMPORTANT:*** Changes made to the Rewired Input Manager configuration after the last time XML data was saved WILL NOT be used because the loaded old saved data has overwritten these values. If you change something in the Rewired Input Manager such as a Joystick Map or Input Behavior settings, you will not see these changes reflected in the current configuration. Clear PlayerPrefs using the inspector option on the UserDataStore_PlayerPrefs component.";
        private const string playerPrefsKeySuffix_controllerAssignments = "ControllerAssignments";

        // 0 = original
        // 1 = include duplicate joystick index
        // 2 = support for loading maps for disconnected recognized controllers
        private const int controllerMapPPKeyVersion_original = 0;
        private const int controllerMapPPKeyVersion_includeDuplicateJoystickIndex = 1;
        private const int controllerMapPPKeyVersion_supportDisconnectedControllers = 2;
        private const int controllerMapPPKeyVersion_includeFormatVersion = 2;
        private const int controllerMapPPKeyVersion = 2;

#if UNITY_4_6_PLUS
        [Tooltip("Should this script be used? If disabled, nothing will be saved or loaded.")]
#endif
        [UnityEngine.SerializeField]
        private bool isEnabled = true;

#if UNITY_4_6_PLUS
        [Tooltip("Should saved data be loaded on start?")]
#endif
        [UnityEngine.SerializeField]
        private bool loadDataOnStart = true;

#if UNITY_4_6_PLUS
        [Tooltip("Should Player Joystick assignments be saved and loaded? This is not totally reliable for all Joysticks on all platforms. " +
            "Some platforms/input sources do not provide enough information to reliably save assignments from session to session " +
            "and reboot to reboot.")]
#endif
        [UnityEngine.SerializeField]
        private bool loadJoystickAssignments = true;

#if UNITY_4_6_PLUS
        [Tooltip("Should Player Keyboard assignments be saved and loaded?")]
#endif
        [UnityEngine.SerializeField]
        private bool loadKeyboardAssignments = true;

#if UNITY_4_6_PLUS
        [Tooltip("Should Player Mouse assignments be saved and loaded?")]
#endif
        [UnityEngine.SerializeField]
        private bool loadMouseAssignments = true;

#if UNITY_4_6_PLUS
        [Tooltip("The PlayerPrefs key prefix. Change this to change how keys are stored in PlayerPrefs. Changing this will make saved data already stored with the old key no longer accessible.")]
#endif
        [UnityEngine.SerializeField]
        private string playerPrefsKeyPrefix = "RewiredSaveData";

        /// <summary>
        /// Should this script be used? If disabled, nothing will be saved or loaded.
        /// </summary>
        public bool IsEnabled { get { return isEnabled; } set { isEnabled = value; } }
        /// <summary>
        /// Should saved data be loaded on start?
        /// </summary>
        public bool LoadDataOnStart { get { return loadDataOnStart; } set { loadDataOnStart = value; } }
        /// <summary>
        /// Should Player Joystick assignments be saved and loaded? This is not totally reliable for all Joysticks on all platforms.
        /// Some platforms/input sources do not provide enough information to reliably save assignments from session to session
        /// and reboot to reboot.
        /// </summary>
        public bool LoadJoystickAssignments { get { return loadJoystickAssignments; } set { loadJoystickAssignments = value; } }
        /// <summary>
        /// Should Player Keyboard assignments be saved and loaded?
        /// </summary>
        public bool LoadKeyboardAssignments { get { return loadKeyboardAssignments; } set { loadKeyboardAssignments = value; } }
        /// <summary>
        /// Should Player Mouse assignments be saved and loaded?
        /// </summary>
        public bool LoadMouseAssignments { get { return loadMouseAssignments; } set { loadMouseAssignments = value; } }
        /// <summary>
        /// The PlayerPrefs key prefix. Change this to change how keys are stored in PlayerPrefs. Changing this will make saved data already stored with the old key no longer accessible.
        /// </summary>
        public string PlayerPrefsKeyPrefix { get { return playerPrefsKeyPrefix; } set { playerPrefsKeyPrefix = value; } }

        private string playerPrefsKey_controllerAssignments { get { return string.Format("{0}_{1}", playerPrefsKeyPrefix, playerPrefsKeySuffix_controllerAssignments); } }

        private bool loadControllerAssignments { get { return loadKeyboardAssignments || loadMouseAssignments || loadJoystickAssignments; } }

        private List<int> allActionIds {
            get {
                if (__allActionIds != null) return __allActionIds; // use the cached version
                List<int> ids = new List<int>();
                IList<InputAction> actions = ReInput.mapping.Actions;
                for (int i = 0; i < actions.Count; i++) {
                    ids.Add(actions[i].id);
                }
                __allActionIds = ids;
                return ids;
            }
        }

        private string allActionIdsString {
            get {
                if (!string.IsNullOrEmpty(__allActionIdsString)) return __allActionIdsString; // use the cached version
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                List<int> ids = allActionIds;
                for (int i = 0; i < ids.Count; i++) {
                    if (i > 0) sb.Append(",");
                    sb.Append(ids[i]);
                }
                __allActionIdsString = sb.ToString();
                return __allActionIdsString;
            }
        }

        [NonSerialized]
        private bool allowImpreciseJoystickAssignmentMatching = true;
        [NonSerialized]
        private bool deferredJoystickAssignmentLoadPending;
        [NonSerialized]
        private bool wasJoystickEverDetected;
        [NonSerialized]
        private List<int> __allActionIds;
        [NonSerialized]
        private string __allActionIdsString;

        #region UserDataStore Implementation

        // Public Methods

        /// <summary>
        /// Save all data now.
        /// </summary>
        public override void Save() {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not save any data.", this);
                return;
            }
            SaveAll();

#if UNITY_EDITOR
            Debug.Log(logPrefix + thisScriptName + " saved all user data to XML.");
#endif
        }

        /// <summary>
        /// Save all data for a specific controller for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void SaveControllerData(int playerId, ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not save any data.", this);
                return;
            }
            SaveControllerDataNow(playerId, controllerType, controllerId);

#if UNITY_EDITOR
            Debug.Log(logPrefix + thisScriptName + " saved " + controllerType + " " + controllerId + " data for Player " + playerId + " to XML.");
#endif
        }

        /// <summary>
        /// Save all data for a specific controller. Does not save Player data.
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void SaveControllerData(ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not save any data.", this);
                return;
            }
            SaveControllerDataNow(controllerType, controllerId);

#if UNITY_EDITOR
            Debug.Log(logPrefix + thisScriptName + " saved " + controllerType + " " + controllerId + " data to XML.");
#endif
        }

        /// <summary>
        /// Save all data for a specific Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        public override void SavePlayerData(int playerId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not save any data.", this);
                return;
            }
            SavePlayerDataNow(playerId);

#if UNITY_EDITOR
            Debug.Log(logPrefix + thisScriptName + " saved all user data for Player " + playerId + " to XML.");
#endif
        }

        /// <summary>
        /// Save all data for a specific InputBehavior for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="behaviorId">Input Behavior id</param>
        public override void SaveInputBehavior(int playerId, int behaviorId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not save any data.", this);
                return;
            }
            SaveInputBehaviorNow(playerId, behaviorId);

#if UNITY_EDITOR
            Debug.Log(logPrefix + thisScriptName + " saved Input Behavior data for Player " + playerId + " to XML.");
#endif
        }

        /// <summary>
        /// Load all data now.
        /// </summary>
        public override void Load() {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not load any data.", this);
                return;
            }
            int count = LoadAll();

#if UNITY_EDITOR
            if(count > 0) Debug.LogWarning(logPrefix + thisScriptName + " loaded all user data from XML. " + editorLoadedMessage);
#endif
        }

        /// <summary>
        /// Load all data for a specific controller for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void LoadControllerData(int playerId, ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not load any data.", this);
                return;
            }
            int count = LoadControllerDataNow(playerId, controllerType, controllerId);

#if UNITY_EDITOR
            if(count > 0) Debug.LogWarning(logPrefix + thisScriptName + " loaded user data for " + controllerType + " " + controllerId + " for Player " + playerId + " from XML. " + editorLoadedMessage);
#endif
        }

        /// <summary>
        /// Load all data for a specific controller. Does not load Player data.
        /// </summary>
        /// <param name="controllerType">Controller type</param>
        /// <param name="controllerId">Controller id</param>
        public override void LoadControllerData(ControllerType controllerType, int controllerId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not load any data.", this);
                return;
            }
            int count = LoadControllerDataNow(controllerType, controllerId);

#if UNITY_EDITOR
            if(count > 0) Debug.LogWarning(logPrefix + thisScriptName + " loaded user data for " + controllerType + " " + controllerId + " from XML. " + editorLoadedMessage);
#endif
        }

        /// <summary>
        /// Load all data for a specific Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        public override void LoadPlayerData(int playerId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not load any data.", this);
                return;
            }
            int count = LoadPlayerDataNow(playerId);

#if UNITY_EDITOR
            if(count > 0) Debug.LogWarning(logPrefix + thisScriptName + " loaded Player + " + playerId + " user data from XML. " + editorLoadedMessage);
#endif
        }

        /// <summary>
        /// Load all data for a specific InputBehavior for a Player.
        /// </summary>
        /// <param name="playerId">Player id</param>
        /// <param name="behaviorId">Input Behavior id</param>
        public override void LoadInputBehavior(int playerId, int behaviorId) {
            if(!isEnabled) {
                Debug.LogWarning(logPrefix + thisScriptName + " is disabled and will not load any data.", this);
                return;
            }
            int count = LoadInputBehaviorNow(playerId, behaviorId);

#if UNITY_EDITOR
            if(count > 0) Debug.LogWarning(logPrefix + thisScriptName + " loaded Player + " + playerId + " InputBehavior data from XML. " + editorLoadedMessage);
#endif
        }

        // Event Handlers

        /// <summary>
        /// Called when SaveDataStore is initialized.
        /// </summary>
        protected override void OnInitialize() {

            // Disallow imprecise joystick assignment matching on some platforms when
            // system id/player Rewired Player alignment needs to stay fixed.
#if !UNITY_EDITOR && (UNITY_XBOXONE || UNITY_PS4 || UNITY_SWITCH)
            allowImpreciseJoystickAssignmentMatching = false;
#endif

            if(loadDataOnStart) {
                Load();

                // Save the controller assignments immediately only if there were joysticks connected on start
                // so the initial auto-assigned joystick assignments will be saved without any user intervention.
                // This will not save over controller assignment data if no joysticks were attached initially.
                // This is not always saved because of delayed joystick connection on some platforms like iOS.
                if(loadControllerAssignments && ReInput.controllers.joystickCount > 0) {
                    wasJoystickEverDetected = true;
                    SaveControllerAssignments();
                }
            }
        }

        /// <summary>
        /// Called when a controller is connected.
        /// </summary>
        /// <param name="args">ControllerStatusChangedEventArgs</param>
        protected override void OnControllerConnected(ControllerStatusChangedEventArgs args) {
            if(!isEnabled) return;

            // Load data when joystick is connected
            if(args.controllerType == ControllerType.Joystick) {
                int count = LoadJoystickData(args.controllerId);
#if UNITY_EDITOR
                if(count > 0) Debug.LogWarning(logPrefix + thisScriptName + " loaded Joystick " + args.controllerId + " (" + ReInput.controllers.GetJoystick(args.controllerId).hardwareName + ") data from XML. " + editorLoadedMessage);
#endif

                // Load joystick assignments once on connect, but deferred until the end of the frame so all joysticks can connect first.
                // This is to get around the issue on some platforms like OSX, Xbox One, and iOS where joysticks are not
                // available immediately and may not be available for several seconds after the Rewired Input manager or
                // Unity starts. Also allows the user to start the game with no joysticks connected and on the first
                // joystick connected, load the assignments for a better user experience on phones/tablets.
                // No further joystick assignments will be made on connect.
                if (loadDataOnStart && loadJoystickAssignments && !wasJoystickEverDetected) {
                    this.StartCoroutine(LoadJoystickAssignmentsDeferred());
                }

                // Save controller assignments
                if(loadJoystickAssignments && !deferredJoystickAssignmentLoadPending) { // do not save assignments while deferred loading is still pending
                    SaveControllerAssignments();
                }

                wasJoystickEverDetected = true;
            }
        }

        /// <summary>
        /// Calls after a controller has been disconnected.
        /// </summary>
        /// <param name="args">ControllerStatusChangedEventArgs</param>
        protected override void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args) {
            if(!isEnabled) return;

            // Save data before joystick is disconnected
            if(args.controllerType == ControllerType.Joystick) {
                SaveJoystickData(args.controllerId);
#if UNITY_EDITOR
                Debug.Log(logPrefix + thisScriptName + " saved Joystick " + args.controllerId + " (" + ReInput.controllers.GetJoystick(args.controllerId).hardwareName + ") data to XML.");
#endif
            }
        }

        /// <summary>
        /// Called when a controller is disconnected.
        /// </summary>
        /// <param name="args">ControllerStatusChangedEventArgs</param>
        protected override void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {
            if(!isEnabled) return;

            // Save controller assignments
            if(loadControllerAssignments) SaveControllerAssignments();
        }

        #endregion

        #region IControllerMapStore Implementation

        /// <summary>
        /// Saves a Controller Map.
        /// </summary>
        /// <param name="playerId">The Player id</param>
        /// <param name="controllerMap">The Controller Map</param>
        public override void SaveControllerMap(int playerId, ControllerMap controllerMap) {
            if (controllerMap == null) return;
            Player player = ReInput.players.GetPlayer(playerId);
            if (player == null) return;
            SaveControllerMap(player, controllerMap);
        }

        /// <summary>
        /// Loads a Controller Map for a Controller.
        /// </summary>
        /// <param name="playerId">The Player id</param>
        /// <param name="controllerIdentifier">Controller Identifier for the Controller. Get this from <see cref="Controller.identifier"/>.</param>
        /// <param name="categoryId">The Map Category id of the Controller Map</param>
        /// <param name="layoutId">The Layout id of the Controller Map</param>
        /// <returns>Controller Map</returns>
        public override ControllerMap LoadControllerMap(int playerId, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if (player == null) return null;
            return LoadControllerMap(player, controllerIdentifier, categoryId, layoutId);
        }

        #endregion

        #region Load

        private int LoadAll() {

            int count = 0;

            // Load controller assignments first so the right maps are loaded
            if(loadControllerAssignments) {
                if(LoadControllerAssignmentsNow()) count += 1;
            }

            // Load all data for all players
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) {
                count += LoadPlayerDataNow(allPlayers[i]);
            }

            // Load all joystick calibration maps
            count += LoadAllJoystickCalibrationData();

            return count;
        }

        private int LoadPlayerDataNow(int playerId) {
            return LoadPlayerDataNow(ReInput.players.GetPlayer(playerId));
        }
        private int LoadPlayerDataNow(Player player) {
            if(player == null) return 0;

            int count = 0;

            // Load Input Behaviors
            count += LoadInputBehaviors(player.id);

            // Load Keyboard Maps
            count += LoadControllerMaps(player.id, ControllerType.Keyboard, 0);

            // Load Mouse Maps
            count += LoadControllerMaps(player.id, ControllerType.Mouse, 0);

            // Load Joystick Maps for each joystick
            foreach(Joystick joystick in player.controllers.Joysticks) {
                count += LoadControllerMaps(player.id, ControllerType.Joystick, joystick.id);
            }

            // Trigger Layout Manager refresh after load
            RefreshLayoutManager(player.id);

            return count;
        }

        private int LoadAllJoystickCalibrationData() {
            int count = 0;
            // Load all calibration maps from all joysticks
            IList<Joystick> joysticks = ReInput.controllers.Joysticks;
            for(int i = 0; i < joysticks.Count; i++) {
                count += LoadJoystickCalibrationData(joysticks[i]);
            }
            return count;
        }

        private int LoadJoystickCalibrationData(Joystick joystick) {
            if(joystick == null) return 0;
            return joystick.ImportCalibrationMapFromXmlString(GetJoystickCalibrationMapXml(joystick)) ? 1 : 0; // load joystick calibration map
        }
        private int LoadJoystickCalibrationData(int joystickId) {
            return LoadJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
        }

        private int LoadJoystickData(int joystickId) {
            int count = 0;
            // Load joystick maps in all Players for this joystick id
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) { // this controller may be owned by more than one player, so check all
                Player player = allPlayers[i];
                if(!player.controllers.ContainsController(ControllerType.Joystick, joystickId)) continue; // player does not have the joystick
                count += LoadControllerMaps(player.id, ControllerType.Joystick, joystickId); // load the maps
                RefreshLayoutManager(player.id); // trigger Layout Manager refresh after load
            }

            // Load calibration maps for joystick
            count += LoadJoystickCalibrationData(joystickId);

            return count;
        }

        private int LoadControllerDataNow(int playerId, ControllerType controllerType, int controllerId) {

            int count = 0;

            // Load map data
            count += LoadControllerMaps(playerId, controllerType, controllerId);

            // Trigger Layout Manager refresh after load
            RefreshLayoutManager(playerId);

            // Loat other controller data
            count += LoadControllerDataNow(controllerType, controllerId);

            return count;
        }
        private int LoadControllerDataNow(ControllerType controllerType, int controllerId) {

            int count = 0;

            // Load calibration data for joysticks
            if(controllerType == ControllerType.Joystick) {
                count += LoadJoystickCalibrationData(controllerId);
            }

            return count;
        }

        private int LoadControllerMaps(int playerId, ControllerType controllerType, int controllerId) {
            int count = 0;
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return count;

            Controller controller = ReInput.controllers.GetController(controllerType, controllerId);
            if(controller == null) return count;

            IList<InputMapCategory> categories = ReInput.mapping.MapCategories;
            for (int i = 0; i < categories.Count; i++) {

                InputMapCategory category = categories[i];
                if (!category.userAssignable) continue; // skip map because not user-assignable

                IList<InputLayout> layouts = ReInput.mapping.MapLayouts(controller.type);
                for (int j = 0; j < layouts.Count; j++) {

                    InputLayout layout = layouts[j];

                    // Load the Controller Map
                    ControllerMap controllerMap = LoadControllerMap(player, controller.identifier, category.id, layout.id);
                    if (controllerMap == null) continue;

                    // Add the map to the Player
                    player.controllers.maps.AddMap(controller, controllerMap);
                    count += 1;
                }
            }

            return count;
        }

        private ControllerMap LoadControllerMap(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId) {
            if (player == null) return null;

            // Get the XML for the Controller Map
            string xml = GetControllerMapXml(player, controllerIdentifier, categoryId, layoutId);
            if (string.IsNullOrEmpty(xml)) return null;

            ControllerMap controllerMap = ControllerMap.CreateFromXml(controllerIdentifier.controllerType, xml);         
            if (controllerMap == null) return null;

            // Load default mappings for new Actions
            List<int> knownActionIds = GetControllerMapKnownActionIds(player, controllerIdentifier, categoryId, layoutId);
            AddDefaultMappingsForNewActions(controllerIdentifier, controllerMap, knownActionIds);

            return controllerMap;
        }

        private int LoadInputBehaviors(int playerId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return 0;

            int count = 0;

            // All players have an instance of each input behavior so it can be modified
            IList<InputBehavior> behaviors = ReInput.mapping.GetInputBehaviors(player.id); // get all behaviors from player
            for(int i = 0; i < behaviors.Count; i++) {
                count += LoadInputBehaviorNow(player, behaviors[i]);
            }

            return count;
        }

        private int LoadInputBehaviorNow(int playerId, int behaviorId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return 0;

            InputBehavior behavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
            if(behavior == null) return 0;

            return LoadInputBehaviorNow(player, behavior);
        }
        private int LoadInputBehaviorNow(Player player, InputBehavior inputBehavior) {
            if(player == null || inputBehavior == null) return 0;

            string xml = GetInputBehaviorXml(player, inputBehavior.id); // try to the behavior for this id
            if(xml == null || xml == string.Empty) return 0; // no data found for this behavior
            return inputBehavior.ImportXmlString(xml) ? 1 : 0; // import the data into the behavior
        }

        private bool LoadControllerAssignmentsNow() {
            try {
                // Try to load assignment save data
                ControllerAssignmentSaveInfo data = LoadControllerAssignmentData();
                if(data == null) return false;

                // Load keyboard and mouse assignments
                if(loadKeyboardAssignments || loadMouseAssignments) {
                    LoadKeyboardAndMouseAssignmentsNow(data);
                }
                
                // Load joystick assignments
                if(loadJoystickAssignments) {
                    LoadJoystickAssignmentsNow(data);
                }

#if UNITY_EDITOR
                Debug.LogWarning(logPrefix + thisScriptName + " loaded controller assignments from PlayerPrefs.");
#endif
            } catch {
#if UNITY_EDITOR
                Debug.LogError(logPrefix + thisScriptName + " encountered an error loading controller assignments from PlayerPrefs.");
#endif
            }

            return true;
        }

        private bool LoadKeyboardAndMouseAssignmentsNow(ControllerAssignmentSaveInfo data) {
            try {
                // Try to load the save data
                if(data == null && (data = LoadControllerAssignmentData()) == null) return false;

                // Process each Player assigning controllers from the save data
                foreach(Player player in ReInput.players.AllPlayers) {
                    if(!data.ContainsPlayer(player.id)) continue;
                    ControllerAssignmentSaveInfo.PlayerInfo playerData = data.players[data.IndexOfPlayer(player.id)];

                    // Assign keyboard
                    if(loadKeyboardAssignments) {
                        player.controllers.hasKeyboard = playerData.hasKeyboard;
                    }

                    // Assign mouse
                    if(loadMouseAssignments) {
                        player.controllers.hasMouse = playerData.hasMouse;
                    }
                }
            } catch {
#if UNITY_EDITOR
                Debug.LogError(logPrefix + thisScriptName + " encountered an error loading keyboard and/or mouse assignments from PlayerPrefs.");
#endif
            }

            return true;
        }

        private bool LoadJoystickAssignmentsNow(ControllerAssignmentSaveInfo data) {
            try {
                if(ReInput.controllers.joystickCount == 0) return false; // no joysticks to assign

                // Try to load the save data
                if(data == null && (data = LoadControllerAssignmentData()) == null) return false;

                // Unassign all Joysticks first
                foreach(Player player in ReInput.players.AllPlayers) {
                    player.controllers.ClearControllersOfType(ControllerType.Joystick);
                }

                // Create a history which helps in assignment of imprecise matches back to the same Players
                // even when the same Joystick is assigned to multiple Players.
                List<JoystickAssignmentHistoryInfo> joystickHistory = loadJoystickAssignments ? new List<JoystickAssignmentHistoryInfo>() : null;

                // Process each Player assigning controllers from the save data
                foreach(Player player in ReInput.players.AllPlayers) {
                    if(!data.ContainsPlayer(player.id)) continue;
                    ControllerAssignmentSaveInfo.PlayerInfo playerData = data.players[data.IndexOfPlayer(player.id)];

                    // Assign joysticks
                    for(int i = 0; i < playerData.joystickCount; i++) {
                        ControllerAssignmentSaveInfo.JoystickInfo joystickInfo = playerData.joysticks[i];
                        if(joystickInfo == null) continue;

                        // Find a matching Joystick if any
                        Joystick joystick = FindJoystickPrecise(joystickInfo); // only assign joysticks with precise matching information
                        if(joystick == null) continue;

                        // Add the Joystick to the history
                        if(joystickHistory.Find(x => x.joystick == joystick) == null) {
                            joystickHistory.Add(new JoystickAssignmentHistoryInfo(joystick, joystickInfo.id));
                        }

                        // Assign the Joystick to the Player
                        player.controllers.AddController(joystick, false);
                    }
                }

                // Do another joystick assignment pass with imprecise matching info all precise matches are done.
                // This is done to make sure all the joysticks with exact matching info get assigned to the right Players
                // before assigning any joysticks with imprecise matching info to reduce the chances of a mis-assignment.
                // This is not allowed on all platforms to prevent issues with system player/id and Rewired Player alignment.

                if(allowImpreciseJoystickAssignmentMatching) {
                    foreach(Player player in ReInput.players.AllPlayers) {
                        if(!data.ContainsPlayer(player.id)) continue;
                        ControllerAssignmentSaveInfo.PlayerInfo playerData = data.players[data.IndexOfPlayer(player.id)];

                        for(int i = 0; i < playerData.joystickCount; i++) {
                            ControllerAssignmentSaveInfo.JoystickInfo joystickInfo = playerData.joysticks[i];
                            if(joystickInfo == null) continue;

                            Joystick joystick = null;

                            // Check assignment history for joystick first
                            int index = joystickHistory.FindIndex(x => x.oldJoystickId == joystickInfo.id);
                            if(index >= 0) { // found in history
                                joystick = joystickHistory[index].joystick; // just get the Joystick from the history
                            } else { // not in history, try to find otherwise

                                // Find all matching Joysticks excluding all Joysticks that have precise matching information available
                                List<Joystick> matches;
                                if(!TryFindJoysticksImprecise(joystickInfo, out matches)) continue; // no matches found

                                // Find the first Joystick that's not already in the history
                                foreach(Joystick match in matches) {
                                    if(joystickHistory.Find(x => x.joystick == match) != null) continue;
                                    joystick = match;
                                    break;
                                }
                                if(joystick == null) continue; // no suitable match found

                                // Add the Joystick to the history
                                joystickHistory.Add(new JoystickAssignmentHistoryInfo(joystick, joystickInfo.id));
                            }

                            // Assign the joystick to the Player
                            player.controllers.AddController(joystick, false);
                        }
                    }
                }
            } catch {
#if UNITY_EDITOR
                Debug.LogError(logPrefix + thisScriptName + " encountered an error loading joystick assignments from PlayerPrefs.");
#endif
            }

            // Auto-assign Joysticks in case save data doesn't include all attached Joysticks
            if(ReInput.configuration.autoAssignJoysticks) {
                ReInput.controllers.AutoAssignJoysticks();
            }

            return true;
        }

        private ControllerAssignmentSaveInfo LoadControllerAssignmentData() {
            try {
                // Check if there is any data saved
                if(!PlayerPrefs.HasKey(playerPrefsKey_controllerAssignments)) return null;

                // Load save data from the registry
                string json = PlayerPrefs.GetString(playerPrefsKey_controllerAssignments);
                if(string.IsNullOrEmpty(json)) return null;

                // Parse Json
                ControllerAssignmentSaveInfo data = JsonParser.FromJson<ControllerAssignmentSaveInfo>(json);
                if(data == null || data.playerCount == 0) return null; // no valid save data found

                return data;
            } catch {
                return null;
            }
        }

        private IEnumerator LoadJoystickAssignmentsDeferred() {
			deferredJoystickAssignmentLoadPending = true;

            yield return new WaitForEndOfFrame(); // defer until the end of the frame
            if(!ReInput.isReady) yield break; // in case Rewired was shut down

            // Load the joystick assignments
            if(LoadJoystickAssignmentsNow(null)) {
#if UNITY_EDITOR
                Debug.LogWarning(logPrefix + thisScriptName + " loaded joystick assignments from PlayerPrefs.");
#endif
            }

            // Save the controller assignments after loading in case anything has been
            // re-assigned to a different Player or a new joystick was connected.
            SaveControllerAssignments();

            deferredJoystickAssignmentLoadPending = false;
        }

        #endregion

        #region Save

        private void SaveAll() {

            // Save all data in all Players including System Player
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) {
                SavePlayerDataNow(allPlayers[i]);
            }

            // Save joystick calibration maps
            SaveAllJoystickCalibrationData();

            // Save controller assignments
            if(loadControllerAssignments) {
                SaveControllerAssignments();
            }

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }

        private void SavePlayerDataNow(int playerId) {
            SavePlayerDataNow(ReInput.players.GetPlayer(playerId));

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }
        private void SavePlayerDataNow(Player player) {
            if(player == null) return;

            // Get all savable data from player
            PlayerSaveData playerData = player.GetSaveData(true);

            // Save Input Behaviors
            SaveInputBehaviors(player, playerData);

            // Save controller maps
            SaveControllerMaps(player, playerData);
        }

        private void SaveAllJoystickCalibrationData() {
            // Save all calibration maps from all joysticks
            IList<Joystick> joysticks = ReInput.controllers.Joysticks;
            for(int i = 0; i < joysticks.Count; i++) {
                SaveJoystickCalibrationData(joysticks[i]);
            }
        }

        private void SaveJoystickCalibrationData(int joystickId) {
            SaveJoystickCalibrationData(ReInput.controllers.GetJoystick(joystickId));
        }
        private void SaveJoystickCalibrationData(Joystick joystick) {
            if(joystick == null) return;
            JoystickCalibrationMapSaveData saveData = joystick.GetCalibrationMapSaveData();
            string key = GetJoystickCalibrationMapPlayerPrefsKey(joystick);
            PlayerPrefs.SetString(key, saveData.map.ToXmlString()); // save the map to player prefs in XML format
        }

        private void SaveJoystickData(int joystickId) {
            // Save joystick maps in all Players for this joystick id
            IList<Player> allPlayers = ReInput.players.AllPlayers;
            for(int i = 0; i < allPlayers.Count; i++) { // this controller may be owned by more than one player, so check all
                Player player = allPlayers[i];
                if(!player.controllers.ContainsController(ControllerType.Joystick, joystickId)) continue; // player does not have the joystick

                // Save controller maps
                SaveControllerMaps(player.id, ControllerType.Joystick, joystickId);
            }

            // Save calibration data
            SaveJoystickCalibrationData(joystickId);
        }

        private void SaveControllerDataNow(int playerId, ControllerType controllerType, int controllerId) {

            // Save map data
            SaveControllerMaps(playerId, controllerType, controllerId);

            // Save other controller data
            SaveControllerDataNow(controllerType, controllerId);

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }
        private void SaveControllerDataNow(ControllerType controllerType, int controllerId) {

            // Save calibration data for joysticks
            if(controllerType == ControllerType.Joystick) {
                SaveJoystickCalibrationData(controllerId);
            }

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }

        private void SaveControllerMaps(Player player, PlayerSaveData playerSaveData) {
            foreach(ControllerMapSaveData saveData in playerSaveData.AllControllerMapSaveData) {
                SaveControllerMap(player, saveData.map);
            }
        }
        private void SaveControllerMaps(int playerId, ControllerType controllerType, int controllerId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return;

            // Save controller maps in this player for this controller id
            if(!player.controllers.ContainsController(controllerType, controllerId)) return; // player does not have the controller

            // Save controller maps
            ControllerMapSaveData[] saveData = player.controllers.maps.GetMapSaveData(controllerType, controllerId, true);
            if(saveData == null) return;

            for(int i = 0; i < saveData.Length; i++) {
                SaveControllerMap(player, saveData[i].map);
            }
        }

        private void SaveControllerMap(Player player, ControllerMap controllerMap) {

            // Save the Controller Map
            string key = GetControllerMapPlayerPrefsKey(player, controllerMap.controller.identifier, controllerMap.categoryId, controllerMap.layoutId, controllerMapPPKeyVersion);
            PlayerPrefs.SetString(key, controllerMap.ToXmlString()); // save the map to player prefs in XML format

            // Save the Action ids list for this Controller Map used to allow new Actions to be added to the
            // Rewired Input Manager and have the new mappings show up when saved data is loaded
            key = GetControllerMapKnownActionIdsPlayerPrefsKey(player, controllerMap.controller.identifier, controllerMap.categoryId, controllerMap.layoutId, controllerMapPPKeyVersion);
            PlayerPrefs.SetString(key, allActionIdsString);
        }

        private void SaveInputBehaviors(Player player, PlayerSaveData playerSaveData) {
            if(player == null) return;
            InputBehavior[] inputBehaviors = playerSaveData.inputBehaviors;
            for(int i = 0; i < inputBehaviors.Length; i++) {
                SaveInputBehaviorNow(player, inputBehaviors[i]);
            }
        }

        private void SaveInputBehaviorNow(int playerId, int behaviorId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if(player == null) return;

            InputBehavior behavior = ReInput.mapping.GetInputBehavior(playerId, behaviorId);
            if(behavior == null) return;

            SaveInputBehaviorNow(player, behavior);

            // Save changes to PlayerPrefs
            PlayerPrefs.Save();
        }
        private void SaveInputBehaviorNow(Player player, InputBehavior inputBehavior) {
            if(player == null || inputBehavior == null) return;

            string key = GetInputBehaviorPlayerPrefsKey(player, inputBehavior.id);
            PlayerPrefs.SetString(key, inputBehavior.ToXmlString()); // save the behavior to player prefs in XML format
        }

        private bool SaveControllerAssignments() {
            try {
                // Save a complete snapshot of controller assignments in all Players
                ControllerAssignmentSaveInfo allPlayerData = new ControllerAssignmentSaveInfo(ReInput.players.allPlayerCount);

                for(int i = 0; i < ReInput.players.allPlayerCount; i++) {
                    Player player = ReInput.players.AllPlayers[i];

                    ControllerAssignmentSaveInfo.PlayerInfo playerData = new ControllerAssignmentSaveInfo.PlayerInfo();
                    allPlayerData.players[i] = playerData;

                    playerData.id = player.id;

                    // Add has keyboard
                    playerData.hasKeyboard = player.controllers.hasKeyboard;

                    // Add has mouse
                    playerData.hasMouse = player.controllers.hasMouse;

                    // Add joysticks
                    ControllerAssignmentSaveInfo.JoystickInfo[] joystickInfos = new ControllerAssignmentSaveInfo.JoystickInfo[player.controllers.joystickCount];
                    playerData.joysticks = joystickInfos;
                    for(int j = 0; j < player.controllers.joystickCount; j++) {
                        Joystick joystick = player.controllers.Joysticks[j];

                        ControllerAssignmentSaveInfo.JoystickInfo joystickInfo = new ControllerAssignmentSaveInfo.JoystickInfo();

                        // Record the device instance id.
                        joystickInfo.instanceGuid = joystick.deviceInstanceGuid;

                        // Record the joystick id for joysticks with only imprecise information so we can use this
                        // to determine if the same joystick was assigned to multiple Players.
                        joystickInfo.id = joystick.id;

                        // Record the hardware identifier string.
                        joystickInfo.hardwareIdentifier = joystick.hardwareIdentifier;

                        // Store the info
                        joystickInfos[j] = joystickInfo;
                    }
                }

                // Save to PlayerPrefs
                PlayerPrefs.SetString(playerPrefsKey_controllerAssignments, JsonWriter.ToJson(allPlayerData));
                PlayerPrefs.Save();

#if UNITY_EDITOR
                Debug.Log(logPrefix + thisScriptName + " saved controller assignments to PlayerPrefs.");
#endif
            } catch {
#if UNITY_EDITOR
                Debug.LogError(logPrefix + thisScriptName + " encountered an error saving controller assignments to PlayerPrefs.");
#endif
            }
            return true;
        }

        #endregion

        private bool ControllerAssignmentSaveDataExists() {
            // Check if there is any data saved
            if(!PlayerPrefs.HasKey(playerPrefsKey_controllerAssignments)) return false;

            // Load save data from the registry
            string json = PlayerPrefs.GetString(playerPrefsKey_controllerAssignments);
            if(string.IsNullOrEmpty(json)) return false;

            return true;
        }

        #region PlayerPrefs Methods

        /* NOTE ON PLAYER PREFS:
         * PlayerPrefs on Windows Standalone is saved in the registry. There is a bug in Regedit that makes any entry with a name equal to or greater than 255 characters
         * (243 + 12 unity appends) invisible in Regedit. Unity will still load the data fine, but if you are debugging and wondering why your data is not showing up in
         * Regedit, this is why. If you need to delete the values, either call PlayerPrefs.Clear or delete the key folder in Regedit -- Warning: both methods will
         * delete all player prefs including any ones you've created yourself or other plugins have created.
         */

        // WARNING: Do not use & symbol in keys. Linux cannot load them after the current session ends.

        private string GetBasePlayerPrefsKey(Player player) {
            string key = playerPrefsKeyPrefix;
            key += "|playerName=" + player.name; // make a key for this specific player, could use id, descriptive name, or a custom profile identifier of your choice
            return key;
        }

        private string GetControllerMapPlayerPrefsKey(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion) {
            // Create a player prefs key like a web querystring so we can search for player prefs key when loading maps
            string key = GetBasePlayerPrefsKey(player);
            key += "|dataType=ControllerMap";
            key += GetControllerMapPlayerPrefsKeyCommonSuffix(player, controllerIdentifier, categoryId, layoutId, ppKeyVersion);
            return key;
        }

        private string GetControllerMapKnownActionIdsPlayerPrefsKey(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion) {
            // Create a player prefs key like a web querystring so we can search for player prefs key when loading maps
            string key = GetBasePlayerPrefsKey(player);
            key += "|dataType=ControllerMap_KnownActionIds";
            key += GetControllerMapPlayerPrefsKeyCommonSuffix(player, controllerIdentifier, categoryId, layoutId, ppKeyVersion);
            return key;
        }

        private static string GetControllerMapPlayerPrefsKeyCommonSuffix(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId, int ppKeyVersion) {
            string key = "";
            if (ppKeyVersion >= controllerMapPPKeyVersion_includeFormatVersion) key += "|kv=" + ppKeyVersion; // include the key version in the string
            key += "|controllerMapType=" + GetControllerMapType(controllerIdentifier.controllerType).Name;
            key += "|categoryId=" + categoryId + "|" + "layoutId=" + layoutId;

            // Choose the key based on the key format version id
            if (ppKeyVersion >= controllerMapPPKeyVersion_supportDisconnectedControllers) {

                // Added in Rewired 1.1.27.0 to support loading controller maps for disconnected controllers
                key += "|hardwareGuid=" + controllerIdentifier.hardwareTypeGuid; // the identifying GUID that determines which known controller this is
                if (controllerIdentifier.hardwareTypeGuid == Guid.Empty) { // not recognized, Hardware Idenfitier is required
                    // This is no longer included for recognized controllers because it makes it impossible to lookup the map when the controller is not attached because the hardware identifier cannot be known without the device present.
                    key += "|hardwareIdentifier=" + controllerIdentifier.hardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
                }
                if (controllerIdentifier.controllerType == ControllerType.Joystick) { // store special info for joystick maps
                    key += "|duplicate=" + GetDuplicateIndex(player, controllerIdentifier).ToString();
                }

            } else {

                // Old version prior to 1.1.27.0
                key += "|hardwareIdentifier=" + controllerIdentifier.hardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
                if (controllerIdentifier.controllerType == ControllerType.Joystick) { // store special info for joystick maps
                    key += "|hardwareGuid=" + controllerIdentifier.hardwareTypeGuid; // the identifying GUID that determines which known joystick this is
                    // Added in Rewired 1.1.19.0
                    if (ppKeyVersion >= controllerMapPPKeyVersion_includeDuplicateJoystickIndex) key += "|duplicate=" + GetDuplicateIndex(player, controllerIdentifier).ToString();
                }
            }
            return key;
        }

        private string GetJoystickCalibrationMapPlayerPrefsKey(Joystick joystick) {
            // Create a player prefs key like a web querystring so we can search for player prefs key when loading maps
            string key = playerPrefsKeyPrefix;
            key += "|dataType=CalibrationMap";
            key += "|controllerType=" + joystick.type.ToString();
            key += "|hardwareIdentifier=" + joystick.hardwareIdentifier; // the hardware identifier string helps us identify maps for unknown hardware because it doesn't have a Guid
            key += "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
            return key;
        }

        private string GetInputBehaviorPlayerPrefsKey(Player player, int inputBehaviorId) {
            // Create a player prefs key like a web querystring so we can search for player prefs key when loading maps
            string key = GetBasePlayerPrefsKey(player);
            key += "|dataType=InputBehavior";
            key += "|id=" + inputBehaviorId;
            return key;
        }

        private string GetControllerMapXml(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId) {
            string key;
            // Must try many times because of new additions in various versions
            for(int i = controllerMapPPKeyVersion; i >= 0; i--) {
                key = GetControllerMapPlayerPrefsKey(player, controllerIdentifier, categoryId, layoutId, i);
                if (PlayerPrefs.HasKey(key)) return PlayerPrefs.GetString(key); // return the data
            }
            return null;
        }

        private List<int> GetControllerMapKnownActionIds(Player player, ControllerIdentifier controllerIdentifier, int categoryId, int layoutId) {
            List<int> actionIds = new List<int>();
            string key = null;
            bool found = false;

            // Must try many times because of new additions in various versions
            for(int i = controllerMapPPKeyVersion; i >= 0; i--) {
                key = GetControllerMapKnownActionIdsPlayerPrefsKey(player, controllerIdentifier, categoryId, layoutId, i);
                if (PlayerPrefs.HasKey(key)) {
                    found = true;
                    break;
                }
            }
            if(!found) return actionIds; // key does not exist

            // Get the data and try to parse it
            string data = PlayerPrefs.GetString(key);
            if(string.IsNullOrEmpty(data)) return actionIds;

            string[] split = data.Split(',');
            for(int i = 0; i < split.Length; i++) {
                if(string.IsNullOrEmpty(split[i])) continue;
                int id;
                if(int.TryParse(split[i], out id)) {
                    actionIds.Add(id);
                }
            }
            return actionIds;
        }

        private string GetJoystickCalibrationMapXml(Joystick joystick) {
            string key = GetJoystickCalibrationMapPlayerPrefsKey(joystick);
            if(!PlayerPrefs.HasKey(key)) return string.Empty; // key does not exist
            return PlayerPrefs.GetString(key); // return the data
        }

        private string GetInputBehaviorXml(Player player, int id) {
            string key = GetInputBehaviorPlayerPrefsKey(player, id);
            if(!PlayerPrefs.HasKey(key)) return string.Empty; // key does not exist
            return PlayerPrefs.GetString(key); // return the data
        }

        #endregion

        #region Misc

        private void AddDefaultMappingsForNewActions(ControllerIdentifier controllerIdentifier, ControllerMap controllerMap, List<int> knownActionIds) {
            if (controllerMap == null || knownActionIds == null) return;
            if (knownActionIds == null || knownActionIds.Count == 0) return;

            // Check for new Actions added to the default mappings that didn't exist when the Controller Map was saved

            // Load default map for comparison
            ControllerMap defaultMap = ReInput.mapping.GetControllerMapInstance(controllerIdentifier, controllerMap.categoryId, controllerMap.layoutId);
            if (defaultMap == null) return;

            // Find any new Action ids that didn't exist when the Controller Map was saved
            List<int> unknownActionIds = new List<int>();
            foreach (int id in allActionIds) {
                if (knownActionIds.Contains(id)) continue;
                unknownActionIds.Add(id);
            }

            if (unknownActionIds.Count == 0) return; // no new Action ids

            // Add all mappings in the default map for previously unknown Action ids
            foreach (ActionElementMap aem in defaultMap.AllMaps) {
                if (!unknownActionIds.Contains(aem.actionId)) continue;

                // Skip this ActionElementMap if there's a conflict within the loaded map
                if (controllerMap.DoesElementAssignmentConflict(aem)) continue;

                // Create an assignment
                ElementAssignment assignment = new ElementAssignment(
                    controllerMap.controllerType,
                    aem.elementType,
                    aem.elementIdentifierId,
                    aem.axisRange,
                    aem.keyCode,
                    aem.modifierKeyFlags,
                    aem.actionId,
                    aem.axisContribution,
                    aem.invert
                );

                // Assign it
                controllerMap.CreateElementMap(assignment);
            }
        }

        private Joystick FindJoystickPrecise(ControllerAssignmentSaveInfo.JoystickInfo joystickInfo) {
            if(joystickInfo == null) return null;
            if(joystickInfo.instanceGuid == Guid.Empty) return null; // do not handle invalid instance guids

            // Find a matching joystick
            IList<Joystick> joysticks = ReInput.controllers.Joysticks;
            for(int i = 0; i < joysticks.Count; i++) {
                if(joysticks[i].deviceInstanceGuid == joystickInfo.instanceGuid) return joysticks[i];
            }

            return null;
        }

        private bool TryFindJoysticksImprecise(ControllerAssignmentSaveInfo.JoystickInfo joystickInfo, out List<Joystick> matches) {
            matches = null;
            if(joystickInfo == null) return false;
            if(string.IsNullOrEmpty(joystickInfo.hardwareIdentifier)) return false; // do not handle invalid hardware identifiers

            // Find a matching joystick
            IList<Joystick> joysticks = ReInput.controllers.Joysticks;
            for(int i = 0; i < joysticks.Count; i++) {
                if(string.Equals(joysticks[i].hardwareIdentifier, joystickInfo.hardwareIdentifier, StringComparison.OrdinalIgnoreCase)) {
                    if(matches == null) matches = new List<Joystick>();
                    matches.Add(joysticks[i]);
                }
            }
            return matches != null;
        }

        private static int GetDuplicateIndex(Player player, ControllerIdentifier controllerIdentifier) {
            // Determine how many duplicates of this controller are owned by this Player
            Controller controller = ReInput.controllers.GetController(controllerIdentifier);
            if (controller == null) return 0; // cannot support index count if the controller is not connected
            int duplicateCount = 0;
            foreach(var c in player.controllers.Controllers) {
                if(c.type != controller.type) continue;
                bool isRecognized = false;
                if(controller.type == ControllerType.Joystick) {
                    if((c as Joystick).hardwareTypeGuid != controller.hardwareTypeGuid) continue;
                    if(controller.hardwareTypeGuid != Guid.Empty) isRecognized = true;
                }
                if(!isRecognized && c.hardwareIdentifier != controller.hardwareIdentifier) continue;
                if(c == controller) return duplicateCount;
                duplicateCount++;
            }
            return duplicateCount;
        }

        private void RefreshLayoutManager(int playerId) {
            Player player = ReInput.players.GetPlayer(playerId);
            if (player == null) return;
            player.controllers.maps.layoutManager.Apply();
        }

        private static Type GetControllerMapType(ControllerType controllerType) {
            switch(controllerType) {
                case ControllerType.Custom: return typeof(CustomControllerMap);
                case ControllerType.Joystick: return typeof(JoystickMap);
                case ControllerType.Keyboard: return typeof(KeyboardMap);
                case ControllerType.Mouse: return typeof(MouseMap);
                default:
                    Debug.LogWarning(logPrefix + "Unknown ControllerType " + controllerType.ToString());
                    return null;
            }
        }

        #endregion

        #region Classes

        private class ControllerAssignmentSaveInfo {

            public PlayerInfo[] players;

            public int playerCount { get { return players != null ? players.Length : 0; } }

            public ControllerAssignmentSaveInfo() {
            }
            public ControllerAssignmentSaveInfo(int playerCount) {
                this.players = new PlayerInfo[playerCount];
                for(int i = 0; i < playerCount; i++) {
                    players[i] = new PlayerInfo();
                }
            }

            public int IndexOfPlayer(int playerId) {
                for(int i = 0; i < playerCount; i++) {
                    if(players[i] == null) continue;
                    if(players[i].id == playerId) return i;
                }
                return -1;
            }

            public bool ContainsPlayer(int playerId) {
                return IndexOfPlayer(playerId) >= 0;
            }

            public class PlayerInfo {

                public int id;
                public bool hasKeyboard;
                public bool hasMouse;
                public JoystickInfo[] joysticks;

                public int joystickCount { get { return joysticks != null ? joysticks.Length : 0; } }

                public int IndexOfJoystick(int joystickId) {
                    for(int i = 0; i < joystickCount; i++) {
                        if(joysticks[i] == null) continue;
                        if(joysticks[i].id == joystickId) return i;
                    }
                    return -1;
                }

                public bool ContainsJoystick(int joystickId) {
                    return IndexOfJoystick(joystickId) >= 0;
                }
            }

            public class JoystickInfo {
                public Guid instanceGuid;
                public string hardwareIdentifier;
                public int id;
            }
        }

        private class JoystickAssignmentHistoryInfo {

            public readonly Joystick joystick;
            public readonly int oldJoystickId;

            public JoystickAssignmentHistoryInfo(Joystick joystick, int oldJoystickId) {
                if(joystick == null) throw new ArgumentNullException("joystick");
                this.joystick = joystick;
                this.oldJoystickId = oldJoystickId;
            }
        }

        #endregion
    }
}