using System;

namespace GameKit
{
    public class StringTypedConstant<TClass> where TClass: StringTypedConstant<TClass>, new()
    {
        #region internal

        private string _name;

        protected static TClass Bind(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException();
            return new TClass
            {
                _name = name
            };
        }

        public override string ToString() => _name;

        public static implicit operator string(StringTypedConstant<TClass> constant)
        {
            if (constant is null) return "unknown"; 
            return constant._name;
        }

        #endregion
    }
}