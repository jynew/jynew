#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;
using XNode;

namespace XNodeEditor {
	public class InputAttributeDrawer : OdinAttributeDrawer<XNode.Node.InputAttribute> {
		protected override bool CanDrawAttributeProperty(InspectorProperty property) {
			Node node = property.Tree.WeakTargets[0] as Node;
			return node != null;
		}

		protected override void DrawPropertyLayout(GUIContent label) {
			Node node = Property.Tree.WeakTargets[0] as Node;
			NodePort port = node.GetInputPort(Property.Name);

			if (!NodeEditor.inNodeEditor) {
				if (Attribute.backingValue == XNode.Node.ShowBackingValue.Always || Attribute.backingValue == XNode.Node.ShowBackingValue.Unconnected && !port.IsConnected)
					CallNextDrawer(label);
				return;
			}

			if (Property.Tree.WeakTargets.Count > 1) {
				SirenixEditorGUI.WarningMessageBox("Cannot draw ports with multiple nodes selected");
				return;
			}

			if (port != null) {
				var portPropoerty = Property.Tree.GetUnityPropertyForPath(Property.UnityPropertyPath);
				if (portPropoerty == null) {
					SirenixEditorGUI.ErrorMessageBox("Port property missing at: " + Property.UnityPropertyPath);
					return;
				} else {
					var labelWidth = Property.GetAttribute<LabelWidthAttribute>();
					if (labelWidth != null)
						GUIHelper.PushLabelWidth(labelWidth.Width);

					NodeEditorGUILayout.PropertyField(portPropoerty, label == null ? GUIContent.none : label, true, GUILayout.MinWidth(30));

					if (labelWidth != null)
						GUIHelper.PopLabelWidth();
				}
			}
		}
	}
}
#endif