using Myra.Graphics2D.UI;
using Myra.Utility;

namespace Myra.Samples.RogueEditor.UI
{
	public partial class NewMapDialog : Dialog
	{
		public NewMapDialog()
		{
			Title = "New Map";

			foreach(var tileInfo in Studio.Instance.Module.TileInfos)
			{
				var text = "[" + tileInfo.Value.Color.ToHexString() + "]" + tileInfo.Key;
				_comboFiller.Items.Add(new ListItem(text, null, tileInfo.Value));
			}

			_comboFiller.SelectedIndex = 0;
			
			UpdateButtons();
		}

		private void UpdateButtons()
		{
			ButtonOk.Enabled = !string.IsNullOrEmpty(_textId.Text);
		}
	}
}