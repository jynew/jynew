#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using XNode;

namespace XNodeEditor {
	internal class OdinNodeInGraphAttributeProcessor<T> : OdinAttributeProcessor<T> where T : Node {
		public override bool CanProcessSelfAttributes(InspectorProperty property) {
			return false;
		}

		public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member) {
			if (!NodeEditor.inNodeEditor)
				return false;

			if (member.MemberType == MemberTypes.Field) {
				switch (member.Name) {
					case "graph":
					case "position":
					case "ports":
						return true;

					default:
						break;
				}
			}

			return false;
		}

		public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {
			switch (member.Name) {
				case "graph":
				case "position":
				case "ports":
					attributes.Add(new HideInInspector());
					break;

				default:
					break;
			}
		}
	}
}
#endif