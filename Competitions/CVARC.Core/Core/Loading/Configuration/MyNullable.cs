using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVARC.V2
{

    //С головой все хорошо. Этот класс действительно нужен. Класс Nullable<double> некорректно передается через JSON!
	public abstract class MyNullable 
	{
		public abstract object GetValue();
	    public bool HasValue { get; set; }
	}


	public class MyNullable<T> : MyNullable
	{
		public override object GetValue()
		{
			return Value;	
		}
		public T Value { get; set; }
		public MyNullable(T value) 
        { 
            Value = value;
		    HasValue = true;
		}

	    public MyNullable()
	    {
	    }

	    public static implicit operator MyNullable<T>(T value)
		{
			return new MyNullable<T>(value);
		}
		public static implicit operator T(MyNullable<T> t)
		{
			return t.Value;
		}
		public override string ToString()
		{
			if (Value == null) return null;
			return Value.ToString();
		}
	}
}
