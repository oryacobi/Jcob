using System;
using Newtonsoft.Json;

namespace OrYacobi.Jcob.Models
{
    public class BinaryAddress
    {
        private Type _type;
        private string _typeString;

        public long Ptr { get; set; }
        public long Len { get; set; }
        public int[][] Structure { get; set; }

        [JsonIgnore]
        public Type Type
        {
            get => _type ?? (_type = _typeString == null? null : Type.GetType(_typeString));
            set
            {
                _type = value;
                if (value != null)
                    _typeString = null;
            }
        }

        public string TypeString
        {
            get => _typeString ?? (_typeString = _type?.FullName);
            set => _typeString = value;
        }
    }
}
