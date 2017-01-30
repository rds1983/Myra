using System.Collections.Generic;
using System.Collections.ObjectModel;
using Myra.Edit;
using Newtonsoft.Json;

namespace Myra.Graphics2D.UI
{
	public class GridBased: Grid
	{
		[HiddenInEditor]
		[JsonIgnore]
		public override ObservableCollection<Proportion> ColumnsProportions
		{
			get { return base.ColumnsProportions; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override ObservableCollection<Proportion> RowsProportions
		{
			get { return base.RowsProportions; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override bool SkipEmptyColumns
		{
			get { return base.SkipEmptyColumns; }
			set { base.SkipEmptyColumns = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override bool SkipEmptyRows
		{
			get { return base.SkipEmptyRows; }
			set { base.SkipEmptyRows = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override IList<Widget> Widgets
		{
			get { return base.Widgets; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override int RowSpacing
		{
			get { return base.RowSpacing; }

			set { base.RowSpacing = value; }
		}

		[HiddenInEditor]
		[JsonIgnore]
		public override int ColumnSpacing
		{
			get { return base.ColumnSpacing; }

			set { base.ColumnSpacing = value; }
		}
	}
}
