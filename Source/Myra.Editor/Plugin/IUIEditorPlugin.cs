namespace Myra.Editor.Plugin
{
	public interface IUIEditorPlugin
	{
		void OnLoad();
		void FillCustomWidgetTypes(WidgetTypesSet widgetTypes);
	}
}
