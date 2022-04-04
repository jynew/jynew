using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Key", "Name", "Sex", "Level", "Exp", "Attack", "Qinggong", "Defence", "Heal", "UsePoison", "DePoison", "AntiPoison", "Quanzhang", "Yujian", "Shuadao", "Qimen", "Anqi", "Wuxuechangshi", "Pinde", "AttackPoison", "Zuoyouhubo", "Shengwang", "IQ", "ExpForItem", "Wugongs", "Items", "Mp", "MaxMp", "MpType", "Hp", "MaxHp", "Hurt", "Poison", "Tili", "ExpForMakeItem", "Weapon", "Armor", "Xiulianwupin", "CurrentSkill", "HpInc")]
	public class ES3UserType_RoleInstance : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_RoleInstance() : base(typeof(Jyx2.RoleInstance)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Jyx2.RoleInstance)obj;
			
			writer.WriteProperty("Key", instance.Key, ES3Type_int.Instance);
			writer.WriteProperty("Name", instance.Name, ES3Type_string.Instance);
			writer.WriteProperty("Sex", instance.Sex, ES3Type_int.Instance);
			writer.WriteProperty("Level", instance.Level, ES3Type_int.Instance);
			writer.WriteProperty("Exp", instance.Exp, ES3Type_int.Instance);
			writer.WriteProperty("Attack", instance.Attack, ES3Type_int.Instance);
			writer.WriteProperty("Qinggong", instance.Qinggong, ES3Type_int.Instance);
			writer.WriteProperty("Defence", instance.Defence, ES3Type_int.Instance);
			writer.WriteProperty("Heal", instance.Heal, ES3Type_int.Instance);
			writer.WriteProperty("UsePoison", instance.UsePoison, ES3Type_int.Instance);
			writer.WriteProperty("DePoison", instance.DePoison, ES3Type_int.Instance);
			writer.WriteProperty("AntiPoison", instance.AntiPoison, ES3Type_int.Instance);
			writer.WriteProperty("Quanzhang", instance.Quanzhang, ES3Type_int.Instance);
			writer.WriteProperty("Yujian", instance.Yujian, ES3Type_int.Instance);
			writer.WriteProperty("Shuadao", instance.Shuadao, ES3Type_int.Instance);
			writer.WriteProperty("Qimen", instance.Qimen, ES3Type_int.Instance);
			writer.WriteProperty("Anqi", instance.Anqi, ES3Type_int.Instance);
			writer.WriteProperty("Wuxuechangshi", instance.Wuxuechangshi, ES3Type_int.Instance);
			writer.WriteProperty("Pinde", instance.Pinde, ES3Type_int.Instance);
			writer.WriteProperty("AttackPoison", instance.AttackPoison, ES3Type_int.Instance);
			writer.WriteProperty("Zuoyouhubo", instance.Zuoyouhubo, ES3Type_int.Instance);
			writer.WriteProperty("Shengwang", instance.Shengwang, ES3Type_int.Instance);
			writer.WriteProperty("IQ", instance.IQ, ES3Type_int.Instance);
			writer.WriteProperty("ExpForItem", instance.ExpForItem, ES3Type_int.Instance);
			writer.WriteProperty("Wugongs", instance.Wugongs, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.List<Jyx2.SkillInstance>)));
			writer.WriteProperty("Items", instance.Items, ES3Internal.ES3TypeMgr.GetES3Type(typeof(System.Collections.Generic.List<Jyx2Configs.Jyx2ConfigCharacterItem>)));
			writer.WriteProperty("Mp", instance.Mp, ES3Type_int.Instance);
			writer.WriteProperty("MaxMp", instance.MaxMp, ES3Type_int.Instance);
			writer.WriteProperty("MpType", instance.MpType, ES3Type_int.Instance);
			writer.WriteProperty("Hp", instance.Hp, ES3Type_int.Instance);
			writer.WriteProperty("MaxHp", instance.MaxHp, ES3Type_int.Instance);
			writer.WriteProperty("Hurt", instance.Hurt, ES3Type_int.Instance);
			writer.WriteProperty("Poison", instance.Poison, ES3Type_int.Instance);
			writer.WriteProperty("Tili", instance.Tili, ES3Type_int.Instance);
			writer.WriteProperty("ExpForMakeItem", instance.ExpForMakeItem, ES3Type_int.Instance);
			writer.WriteProperty("Weapon", instance.Weapon, ES3Type_int.Instance);
			writer.WriteProperty("Armor", instance.Armor, ES3Type_int.Instance);
			writer.WriteProperty("Xiulianwupin", instance.Xiulianwupin, ES3Type_int.Instance);
			writer.WriteProperty("CurrentSkill", instance.CurrentSkill, ES3Type_int.Instance);
			writer.WriteProperty("HpInc", instance.HpInc, ES3Type_int.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Jyx2.RoleInstance)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Key":
						instance.Key = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Name":
						instance.Name = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "Sex":
						instance.Sex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Level":
						instance.Level = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Exp":
						instance.Exp = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Attack":
						instance.Attack = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Qinggong":
						instance.Qinggong = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Defence":
						instance.Defence = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Heal":
						instance.Heal = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "UsePoison":
						instance.UsePoison = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "DePoison":
						instance.DePoison = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "AntiPoison":
						instance.AntiPoison = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Quanzhang":
						instance.Quanzhang = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Yujian":
						instance.Yujian = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Shuadao":
						instance.Shuadao = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Qimen":
						instance.Qimen = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Anqi":
						instance.Anqi = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Wuxuechangshi":
						instance.Wuxuechangshi = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Pinde":
						instance.Pinde = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "AttackPoison":
						instance.AttackPoison = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Zuoyouhubo":
						instance.Zuoyouhubo = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Shengwang":
						instance.Shengwang = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "IQ":
						instance.IQ = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "ExpForItem":
						instance.ExpForItem = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Wugongs":
						instance.Wugongs = reader.Read<System.Collections.Generic.List<Jyx2.SkillInstance>>();
						break;
					case "Items":
						instance.Items = reader.Read<System.Collections.Generic.List<Jyx2Configs.Jyx2ConfigCharacterItem>>();
						break;
					case "Mp":
						instance.Mp = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "MaxMp":
						instance.MaxMp = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "MpType":
						instance.MpType = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Hp":
						instance.Hp = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "MaxHp":
						instance.MaxHp = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Hurt":
						instance.Hurt = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Poison":
						instance.Poison = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Tili":
						instance.Tili = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "ExpForMakeItem":
						instance.ExpForMakeItem = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Weapon":
						instance.Weapon = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Armor":
						instance.Armor = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Xiulianwupin":
						instance.Xiulianwupin = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "CurrentSkill":
						instance.CurrentSkill = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "HpInc":
						instance.HpInc = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Jyx2.RoleInstance();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_RoleInstanceArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_RoleInstanceArray() : base(typeof(Jyx2.RoleInstance[]), ES3UserType_RoleInstance.Instance)
		{
			Instance = this;
		}
	}
}