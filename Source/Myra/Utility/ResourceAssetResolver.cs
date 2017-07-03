using System;
using System.IO;
using System.Reflection;

namespace Myra.Utility
{
    public class ResourceAssetResolver
    {
        private Assembly _assembly;

        public Assembly Assembly
        {
            get { return _assembly; }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _assembly = value;
            }
        }

        public string Prefix { get; set; }

        public ResourceAssetResolver(Assembly assembly, string prefix)
        {
            Assembly = assembly;
            Prefix = prefix;
        }

        public Stream Open(string assetName)
        {
            // Once you figure out the name, pass it in as the argument here.
            var stream = Assembly.GetManifestResourceStream(Prefix + assetName);

            return stream;
        }

        public string ReadAsString(string assetName)
        {
            string result;
            using (var input = Open(assetName))
            {
                using (var textReader = new StreamReader(input))
                {
                    result = textReader.ReadToEnd();
                }
            }

            return result;
        }
    }
}