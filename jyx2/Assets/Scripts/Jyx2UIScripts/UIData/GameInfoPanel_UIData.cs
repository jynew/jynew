using UnityEngine.UI;

public partial class GameInfoPanel
{
	private Text VersionText_Text;

	public void InitTrans()
	{
		VersionText_Text = transform.Find("VersionText").GetComponent<Text>();

	}
}
