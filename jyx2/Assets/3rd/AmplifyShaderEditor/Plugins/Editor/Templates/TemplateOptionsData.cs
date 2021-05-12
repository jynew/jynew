// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	/*ase_pass_options OLDEST
	DefineOnConnected:portId:definevalue	
	DefineOnUnconnected:portId:definevalue
	Options:name:defaultOption:opt0:opt1:opt2
	SetVisible:PortId:OptionName:OptionValue
	*/

	/*ase_pass_options OLD
	Option:Option Name:UI Type:Default:Item0,Item1,Item3...ItemN
	Action:Action Type:Action Data:ConditionA && ConditionB || ConditionC:
	*/

	/*ase_pass_options:UniqueId:PropagateDataToHiddenPasses
	Option:Color Offset:A,B,C:A
		A:ShowPort:My Port Name
		B,C:HidePort:My Port Name
		B:SetDefine:MY_DEFINE
		C:SetDefine:MY_COLOR_DEFINE
	Option:My Other Option:True,False
		True:ShowOption:Color Offset
		False:HideOption:Color Offset
	Port:My Port Name
		On:SetDefine:MY_COLOR_DEFINE
		Off:UnsetDefine:MY_COLOR_DEFINE
	*/
	public enum AseOptionsUIWidget
	{
		Dropdown,
		Toggle
	}

	public enum AseOptionsType
	{
		Option,
		Port
	}

	public enum AseOptionsActionType
	{
		ShowOption,
		HideOption,
		HidePort,
		ShowPort,
		SetDefine,
		UnsetDefine,
		ExcludePass
	}

	public enum AseOptionsSetup
	{
		CopyOptionsFromMainPass,
		Id,
		Name
	}

	[Serializable]
	public class TemplateActionItem
	{
		public AseOptionsActionType ActionType;
		public string ActionData = string.Empty;
		public int ActionDataIdx = -1;
		public override string ToString()
		{
			return ActionType + " " + ActionData + " " + ActionDataIdx;
		}
	}

	[Serializable]
	public class TemplateActionItemGrid
	{
		[Serializable]
		public class TemplateActionItemRow
		{
			public TemplateActionItem[] Columns;
		}

		public TemplateActionItemRow[] Rows;

		public TemplateActionItemGrid( int rowsCount )
		{
			Rows = new TemplateActionItemRow[ rowsCount ];
		} 

		public TemplateActionItem this[ int row, int column ]
		{
			get { return Rows[ row ].Columns[ column ]; }
			set { Rows[ row ].Columns[ column ] = value; }
		}

		public TemplateActionItem[] this[ int row ]
		{
			get { return Rows[ row ].Columns; }

			set
			{
				if( Rows[ row ] == null )
					Rows[ row ] = new TemplateActionItemRow();

				Rows[ row ].Columns = value;
			}
		}
	}

	[Serializable]
	public class TemplateOptionsItem
	{
		public AseOptionsType Type;
		public AseOptionsUIWidget UIWidget;

		public string Id = string.Empty;
		public string Name = string.Empty;
		public string DefaultOption = string.Empty;
		public string[] Options = null;

		public TemplateActionItemGrid ActionsPerOption = null;

		[SerializeField]
		private int m_defaultOptionIndex = -1;

		public int OptionIndexFor( string option )
		{
			for( int i = 0; i < Options.Length; i++ )
			{
				if( Options[ i ].Equals( option ) )
				{
					return i;
				}
			}
			Debug.LogWarning( "Couldn't find index for option: " + option );
			return 0;
		}

		public int DefaultOptionIndex
		{
			get
			{
				if( m_defaultOptionIndex > -1 )
					return m_defaultOptionIndex;

				for( int i = 0; i < Options.Length; i++ )
				{
					if( Options[ i ].Equals( DefaultOption ) )
					{
						m_defaultOptionIndex = i;
						return i;
					}
				}
				Debug.LogWarning( "Couldn't find index for default option: " + DefaultOption );
				return 0;
			}
		}
	}

	[Serializable]
	public class TemplateOptionsContainer
	{
		public bool Enabled = false;
		public string Body = string.Empty;
		public int Index = -1;
		public int Id = -1;
		public string Name = string.Empty;
		public bool CopyOptionsFromMainPass = false;
		public TemplateOptionsItem[] Options = null;
	}

	public class TemplateOptionsToolsHelper
	{
		public const string PassOptionsMainPattern = @"\/\*ase_pass_options:([\w:= ]*)[\n]([\w: \t;\n&|,_]*)\*\/";
		public static Dictionary<string, AseOptionsSetup> AseOptionsSetupDict = new Dictionary<string, AseOptionsSetup>()
		{
			{ "CopyOptionsFromMainPass",AseOptionsSetup.CopyOptionsFromMainPass},
			{ "Id",AseOptionsSetup.Id},
			{ "Name",AseOptionsSetup.Name},
		};

		public static Dictionary<string, AseOptionsUIWidget> AseOptionsUITypeDict = new Dictionary<string, AseOptionsUIWidget>()
		{
			{ "Dropdown",AseOptionsUIWidget.Dropdown },
			{ "Toggle", AseOptionsUIWidget.Toggle }
		};

		public static Dictionary<string, AseOptionsActionType> AseOptionsActionTypeDict = new Dictionary<string, AseOptionsActionType>()
		{
			{"ShowOption",  AseOptionsActionType.ShowOption },
			{"HideOption",  AseOptionsActionType.HideOption },
			{"HidePort",    AseOptionsActionType.HidePort },
			{"ShowPort",    AseOptionsActionType.ShowPort },
			{"SetDefine",   AseOptionsActionType.SetDefine },
			{"UnsetDefine", AseOptionsActionType.UnsetDefine },
			{"ExcludePass", AseOptionsActionType.ExcludePass }
		};

		public static TemplateOptionsContainer GenerateOptionsContainer( string data )
		{
			TemplateOptionsContainer optionsContainer = new TemplateOptionsContainer();

			Match match = Regex.Match( data, PassOptionsMainPattern );
			optionsContainer.Enabled = match.Success;
			if( match.Success )
			{
				optionsContainer.Body = match.Value;
				optionsContainer.Index = match.Index;

				List<TemplateOptionsItem> optionItemsList = new List<TemplateOptionsItem>();
				List<List<TemplateActionItem>> actionItemsList = new List<List<TemplateActionItem>>();
				Dictionary<string, int> optionItemToIndex = new Dictionary<string, int>();
				TemplateOptionsItem currentOption = null;

				//OPTIONS OVERALL SETUP
				string[] setupLines = match.Groups[ 1 ].Value.Split( ':' );
				for( int i = 0; i < setupLines.Length; i++ )
				{
					if( AseOptionsSetupDict.ContainsKey( setupLines[ i ] ) )
					{
						AseOptionsSetup setup = AseOptionsSetupDict[ setupLines[ i ] ];
						switch( setup )
						{
							case AseOptionsSetup.CopyOptionsFromMainPass: optionsContainer.CopyOptionsFromMainPass = true; break;
						}
					}
					else
					{
						string[] args = setupLines[ i ].Split( '=' );
						if( args.Length > 1 && AseOptionsSetupDict.ContainsKey( args[ 0 ] ) )
						{
							AseOptionsSetup setup = AseOptionsSetupDict[ args[ 0 ] ];
							switch( setup )
							{
								case AseOptionsSetup.Id:  int.TryParse(args[1], out optionsContainer.Id ) ; break;
								case AseOptionsSetup.Name: optionsContainer.Name = args[1]; break;
							}
						}
					}
				}

				//AVAILABLE OPTIONS
				string body = match.Groups[ 2 ].Value.Replace( "\t", string.Empty );
				string[] optionLines = body.Split( '\n' );
				for( int oL = 0; oL < optionLines.Length; oL++ )
				{
					string[] optionItems = optionLines[ oL ].Split( ':' );
					if( optionItems.Length > 0 )
					{
						string[] itemIds = optionItems[ 0 ].Split( ',' );
						switch( itemIds[0] )
						{
							case "Option":
							{
								//Fills previous option with its actions
								//actionItemsList is cleared over here
								FillOptionAction( currentOption, ref actionItemsList );

								optionItemToIndex.Clear();
								currentOption = new TemplateOptionsItem();
								currentOption.Type = AseOptionsType.Option;
								currentOption.Name = optionItems[ 1 ];
								currentOption.Id = itemIds.Length > 1? itemIds[1]:optionItems[ 1 ];
								currentOption.Options = optionItems[ 2 ].Split( ',' );
								for( int opIdx = 0; opIdx < currentOption.Options.Length; opIdx++ )
								{
									optionItemToIndex.Add( currentOption.Options[ opIdx ], opIdx );
									actionItemsList.Add( new List<TemplateActionItem>() );
								}
								if( optionItems.Length > 3 )
								{
									currentOption.DefaultOption = optionItems[ 3 ];
								}
								else
								{
									currentOption.DefaultOption = currentOption.Options[ 0 ];
								}

								if( currentOption.Options.Length > 2 )
								{
									currentOption.UIWidget = AseOptionsUIWidget.Dropdown;
								}
								else if( currentOption.Options.Length == 2 )
								{
									if( ( currentOption.Options[ 0 ].Equals( "true" ) && currentOption.Options[ 1 ].Equals( "false" ) ) ||
										( currentOption.Options[ 0 ].Equals( "false" ) && currentOption.Options[ 1 ].Equals( "true" ) ) )
									{
										currentOption.UIWidget = AseOptionsUIWidget.Toggle;
									}
								}
								else
								{
									Debug.LogWarning( "Detected an option with less than two items:" + optionItems[ 1 ] );
								}
								optionItemsList.Add( currentOption );
							}
							break;
							case "Port":
							{
								//Fills previous option with its actions
								//actionItemsList is cleared over here
								FillOptionAction( currentOption, ref actionItemsList );

								optionItemToIndex.Clear();

								currentOption = new TemplateOptionsItem();
								currentOption.Type = AseOptionsType.Port;
								currentOption.Name = optionItems[ 1 ];

								currentOption.Options = new string[] { "On", "Off" };
								optionItemToIndex.Add( currentOption.Options[ 0 ], 0 );
								optionItemToIndex.Add( currentOption.Options[ 1 ], 1 );

								actionItemsList.Add( new List<TemplateActionItem>() );
								actionItemsList.Add( new List<TemplateActionItem>() );
								
								optionItemsList.Add( currentOption );
							}
							break;
							default:
							{
								if( optionItemToIndex.ContainsKey( optionItems[ 0 ] ) )
								{
									int idx = 0;
									if( currentOption != null && currentOption.UIWidget == AseOptionsUIWidget.Toggle )
									{
										idx = ( optionItems[ 0 ].Equals( "true" ) ) ? 1 : 0;
									}
									else
									{
										idx = optionItemToIndex[ optionItems[ 0 ] ];
									}
									actionItemsList[ idx ].Add( CreateActionItem( optionItems ) );
								}
								else
								{
									//string[] ids = optionItems[ 0 ].Split( ',' );
									if( itemIds.Length > 1 )
									{
										for( int i = 0; i < itemIds.Length; i++ )
										{
											if( optionItemToIndex.ContainsKey( itemIds[ i ] ) )
											{
												int idx = optionItemToIndex[ itemIds[ i ] ];
												actionItemsList[ idx ].Add( CreateActionItem( optionItems ) );
											}
										}
									}
								}

							}
							break;
						}
					}
				}

				//Fills last option with its actions
				FillOptionAction( currentOption, ref actionItemsList );

				actionItemsList.Clear();
				actionItemsList = null;

				optionsContainer.Options = optionItemsList.ToArray();
				optionItemsList.Clear();
				optionItemsList = null;

				optionItemToIndex.Clear();
				optionItemToIndex = null;
			}
			return optionsContainer;
		}

		static void FillOptionAction( TemplateOptionsItem currentOption, ref List<List<TemplateActionItem>> actionItemsList )
		{
			if( currentOption != null )
			{
				currentOption.ActionsPerOption = new TemplateActionItemGrid( actionItemsList.Count );
				for( int i = 0; i < actionItemsList.Count; i++ )
				{
					currentOption.ActionsPerOption[ i ] = actionItemsList[ i ].ToArray();
					actionItemsList[ i ].Clear();
				}
				actionItemsList.Clear();
			}
		}

		static TemplateActionItem CreateActionItem( string[] optionItems )
		{
			TemplateActionItem actionItem = new TemplateActionItem();
			actionItem.ActionType = AseOptionsActionTypeDict[ optionItems[ 1 ] ];
			actionItem.ActionData = optionItems[ 2 ];
			return actionItem;
		}
	}
}
