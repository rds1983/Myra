using AssetManagementBase;

namespace Myra.Graphics2D.UI.Properties
{
    public interface IInspector
    {
        Desktop Desktop { get; }
        
        /// <summary>
        /// The selected field object.
        /// </summary>
        object SelectedField { get; }
        
        /// <summary>
        /// Base file path content is chosen from
        /// </summary>
        string BasePath { get; }
        AssetManager AssetManager { get; }
        
        void FireChanged(string propertyName);
    }
}