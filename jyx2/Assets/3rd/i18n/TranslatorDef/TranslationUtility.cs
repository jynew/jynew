/*
 * 金庸群侠传3D重制版
 * https://github.com/jynew/jynew
 *
 * 这是本开源项目文件头，所有代码均使用MIT协议。
 * 但游戏内资源和第三方插件、dll等请仔细阅读LICENSE相关授权协议文档。
 *
 * 金庸老先生千古！
 *
 * 本文件作者：东方怂天(EasternDay)
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace i18n.TranslatorDef
{
    /// <summary>
    /// 一个静态的翻译辅助工具集
    /// 主要定义了一些功能辅助函数和语言数组
    /// </summary>
    public static class TranslationUtility
    {
        /// <summary>
        /// 全局通用的获取文本翻译函数，针对string做的单独的拓展方法
        /// </summary>
        /// <param name="content">待转化文本</param>
        /// <param name="fromToken">来源于哪里，一般是UI或者物体</param>
        /// <returns></returns>
        public static string GetContent(this string content,string fromToken="")
        {
            //没有默认全局配置或者语言文件则直接返回
            if (!GlobalAssetConfig.Instance || !GlobalAssetConfig.Instance.defaultTranslator) return content;
            
            //调用默认全局配置
            var translator = GlobalAssetConfig.Instance.defaultTranslator;
            content = translator.GetOrRegTranslation(fromToken , content);
            return content;
        }

        #region 语言选择下拉列表显示相关
        
        /// <summary>
        /// 语言列表，采用了单例模式，防止多次加载浪费内存
        /// </summary>
        public static ValueDropdownList<LangFlag> LocaleList
        {
            get
            {
                if (_localeList!=null) return _localeList;
                _localeList = LangFlagSelection() as ValueDropdownList<LangFlag>;
                return _localeList;
            }
        }
        
        private static ValueDropdownList<LangFlag> _localeList;
        
        /// <summary>
        /// 将当前类中的数据字典转化为下拉列表选项
        /// </summary>
        /// <returns>返回数组</returns>
        private static IEnumerable LangFlagSelection()
        {
            var list = new ValueDropdownList<LangFlag>();
            LangNames.ForEach(pair =>
                list.Add(new ValueDropdownItem<LangFlag>($"{pair.Value} ({pair.Key.ToString()})",
                    pair.Key)));
            return list;
        }
        #endregion

        #region 语言数组相关设置
        
        /// <summary>
        /// ISO 639-1标准定义了两个字母代码，这是常用语言。
        /// 有些语言标志还额外使用 ISO 3166标准适用于国家和地区的双字符代码。
        /// </summary>
        public enum LangFlag
        {
            /// <summary>
            ///     Afar
            /// </summary>
            AA,

            /// <summary>
            ///     Abkhazian
            /// </summary>
            AB,

            /// <summary>
            ///     Avestan
            /// </summary>
            AE,

            /// <summary>
            ///     Afrikaans
            /// </summary>
            AF,

            /// <summary>
            ///     Akan
            /// </summary>
            AK,

            /// <summary>
            ///     Amharic
            /// </summary>
            AM,

            /// <summary>
            ///     Aragonese
            /// </summary>
            AN,

            /// <summary>
            ///     Arabic
            /// </summary>
            AR,

            /// <summary>
            ///     Assamese
            /// </summary>
            AS,

            /// <summary>
            ///     Avaric
            /// </summary>
            AV,

            /// <summary>
            ///     Aymara
            /// </summary>
            AY,

            /// <summary>
            ///     Azerbaijani
            /// </summary>
            AZ,

            /// <summary>
            ///     Bashkir
            /// </summary>
            BA,

            /// <summary>
            ///     Belarusian
            /// </summary>
            BE,

            /// <summary>
            ///     Bulgarian
            /// </summary>
            BG,

            /// <summary>
            ///     Bihari
            /// </summary>
            BH,

            /// <summary>
            ///     Bislama
            /// </summary>
            BI,

            /// <summary>
            ///     Bambara
            /// </summary>
            BM,

            /// <summary>
            ///     Bengali
            /// </summary>
            BN,

            /// <summary>
            ///     Tibetan
            /// </summary>
            BO,

            /// <summary>
            ///     Breton
            /// </summary>
            BR,

            /// <summary>
            ///     Bosnian
            /// </summary>
            BS,

            /// <summary>
            ///     Catalan
            /// </summary>
            CA,

            /// <summary>
            ///     Chechen
            /// </summary>
            CE,

            /// <summary>
            ///     Chamorro
            /// </summary>
            CH,

            /// <summary>
            ///     Corsican
            /// </summary>
            CO,

            /// <summary>
            ///     Cree
            /// </summary>
            CR,

            /// <summary>
            ///     Czech
            /// </summary>
            CS,

            /// <summary>
            ///     Church Slavic
            /// </summary>
            CU,

            /// <summary>
            ///     Chuvash
            /// </summary>
            CV,

            /// <summary>
            ///     Welsh
            /// </summary>
            CY,

            /// <summary>
            ///     Danish
            /// </summary>
            DA,

            /// <summary>
            ///     German
            /// </summary>
            DE,

            /// <summary>
            ///     Divehi
            /// </summary>
            DV,

            /// <summary>
            ///     Dzongkha
            /// </summary>
            DZ,

            /// <summary>
            ///     Ewe
            /// </summary>
            EE,

            /// <summary>
            ///     Greek
            /// </summary>
            EL,

            /// <summary>
            ///     English
            /// </summary>
            EN,

            /// <summary>
            ///     English(United Kingdom)
            /// </summary>
            EN_GB,

            /// <summary>
            ///     Esperanto
            /// </summary>
            EO,

            /// <summary>
            ///     Spanish
            /// </summary>
            ES,

            /// <summary>
            ///     Estonian
            /// </summary>
            ET,

            /// <summary>
            ///     Basque
            /// </summary>
            EU,

            /// <summary>
            ///     Persian
            /// </summary>
            FA,

            /// <summary>
            ///     Fulah
            /// </summary>
            FF,

            /// <summary>
            ///     Finnish
            /// </summary>
            FI,

            /// <summary>
            ///     Finnish
            /// </summary>
            FJ,

            /// <summary>
            ///     Faroese
            /// </summary>
            FO,

            /// <summary>
            ///     French
            /// </summary>
            FR,

            /// <summary>
            ///    Western Frisian
            /// </summary>
            FY,

            /// <summary>
            ///     Irish
            /// </summary>
            GA,

            /// <summary>
            ///     Scottish Gaelic
            /// </summary>
            GD,

            /// <summary>
            ///     Galician
            /// </summary>
            GL,

            /// <summary>
            ///     Guaraní
            /// </summary>
            GN,

            /// <summary>
            ///     Gujarati
            /// </summary>
            GU,

            /// <summary>
            ///     Manx
            /// </summary>
            GV,

            /// <summary>
            ///     Hausa
            /// </summary>
            HA,

            /// <summary>
            ///     Hebrew
            /// </summary>
            HE,

            /// <summary>
            ///     Hindi
            /// </summary>
            HI,

            /// <summary>
            ///     Hiri Motu
            /// </summary>
            HO,

            /// <summary>
            ///     Croatian
            /// </summary>
            HR,

            /// <summary>
            ///     Haitian Creole
            /// </summary>
            HT,

            /// <summary>
            ///     Hungarian
            /// </summary>
            HU,

            /// <summary>
            ///     Armenian
            /// </summary>
            HY,

            /// <summary>
            ///     Herero
            /// </summary>
            HZ,

            /// <summary>
            ///     International Auxiliary Language Association
            /// </summary>
            IA,

            /// <summary>
            ///     Indonesian
            /// </summary>
            ID,

            /// <summary>
            ///     Interlingue
            /// </summary>
            IE,

            /// <summary>
            ///     Igbo
            /// </summary>
            IG,

            /// <summary>
            ///     Sichuan Yi
            /// </summary>
            II,

            /// <summary>
            ///     Inupiaq
            /// </summary>
            IK,

            /// <summary>
            ///     Ido
            /// </summary>
            IO,

            /// <summary>
            ///     Icelandic
            /// </summary>
            IS,

            /// <summary>
            ///     Italian
            /// </summary>
            IT,

            /// <summary>
            ///     Inuktitut    
            /// </summary>
            IU,

            /// <summary>
            ///     Japanese
            /// </summary>
            JA,

            /// <summary>
            ///     Javanese
            /// </summary>
            JV,

            /// <summary>
            ///     Georgian
            /// </summary>
            KA,

            /// <summary>
            ///     Kongo
            /// </summary>
            KG,

            /// <summary>
            ///     Kikuyu
            /// </summary>
            KI,

            /// <summary>
            ///     Kwanyama
            /// </summary>
            KJ,

            /// <summary>
            ///     Kazakh
            /// </summary>
            KK,

            /// <summary>
            ///     Kalaallisut
            /// </summary>
            KL,

            /// <summary>
            ///     Khmer
            /// </summary>
            KM,

            /// <summary>
            ///     Kannada
            /// </summary>
            KN,

            /// <summary>
            ///     Korean
            /// </summary>
            KO_KR,

            /// <summary>
            ///     Korean(North Korea)
            /// </summary>
            KO_KP,

            /// <summary>
            ///     Kanuri
            /// </summary>
            KR,

            /// <summary>
            ///     Kashmiri
            /// </summary>
            KS,

            /// <summary>
            ///     Kurdish
            /// </summary>
            KU,

            /// <summary>
            ///     Komi
            /// </summary>
            KV,

            /// <summary>
            ///     Cornish
            /// </summary>
            KW,

            /// <summary>
            ///     Kirghiz
            /// </summary>
            KY,

            /// <summary>
            ///     Latin
            /// </summary>
            LA,

            /// <summary>
            ///     Luxembourgish
            /// </summary>
            LB,

            /// <summary>
            ///     Ganda
            /// </summary>
            LG,

            /// <summary>
            ///     Limburgish
            /// </summary>
            LI,

            /// <summary>
            ///     Lingala
            /// </summary>
            LN,

            /// <summary>
            ///     Lao
            /// </summary>
            LO,

            /// <summary>
            ///     Lithuanian
            /// </summary>
            LT,

            /// <summary>
            ///     Luba-Katanga
            /// </summary>
            LU,

            /// <summary>
            ///     Latvian
            /// </summary>
            LV,

            /// <summary>
            ///     Malagasy
            /// </summary>
            MG,

            /// <summary>
            ///     Marshallese
            /// </summary>
            MH,

            /// <summary>
            ///     Māori
            /// </summary>
            MI,

            /// <summary>
            ///     Macedonian
            /// </summary>
            MK,

            /// <summary>
            ///     Malayalam
            /// </summary>
            ML,

            /// <summary>
            ///     Mongolian
            /// </summary>
            MN,

            /// <summary>
            ///     Moldavian
            /// </summary>
            MO,

            /// <summary>
            ///     Marathi
            /// </summary>
            MR,

            /// <summary>
            ///     Malay
            /// </summary>
            MS,

            /// <summary>
            ///     Maltese
            /// </summary>
            MT,

            /// <summary>
            ///     Burmese
            /// </summary>
            MY,

            /// <summary>
            ///     Nauru
            /// </summary>
            NA,

            /// <summary>
            ///     Norwegian 
            /// </summary>
            NB,

            /// <summary>
            ///     North Ndebele
            /// </summary>
            ND,

            /// <summary>
            ///     Nepali
            /// </summary>
            NE,

            /// <summary>
            ///     Ndonga
            /// </summary>
            NG,

            /// <summary>
            ///     Dutch
            /// </summary>
            NL,

            /// <summary>
            ///     Norwegian 
            /// </summary>
            NN,

            /// <summary>
            ///     Norwegian
            /// </summary>
            NO,

            /// <summary>
            ///     South Ndebele
            /// </summary>
            NR,

            /// <summary>
            ///     Navajo
            /// </summary>
            NV,

            /// <summary>
            ///     Chichewa
            /// </summary>
            NY,

            /// <summary>
            ///     Occitan
            /// </summary>
            OC,

            /// <summary>
            ///     Ojibwa
            /// </summary>
            OJ,

            /// <summary>
            ///     Oromo
            /// </summary>
            OM,

            /// <summary>
            ///     Oriya
            /// </summary>
            OR,

            /// <summary>
            ///     Ossetian
            /// </summary>
            OS,

            /// <summary>
            ///     Panjabi
            /// </summary>
            PA,

            /// <summary>
            ///     Pāli
            /// </summary>
            PI,

            /// <summary>
            ///     Polish
            /// </summary>
            PL,

            /// <summary>
            ///     Pashto
            /// </summary>
            PS,

            /// <summary>
            ///     Portuguese
            /// </summary>
            PT,

            /// <summary>
            ///     Portuguese(Brazil)
            /// </summary>
            PT_BR,

            /// <summary>
            ///     Quechua
            /// </summary>
            QU,

            /// <summary>
            ///     Raeto-Romance
            /// </summary>
            RM,

            /// <summary>
            ///     Kirundi
            /// </summary>
            RN,

            /// <summary>
            ///     Romanian
            /// </summary>
            RO,

            /// <summary>
            ///     Russian
            /// </summary>
            RU,

            /// <summary>
            ///     Kinyarwanda
            /// </summary>
            RW,

            /// <summary>
            ///     Sanskrit
            /// </summary>
            SA,

            /// <summary>
            ///     Sardinian
            /// </summary>
            SC,

            /// <summary>
            ///     Sindhi
            /// </summary>
            SD,

            /// <summary>
            ///     Northern Sami
            /// </summary>
            SE,

            /// <summary>
            ///     Sango
            /// </summary>
            SG,

            /// <summary>
            ///     Sinhalese
            /// </summary>
            SI,

            /// <summary>
            ///     Slovak
            /// </summary>
            SK,

            /// <summary>
            ///     Slovenian
            /// </summary>
            SL,

            /// <summary>
            ///     Samoan
            /// </summary>
            SM,

            /// <summary>
            ///     Shona
            /// </summary>
            SN,

            /// <summary>
            ///     Somali
            /// </summary>
            SO,

            /// <summary>
            ///     Albanian
            /// </summary>
            SQ,

            /// <summary>
            ///     Serbian
            /// </summary>
            SR,

            /// <summary>
            ///     Swati
            /// </summary>
            SS,

            /// <summary>
            ///     Sotho
            /// </summary>
            ST,

            /// <summary>
            ///     Sundanese
            /// </summary>
            SU,

            /// <summary>
            ///     Swedish
            /// </summary>
            SV,

            /// <summary>
            ///     Swahili
            /// </summary>
            SW,

            /// <summary>
            ///     Tamil
            /// </summary>
            TA,

            /// <summary>
            ///     Telugu
            /// </summary>
            TE,

            /// <summary>
            ///     Tajik
            /// </summary>
            TG,

            /// <summary>
            ///     Thai
            /// </summary>
            TH,

            /// <summary>
            ///     Tigrinya
            /// </summary>
            TI,

            /// <summary>
            ///     Turkmen
            /// </summary>
            TK,

            /// <summary>
            ///     Tagalog
            /// </summary>
            TL,

            /// <summary>
            ///     Tswana
            /// </summary>
            TN,

            /// <summary>
            ///     Tonga
            /// </summary>
            TO,

            /// <summary>
            ///     Turkish
            /// </summary>
            TR,

            /// <summary>
            ///     Tsonga
            /// </summary>
            TS,

            /// <summary>
            ///     Tatar
            /// </summary>
            TT,

            /// <summary>
            ///     Twi
            /// </summary>
            TW,

            /// <summary>
            ///     Tahitian
            /// </summary>
            TY,

            /// <summary>
            ///     Uyghur
            /// </summary>
            UG,

            /// <summary>
            ///     Ukrainian
            /// </summary>
            UK,

            /// <summary>
            ///     Urdu
            /// </summary>
            UR,

            /// <summary>
            ///     Uzbek
            /// </summary>
            UZ,

            /// <summary>
            ///     Venda
            /// </summary>
            VE,

            /// <summary>
            ///     Vietnamese
            /// </summary>
            VI,

            /// <summary>
            ///     Volapük
            /// </summary>
            VO,

            /// <summary>
            ///     Walloon
            /// </summary>
            WA,

            /// <summary>
            ///     Wolof
            /// </summary>
            WO,

            /// <summary>
            ///     Xhosa
            /// </summary>
            XH,

            /// <summary>
            ///     Yiddish
            /// </summary>
            YI,

            /// <summary>
            ///     Yoruba
            /// </summary>
            YO,

            /// <summary>
            ///     Zhuang
            /// </summary>
            ZA,

            /// <summary>
            ///     Simplified Chinese
            /// </summary>
            ZH_CN,

            /// <summary>
            ///     Traditional Chinese
            /// </summary>
            ZH_TW,

            /// <summary>
            ///     Zulu
            /// </summary>
            ZU
        }
        
        /// <summary>
        /// 语言名字典集合，只读
        /// </summary>
        public static readonly Dictionary<LangFlag, string> LangNames = new Dictionary<LangFlag, string>
        {
            {LangFlag.AA, "Afaraf"},
            {LangFlag.AB, "Аҧсуа"},
            {LangFlag.AE, "avesta"},
            {LangFlag.AF, "Afrikaans"},
            {LangFlag.AK, "Akan"},
            {LangFlag.AM, "አማርኛ"},
            {LangFlag.AN, "Aragonés"},
            {LangFlag.AR, "العربية"},
            {LangFlag.AS, "অসমীয়া"},
            {LangFlag.AV, "магӀарул мацӀ"},
            {LangFlag.AY, "aymar aru"},
            {LangFlag.AZ, "azərbaycan dili"},
            {LangFlag.BA, "башҡорт теле"},
            {LangFlag.BE, "Беларуская"},
            {LangFlag.BG, "български език"},
            {LangFlag.BH, "भोजपुरी"},
            {LangFlag.BI, "Bislama"},
            {LangFlag.BM, "bamanankan"},
            {LangFlag.BN, "বাংলা"},
            {LangFlag.BO, "བོད་ཡིག"},
            {LangFlag.BR, "brezhoneg"},
            {LangFlag.BS, "bosanski jezik"},
            {LangFlag.CA, "Català"},
            {LangFlag.CE, "нохчийн мотт"},
            {LangFlag.CH, "Chamoru"},
            {LangFlag.CO, "lingua corsa"},
            {LangFlag.CR, "ᓀᐦᐃᔭᐍᐏᐣ"},
            {LangFlag.CS, "čeština"},
            {LangFlag.CU, "ѩзыкъ словѣньскъ"},
            {LangFlag.CV, "чӑваш чӗлхи"},
            {LangFlag.CY, "Cymraeg"},
            {LangFlag.DA, "dansk"},
            {LangFlag.DE, "Deutsch"},
            {LangFlag.DV, "ދިވެހި"},
            {LangFlag.DZ, "རྫོང་ཁ"},
            {LangFlag.EE, "Ɛʋɛgbɛ"},
            {LangFlag.EL, "Ελληνικά"},
            {LangFlag.EN, "English"},
            {LangFlag.EN_GB, "English(United Kingdom)"},
            {LangFlag.EO, "Esperanto"},
            {LangFlag.ES, "español"},
            {LangFlag.ET, "Eesti keel"},
            {LangFlag.EU, "euskara"},
            {LangFlag.FA, "فارسی"},
            {LangFlag.FF, "Fulfulde"},
            {LangFlag.FI, "suomen kieli"},
            {LangFlag.FJ, "vosa Vakaviti"},
            {LangFlag.FO, "Føroyskt"},
            {LangFlag.FR, "français"},
            {LangFlag.FY, "Frysk"},
            {LangFlag.GA, "Gaeilge"},
            {LangFlag.GD, "Gàidhlig"},
            {LangFlag.GL, "Galego"},
            {LangFlag.GN, "Avañe'ẽ"},
            {LangFlag.GU, "ગુજરાતી"},
            {LangFlag.GV, "Ghaelg"},
            {LangFlag.HA, "هَوُسَ"},
            {LangFlag.HE, "עברית"},
            {LangFlag.HI, "हिन्दी"},
            {LangFlag.HO, "Hiri Motu"},
            {LangFlag.HR, "Hrvatski"},
            {LangFlag.HT, "Kreyòl ayisyen"},
            {LangFlag.HU, "Magyar"},
            {LangFlag.HY, "Հայերեն"},
            {LangFlag.HZ, "Otjiherero"},
            {LangFlag.IA, "Interlingua"},
            {LangFlag.ID, "Bahasa Indonesia"},
            {LangFlag.IE, "Interlingue"},
            {LangFlag.IG, "Igbo"},
            {LangFlag.II, "ꆇꉙ"},
            {LangFlag.IK, "Iñupiaq"},
            {LangFlag.IO, "Ido"},
            {LangFlag.IS, "Íslenska"},
            {LangFlag.IT, "Italiano"},
            {LangFlag.IU, "ᐃᓄᒃᑎᑐᑦ"},
            {LangFlag.JA, "日本語(にほんご)"},
            {LangFlag.JV, "basa Jawa"},
            {LangFlag.KA, "ქართული"},
            {LangFlag.KG, "KiKongo"},
            {LangFlag.KI, "Gĩkũyũ"},
            {LangFlag.KJ, "Kuanyama"},
            {LangFlag.KK, "قازاق تىلى"},
            {LangFlag.KL, "kalaallisut"},
            {LangFlag.KM, "ភាសាខ្មែរ"},
            {LangFlag.KN, "ಕನ್ನಡ"},
            {LangFlag.KO_KR, "한국어(韓國語)"},
            {LangFlag.KO_KP, "조선말(朝鮮말)"},
            {LangFlag.KR, "Kanuri"},
            {LangFlag.KS, "कश्मीरी‎"},
            {LangFlag.KU, "كوردی‎"},
            {LangFlag.KV, "коми кыв"},
            {LangFlag.KW, "Kernewek"},
            {LangFlag.KY, "قىرعىز تىلى"},
            {LangFlag.LA, "latine"},
            {LangFlag.LB, "Lëtzebuergesch"},
            {LangFlag.LG, "Luganda"},
            {LangFlag.LI, "Limburgs"},
            {LangFlag.LN, "Lingála"},
            {LangFlag.LO, "ພາສາລາວ"},
            {LangFlag.LT, "lietuvių kalba"},
            {LangFlag.LU, "Tshiluba"},
            {LangFlag.LV, "latviešu valoda"},
            {LangFlag.MG, "Malagasy fiteny"},
            {LangFlag.MH, "Kajin M̧ajeļ"},
            {LangFlag.MI, "te reo Māori"},
            {LangFlag.MK, "македонски јазик"},
            {LangFlag.ML, "മലയാളം"},
            {LangFlag.MN, "Монгол хэл"},
            {LangFlag.MO, "moldovenească"},
            {LangFlag.MR, "मराठी"},
            {LangFlag.MS, "bahasa Melayu"},
            {LangFlag.MT, "Malti"},
            {LangFlag.MY, "ဗမာစာ"},
            {LangFlag.NA, "Ekakairũ Naoero"},
            {LangFlag.NB, "Norsk bokmål"},
            {LangFlag.ND, "isiNdebele"},
            {LangFlag.NE, "नेपाली"},
            {LangFlag.NG, "Owambo"},
            {LangFlag.NL, "Nederlands"},
            {LangFlag.NN, "Norsk nynorsk"},
            {LangFlag.NO, "Norsk"},
            {LangFlag.NR, "Ndébélé"},
            {LangFlag.NV, "Diné bizaad"},
            {LangFlag.NY, "chiCheŵa"},
            {LangFlag.OC, "Occitan"},
            {LangFlag.OJ, "ᐊᓂᔑᓈᐯᒧᐎᓐ"},
            {LangFlag.OM, "Afaan Oromoo"},
            {LangFlag.OR, "ଓଡ଼ିଆ"},
            {LangFlag.OS, "Ирон æвзаг"},
            {LangFlag.PA, "ਪੰਜਾਬੀ"},
            {LangFlag.PI, "पाऴि"},
            {LangFlag.PL, "polski"},
            {LangFlag.PS, "پښتو"},
            {LangFlag.PT, "Português"},
            {LangFlag.PT_BR, "Português(Brasil)"},
            {LangFlag.QU, "Runa Simi"},
            {LangFlag.RM, "rumantsch grischun"},
            {LangFlag.RN, "kiRundi"},
            {LangFlag.RO, "română"},
            {LangFlag.RU, "русский язык"},
            {LangFlag.RW, "Kinyarwanda"},
            {LangFlag.SA, "संस्कृतम्"},
            {LangFlag.SC, "sardu"},
            {LangFlag.SD, "सिन्धी"},
            {LangFlag.SE, "Davvisámegiella"},
            {LangFlag.SG, "yângâ tî sängö"},
            {LangFlag.SI, "සිංහල"},
            {LangFlag.SK, "slovenčina"},
            {LangFlag.SL, "slovenščina"},
            {LangFlag.SM, "gagana fa'a Samoa"},
            {LangFlag.SN, "gagana fa'a Samoa"},
            {LangFlag.SO, "Soomaaliga"},
            {LangFlag.SQ, "Shqip"},
            {LangFlag.SR, "српски језик"},
            {LangFlag.SS, "SiSwati"},
            {LangFlag.ST, "seSotho"},
            {LangFlag.SU, "Basa Sunda"},
            {LangFlag.SV, "Svenska"},
            {LangFlag.SW, "Kiswahili"},
            {LangFlag.TA, "தமிழ்"},
            {LangFlag.TE, "తెలుగు"},
            {LangFlag.TG, "тоҷикӣ"},
            {LangFlag.TH, "ไทย"},
            {LangFlag.TI, "ትግርኛ"},
            {LangFlag.TK, "Türkmen"},
            {LangFlag.TL, "Tagalog"},
            {LangFlag.TN, "seTswana"},
            {LangFlag.TO, "faka Tonga"},
            {LangFlag.TR, "Türkçe"},
            {LangFlag.TS, "xiTsonga"},
            {LangFlag.TT, "татарча"},
            {LangFlag.TW, "Twi"},
            {LangFlag.TY, "Reo Mā`ohi"},
            {LangFlag.UG, "ئۇيغۇرچە‎"},
            {LangFlag.UK, "Українська"},
            {LangFlag.UR, "اردو"},
            {LangFlag.UZ, "O'zbek"},
            {LangFlag.VE, "tshiVenḓa"},
            {LangFlag.VI, "Tiếng Việt"},
            {LangFlag.VO, "Volapük"},
            {LangFlag.WA, "Walon"},
            {LangFlag.WO, "Wollof"},
            {LangFlag.XH, "isiXhosa"},
            {LangFlag.YI, "ייִדיש"},
            {LangFlag.YO, "Yorùbá"},
            {LangFlag.ZA, "Saw cuengh"},
            {LangFlag.ZH_CN, "简体中文"},
            {LangFlag.ZH_TW, "正體中文"},
            {LangFlag.ZU, "isiZulu"}
        };

        #endregion
    }
}