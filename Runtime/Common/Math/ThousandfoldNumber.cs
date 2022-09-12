using System;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    [Serializable]
    public struct ThousandfoldNumber
    {
        public double value;
        public byte exp;

        public ThousandfoldNumber(double value): this()
        {
            this.value = value;
            this.exp = 0;
            Trim();
        }
        
        public ThousandfoldNumber(double value, byte exp): this()
        {
            this.value = value;
            this.exp = exp;
            Trim();
        }

        #region operations
        public static ThousandfoldNumber operator +(ThousandfoldNumber a, ThousandfoldNumber b)
        {
            a.Append(b.value, b.exp);
            return a;
        }
        public static ThousandfoldNumber operator -(ThousandfoldNumber a, ThousandfoldNumber b)
        {
            a.Append(-b.value, b.exp);
            return a;
        }

        public static ThousandfoldNumber operator *(ThousandfoldNumber a, ThousandfoldNumber m)
        {
            a.Multiply(m);
            return a;
        }

        public static ThousandfoldNumber operator *(ThousandfoldNumber a, int m)
        {
            a.MultiplyFloat(m);
            return a;
        }
        public static ThousandfoldNumber operator *(ThousandfoldNumber a, float m)
        {
            a.MultiplyFloat(m);
            return a;
        }
        public static ThousandfoldNumber operator *(ThousandfoldNumber a, double m)
        {
            a.MultiplyFloat(m);
            return a;
        }
        public static ThousandfoldNumber operator /(ThousandfoldNumber a, ThousandfoldNumber d)
        {
            a.Division(d);
            return a;
        }
        public static ThousandfoldNumber operator /(ThousandfoldNumber a, int d)
        {
            a.DivisionValue(d);
            return a;
        }
        public static ThousandfoldNumber operator /(ThousandfoldNumber a, float d)
        {
            a.DivisionValue(d);
            return a;
        }
        public static ThousandfoldNumber operator /(ThousandfoldNumber a, double d)
        {
            a.DivisionValue(d);
            return a;
        }
        public static bool operator >(ThousandfoldNumber a, ThousandfoldNumber b)
        {                     
            return a.CompareTo(b) == 1;
        }

        public static bool operator <(ThousandfoldNumber a, ThousandfoldNumber b)
        {
            return a.CompareTo(b) == -1;
        }
        public static bool operator >=(ThousandfoldNumber a, ThousandfoldNumber b)
        {
            return a.CompareTo(b) != -1;
        }
        public static bool operator <=(ThousandfoldNumber a, ThousandfoldNumber b)
        {
            return a.CompareTo(b) != 1;
        }
        public static bool operator ==(ThousandfoldNumber a, ThousandfoldNumber b)
        {
            return a.exp == b.exp && Math.Abs(a.value - b.value) < 0.0001f;            
        }

        public static bool operator !=(ThousandfoldNumber a, ThousandfoldNumber b)
        {
            return a.exp != b.exp || Math.Abs(a.value - b.value) > 0.0001f;
        }

        public override bool Equals(object obj)
        {
            if (obj is ThousandfoldNumber number) return this == number;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public static implicit operator ThousandfoldNumber(double num)
        {
            return new ThousandfoldNumber(num, 0);
        }

        public static implicit operator ThousandfoldNumber(int value)
        {
            return new ThousandfoldNumber(value, 0);
        }

        public static implicit operator double(ThousandfoldNumber num)
        {
            return (num.value * Math.Pow(1000, num.exp));
        }

        public override string ToString()
        {
            if (value >= 100) return $"{value/100:0.0000}e+{exp * 3 + 2:000}";
            if (value >= 10) return $"{value/10:0.0000}e+{exp * 3 + 1:000}";
            return $"{value:0.0000}e+{exp * 3:000}";
        }

        
        public string ToStringWithSpace(string[] articles) => ToString(articles, " ");
        public string ToString(string[] articles, string divider = "")
        {
            if (exp == 0) return $"{value:0}";
            var dExp = 0;
            if (articles.Length <= exp) dExp = exp - articles.Length + 1;
            return $"{value * (Math.Pow(1000, dExp)):0.####}{divider}{articles[Math.Min(exp, articles.Length - 1)]}";
        }

        #endregion
    
        public void Append(double v, byte e)
        {
            if (e < exp)
            {
                if (exp - e > 3) return;
                v /= (float)Math.Pow(1000, exp - e);
            }                            

            if (e > exp)
            {
                if (e - exp > 3)
                    value = 0;
                else
                    value /= (float)Math.Pow(1000, e - exp);
                exp = e;
            }

            value += v;
            Trim();
        }

        public void Multiply(ThousandfoldNumber m)
        {
            exp += m.exp;
            MultiplyFloat(m.value);
        }
            
        private void MultiplyFloat(double m)
        {
            if (m == 0)
            {
                exp = 0;
                value = 0;
                return;
            }

            while (Math.Abs(value) > 1000)
            {
                value /= 1000;
                exp++;
            }

            value *= m;
            Trim();
        }

        public void Division(ThousandfoldNumber m)
        {
            if (exp < m.exp)
            {
                exp = 0;
                value = 0;
                return;
            } 
            exp -= m.exp;
            DivisionValue(m.value);
        }

        private void DivisionValue(double m)
        {
            while (Math.Abs(m) > 1000f && exp > 0)
            {
                exp--;
                m /= 1000f;
            }
            value /= m;
            Trim();
        }

        public void Pow(int p)
        {
            if (p == 0)
            {
                value = 1;
                exp = 0;
                return;
            }
            var v = value;
            var e = exp;

            for (int i = 0; i < p - 1; i++)
            {
                exp += e;
                value *= v;

                while (Math.Abs(value) > 1000)
                {
                    value /= 1000;
                    exp++;
                }
            }
        }

        public int CompareTo(ThousandfoldNumber num)
        {
            // sign equals
            int s1 = value >= 0 ? 1 : -1;
            int s2 = num.value >= 0 ? 1 : -1;
            if (s1 != s2) return s1 > s2 ? 1 : -1;

            // exp equals
            if (exp != num.exp) return exp > num.exp ? 1 : -1;

            // val equals
            return value.CompareTo(num.value);
        }

        private void Trim()
        {
            while (Math.Abs(value) < 1 && exp > 0)
            {
                value *= 1000;
                exp--;
            }
            
            while (Math.Abs(value) > 1000)
            {
                value /= 1000f;
                exp++;
            }            
        }
    }
}