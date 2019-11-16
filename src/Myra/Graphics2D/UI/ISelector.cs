using Myra.Attributes;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Myra.Graphics2D.UI
{
	internal interface ISelector
	{
		SelectionMode SelectionMode { get; set; }
		int? SelectedIndex { get; set; }

		event EventHandler SelectedIndexChanged;
	}

	internal interface ISelectorT<ItemType>: ISelector
	{
		ObservableCollection<ItemType> Items { get; }
		ItemType SelectedItem { get; set; }
	}
}
