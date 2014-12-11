namespace saltr.utils
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public enum PropertyType
    {
        Text,
        Number,
        Boolean,
        Color,
        Group
    }

    [Serializable]
    public class PropertyValue
    {
        public PropertyType type = PropertyType.Text;

        [SerializeField]
        private float _number;

        [SerializeField]
        private string _text;

        [SerializeField]
        private bool _boolean;

        [SerializeField]
        private Color _color;

        [SerializeField]
        private List<PropertyEntry> _group;

        public float Number
        {
            get { return _number; }
            set
            {
                ResetValues();
                _number = value;
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                ResetValues();
                _text = value;
            }
        }

        public bool Boolean
        {
            get { return _boolean; }
            set
            {
                ResetValues();
                _boolean = value;
            }
        }

        public Color Color
        {
            get { return _color; }
            set
            {
                ResetValues();
                _color = value;
            }
        }

        public List<PropertyEntry> Group
        {
            get
            {
                if (_group == null)
                {
                    _group = new List<PropertyEntry>();
                }
                return _group;
            }
            set
            {
                ResetValues();
                _group = value;
            }
        }

        public object Object
        {
            get
            {
                object result = new object();

                switch (type)
                {
                    case PropertyType.Text:
                        result = _text;
                        break;
                    case PropertyType.Number:
                        result = (double)_number;
                        break;
                    case PropertyType.Boolean:
                        result = _boolean;
                        break;
                    case PropertyType.Color:
                        result = ColorToHex(_color);
                        break;
                }
                return result;
            }

            set
            {
                switch (type)
                {
                    case PropertyType.Text:
                        _text = (string)value;
                        break;
                    case PropertyType.Number:
                        _number = (float)value;
                        break;
                    case PropertyType.Boolean:
                        _boolean = (bool)value;
                        break;
                    case PropertyType.Color:
                        _color = HexToColor((string)value);
                        break;
                }
            }
        }

        private string ColorToHex(Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return "#" + hex;
        }

        private Color HexToColor(string hex)
        {
            //hex.Substring starts from 1 because '#' is 0; 
            byte r = byte.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        private void ResetValues()
        {
            _number = 0;
            _text = "";
            _boolean = false;
            _color = new Color(1, 1, 1);
            _group = null;
        }
      
        public static implicit operator List<PropertyEntry>(PropertyValue propertyValue)
        {
            return propertyValue._group;
        }
    }

    [Serializable]
    public class FeatureEntry
    {
        public string token;
        public List<PropertyEntry> properties = new List<PropertyEntry>();
        public bool isRequired;
        public bool isPropertyOpened = true;

        public FeatureEntry Clone()
        {
            FeatureEntry  clone = new FeatureEntry();
            clone.token = this.token;
            clone.isRequired = this.isRequired;
            clone.properties = new List<PropertyEntry>();

            foreach (PropertyEntry property in this.properties)
            {
                clone.properties.Add(property.Clone());
            }

            return clone;
        }
    }

    [Serializable]
    public class PropertyEntry
    {
        public string key;
        public PropertyValue value = new PropertyValue();
        public bool isPropertyOpened = true;			//only for PropertyType.Group in order to hide

        internal PropertyEntry Clone()
        {
            PropertyEntry clone = new PropertyEntry();
            clone.key = this.key;
            clone.value = new PropertyValue();
            clone.value.type = this.value.type;
            if (this.value.type != PropertyType.Group)
            {
                clone.value = this.value;
            }
            else
            {
                List<PropertyEntry> cloneGroup = new List<PropertyEntry>();
                clone.value.Group = cloneGroup;

                foreach (PropertyEntry property in (List<PropertyEntry>)this.value)
                {
                    cloneGroup.Add(property.Clone());
                }
            }
            return clone;
        }
    }
}