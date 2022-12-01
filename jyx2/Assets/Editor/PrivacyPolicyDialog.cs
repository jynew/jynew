#if UNITY_ANDROID

using UnityEditor.Android;
using UnityEngine;
using System.IO;

/// <summary>
/// 以下内容，是用来在unity初始化前弹出隐私协议的窗口，调试正常unity版本是2019.4
/// </summary>
class AndroidPostBuildProcessor : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 999;

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        Debug.Log("AndroidPostBuildProcessor.OnPostGenerateGradleAndroidProject at path " + path);

        var unityPlayerJavaFilePath = path + "/src/main/java/com/unity3d/player/UnityPlayerActivity.java";

        var content = File.ReadAllText(unityPlayerJavaFilePath);

        content = content.Replace("import android.os.Process;", "import android.os.Process;\n\n" + PrivacyImport);
        content = content.Replace("mUnityPlayer = new UnityPlayer(this, this);", PrivacyString + "\n" + "mUnityPlayer = new UnityPlayer(this, this);");

        File.WriteAllText(unityPlayerJavaFilePath, content);
    }

    private const string PrivacyImport = @"
import android.app.AlertDialog;
import android.content.SharedPreferences;
import android.content.DialogInterface;
";

    private const string PrivacyString = @"
        SharedPreferences base = getSharedPreferences(""base"", MODE_PRIVATE);
        Boolean anInt = base.getBoolean(""isFirstStartJynew"", true);
        if (anInt == true){
            AlertDialog.Builder dialog = new AlertDialog.Builder(UnityPlayerActivity.this);
            dialog.setTitle(""侠启游戏隐私政策"");
            dialog.setMessage(
                    ""\n"" +
                    ""更新日期：2022年12月1日\n"" +
                    ""\n"" +
                    ""生效日期：2022年12月1日\n"" +
                    ""\n\n"" +
                    ""欢迎您选择由侠启游戏（以下简称“我们”或“侠启”）提供的游戏《群侠传，启动！》。我们深知个人信息对您的重要性，将尽全力保护您的个人信息安全。我们严格遵守法律法规，遵循以下隐私保护原则，为您提供更加安全、可靠的服务：\n"" +
                    ""\n"" +
                    ""安全可靠：我们尽力通过合理有效的信息安全技术及管理流程，防止您的信息泄露。\n"" +
                    ""\n"" + 
                    ""合理必要：我们根据合法、正当、必要的原则，仅收集实现产品功能所必要的信息。\n"" +
                    ""\n"" + 
                    ""保护隐私：我们仅在征得您同意的前提下合法收集、使用您的个人信息。\n"" + 
                    ""\n"" + 
                    ""希望您仔细阅读《隐私政策》（以下简称“本政策”），详细了解我们收集的信息以及这些信息的使用方式和途径，以便您更好地了解我们的服务并作出适当的选择。\n"" + 
                    ""\n"" + 
                    ""若您使用侠启提供服务，即表示您同意我们在本政策中所述内容。\n"" + 
                    ""\n"" + 
                    ""我们会收集哪些信息\n"" + 
                    ""\n"" + 
                    ""我们严格遵守法律法规以及与用户的约定，将收集以下信息\n"" + 
                    ""\n"" + 
                    ""1 您在使用我们服务时主动提供的信息\n"" + 
                    ""\n"" + 
                    ""1.1 您在注册帐户时填写的信息。\n"" + 
                    ""\n"" + 
                    ""包括但不限于，您在注册帐户时所填写的昵称、手机号码等。\n"" + 
                    ""\n"" + 
                    ""1.2 您在使用服务时上传的信息。\n"" + 
                    ""\n"" + 
                    ""包括但不限于，您在使用App时，上传的头像、照片、图片、文字等。\n"" + 
                    ""\n"" + 
                    ""1.3 您通过我们的客服或参加我们举办的活动时所提交的信息。\n"" + 
                    ""\n"" + 
                    ""包括但不限于，您参与我们线上活动时填写的调查问卷中可能包含您的姓名、电话、家庭地址等信息。\n"" + 
                    ""\n"" + 
                    ""我们的部分服务可能需要您提供特定的个人敏感信息来实现特定功能。若您选择不提供该类信息，则可能无法正常使用服务中的特定功能，但不影响您使用服务中的其他功能。若您主动提供您的个人敏感信息，即表示您同意我们按本政策所述目的和方式来处理您的个人敏感信息。\n"" + 
                    ""\n"" + 
                    ""2 我们在您使用服务时获取的信息\n"" + 
                    ""\n"" + 
                    ""2.1 日志信息。当您使用我们的服务时，我们可能会自动收集相关信息并存储为服务日志信息。\n"" + 
                    ""\n"" + 
                    ""(1) 设备信息。例如，设备型号、制造厂商、操作系统版本、唯一设备标识符、电池、信号强度等信息。\n"" + 
                    ""\n"" + 
                    ""(2) 软件信息。例如，软件的版本号、浏览器类型、广告标识符。\n"" + 
                    ""\n"" + 
                    ""(3) IP地址、网卡（MAC）地址。\n"" + 
                    ""\n"" + 
                    ""(4) 服务日志信息。例如，您在使用我们服务时搜索、查看的信息、服务故障信息、引荐网址等信息。\n"" + 
                    ""\n"" + 
                    ""(5) 通讯日志信息。例如，您在使用我们服务时曾经通讯的账户、通讯时间和时长。\n"" + 
                    ""\n"" + 
                    ""2.2 位置信息。当您使用与位置有关的服务时，我们可能会记录您设备所在的位置信息，以便为您提供相关服务。\n"" + 
                    ""\n"" + 
                    ""(1) 在您使用服务时，我们可能会通过IP地址、GPS、WiFi或基站等途径获取您的地理位置、语言所在地、时区等信息；\n"" + 
                    ""\n"" + 
                    ""(2) 您或其他用户在使用服务时提供的信息中可能包含您所在地理位置信息，例如您提供的帐号信息中可能包含的您所在地区信息，您或其他人共享的照片包含的地理标记信息；\n"" + 
                    ""\n"" + 
                    ""3 其他相关信息。\n"" + 
                    ""\n"" + 
                    ""3.1 为了帮助您更好地使用我们的产品或服务，我们会收集相关信息。\n"" + 
                    ""\n"" + 
                    ""例如，我们收集的好友列表。\n"" + 
                    ""\n"" + 
                    ""3.2 其他用户分享的信息中含有您的信息\n"" + 
                    ""\n"" + 
                    ""例如，其他用户发布的照片或分享的视频中可能包含您的信息。\n"" + 
                    ""\n"" + 
                    ""3.3 从第三方合作伙伴获取的信息\n"" + 
                    ""\n"" + 
                    ""我们可能会获得您在使用第三方合作伙伴服务时所产生或分享的信息。例如，您使用第三方合作伙伴提供的支付服务时，我们会获得您登录第三方合作伙伴服务的名称、登录时间，方便您进行授权管理。请您仔细阅读第三方合作伙伴服务的用户协议或隐私政策。\n"" + 
                    ""\n"" + 
                    ""我们如何使用收集的信息\n"" + 
                    ""\n"" + 
                    ""我们将严格遵守法律法规以及与用户的约定，将收集的信息用于以下用途。\n"" + 
                    ""\n"" + 
                    ""1 向您提供服务。\n"" + 
                    ""\n"" + 
                    ""2 产品开发和服务优化。例如，当我们的系统发生故障时，我们会记录和分析系统故障时产生的信息，优化我们的服务。\n"" + 
                    ""\n"" + 
                    ""3 安全保障。例如，我们会将您的信息用于身份验证、安全防范、反诈骗监测、存档备份、客户的安全服务等用途。\n"" + 
                    ""\n"" + 
                    ""4 评估、改善我们的广告投放和其他促销及推广活动的效果。\n"" + 
                    ""\n"" + 
                    ""5 管理软件。例如，进行软件认证、软件升级等。\n"" + 
                    ""\n"" + 
                    ""6 邀请您参与有关我们服务的调查。\n"" + 
                    ""\n"" + 
                    ""为了确保服务的安全，帮助我们更好地了解我们应用程序的运行情况，我们可能记录相关信息，例如，您使用应用程序的频率、故障信息、总体使用情况、性能数据以及应用程序的来源。我们不会将我们存储在分析软件中的信息与您在应用程序中提供的个人身份信息相结合。\n"" + 
                    ""\n"" + 
                    ""谁在使用收集的信息\n"" + 
                    ""\n"" + 
                    ""您提供的信息主要由我们使用，并可能会被分享给第三方合作伙伴（第三方服务供应商、承包商、代理、广告合作伙伴、应用开发者等，例如，代表我们推送通知的通讯服务提供商、我们聘请来提供第三方数据统计和分析服务的公司）（他们可能并非位于您所在的法域）这些信息将仅用于以下用途：\n"" + 
                    ""\n"" + 
                    ""1 向您提供我们的服务；\n"" + 
                    ""\n"" + 
                    ""2 理解、维护和改善我们的服务。\n"" + 
                    ""\n"" + 
                    ""我们仅会出于合法、正当、必要、特定、明确的目的共享您的个人信息，并且只会共享提供服务所必要的个人信息。\n"" + 
                    ""\n"" + 
                    ""对我们与之共享个人信息的公司、组织和个人，我们会与其签署严格的保密协定，要求他们按照我们的说明、本隐私政策以及其他任何相关的保密和安全措施来处理个人信息。\n"" + 
                    ""\n"" + 
                    ""我们可能基于以下目的披露您的个人信息：\n"" + 
                    ""\n"" + 
                    ""1 遵守适用的法律法规等有关规定；\n"" + 
                    ""\n"" + 
                    ""2 遵守法院判决、裁定或其他法律程序的规定；\n"" + 
                    ""\n"" + 
                    ""3 遵守相关政府机关或其他法定授权组织的要求；\n"" + 
                    ""\n"" + 
                    ""4 我们有理由确信需要遵守法律法规等有关规定；\n"" + 
                    ""\n"" + 
                    ""5 为执行相关服务协议或本政策、维护社会公共利益，为保护我们的客户、我们或我们的关联公司、其他用户或雇员的人身财产安全或其他合法权益合理且必要的用途。\n"" + 
                    ""\n"" + 
                    ""您所享有的权利\n"" + 
                    ""\n"" + 
                    ""按照中国相关的法律、法规、标准，以及其他国家、地区的通行做法，我们保障您对自己的个人信息行使以下权利：\n"" + 
                    ""\n"" + 
                    ""1 您有权访问您的个人信息，法律法规规定的例外情况除外。您访问、修改的范围和方式将取决于您使用的具体服务。在您访问、修改相关信息时，我们可能会要求您进行身份验证，以保障账号安全。\n"" + 
                    ""\n"" + 
                    ""2 当您发现我们处理的关于您的个人信息有错误时，您有权更正。但请您理解，由于技术所限、法律或监管要求，我们可能无法满足您的所有要求。\n"" + 
                    ""\n"" + 
                    ""未成年人保护\n"" + 
                    ""\n"" + 
                    ""若您是18周岁以下的未成年人，在使用侠启的服务前，应事先取得您的家长或法定监护人的同意。若您是未成年人的监护人，当您对您所监护的未成年人的个人信息有相关疑问时，请与我们联系。\n"" + 
                    ""\n"" + 
                    ""联系我们\n"" + 
                    ""\n"" + 
                    ""如您对本政策或其他相关事宜有疑问，请通过电子邮件地址jy_remastered@163.com与我们联系。\n"" + 
                    ""\n"" + 
                    ""适用范围\n"" + 
                    ""\n"" + 
                    ""我们的所有服务均适用本政策。但某些服务有其特定的隐私指引/声明，该特定隐私指引/声明更具体地说明我们在该服务中如何处理您的信息。如本政策与特定服务的隐私指引/声明有不一致之处，请以该特定隐私指引/声明为准。\n"" + 
                    ""\n"" + 
                    ""变更\n"" + 
                    ""\n"" + 
                    ""我们可能适时修订本政策内容。在该种情况下，若您继续使用我们的服务，即表示同意受经修订的政策约束。\n"" + 
                    "" \n"");  //设置内容

            dialog.setCancelable(false);  //是否可以取消

            dialog .setNegativeButton(""拒绝"", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialogInterface, int i) {
                    dialogInterface.dismiss();
                    android.os.Process.killProcess(android.os.Process.myPid());
                }
            });

            dialog.setPositiveButton(""同意"", new DialogInterface.OnClickListener() {
                @Override
                public void onClick(DialogInterface dialog, int which) {
                    SharedPreferences.Editor editor = base.edit();
                    editor.putBoolean(""isFirstStartJynew"", false);
                    editor.commit();
                }
            });

            dialog.show();
        }
    ";
}


#endif