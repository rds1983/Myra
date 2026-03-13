using System;

namespace Myra.Samples.Inspector
{
    /// <summary>
    /// Caches generated strings and only regenerates them on a dirty-pattern basis.
    /// </summary>
    public class StringGenerator
    {
        private readonly Func<string> _textGenerator;
        private string _textCache;
        private bool _isDirty = true;

        public string Text
        {
            get
            {
                if(_isDirty)
                    Clean();
                return _textCache;
            }
        }

        public StringGenerator(Func<string> generator)
        {
            _textGenerator = generator;
        }
            
        public void MarkDirty() => _isDirty = true;
        private void Clean()
        {
            _isDirty = false;
            _textCache = _textGenerator.Invoke();
        }
    }
}