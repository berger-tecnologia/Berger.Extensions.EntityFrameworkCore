namespace Berger.Extensions.EntityFrameworkCore
{
    public class FieldRelational
    {
        #region Properties
        public Type Type;
        public bool Index;
        public object Value;
        public string Name;
        #endregion

        #region Constructors
        public FieldRelational(Type type, bool index)
        {
            Type = type;
            Index = index;
        }
        public FieldRelational(Type type, string name)
        {
            Type = type;
            Name = name;
        }
        public FieldRelational(string name)
        {
            Name = name;
        }
        public FieldRelational(Type type, bool index, object value)
        {
            Type = type;
            Index = index;
            Value = value;
        }
        public FieldRelational(Type type, string name, bool index)
        {
            Name = name;
            Type = type;
            Index = index;
        }
        public FieldRelational(Type type, string name, bool index, object value)
        {
            Name = name;
            Type = type;
            Index = index;
            Value = value;
        }
        #endregion
    }
}