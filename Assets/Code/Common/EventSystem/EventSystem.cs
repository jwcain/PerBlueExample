using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Bestagon.Events.Internal {

    public class EventSystem<K> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key) => _Handle(key, (object func) => { return ((System.Func<object>)func)?.Invoke(); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func< object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func< object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T t) => _Handle(key, (object func) => { return ((System.Func<T, object>)func)?.Invoke(t); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2) => _Handle(key, (object func) => { return ((System.Func<T1, T2, object>)func)?.Invoke(t1, t2); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, object>)func)?.Invoke(t1, t2, t3); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, object>)func)?.Invoke(t1, t2, t3, t4); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, object>)func)?.Invoke(t1, t2, t3, t4, t5); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object> func) => _Deregister(key, func);
    }

    public class EventSystem<K, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : EventSystemBase<K> {
        /// <summary>
        /// Handles the occurence of an event by its Key
        /// </summary>
        /// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>
        public object[] Handle(K key, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15, T16 t16) => _Handle(key, (object func) => { return ((System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, object>)func)?.Invoke(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16); });
        /// <summary>
        /// Registers a Func to be invoked when an Event occurs
        /// </summary>
        public void Register(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, object> func) => _Register(key, func);
        /// <summary>
        /// Registers a Func from the EventSystem
        /// </summary>
        public void Deregister(K key, System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, object> func) => _Deregister(key, func);
    }









    //Code to write the above statements
    //DOES NOT DO THE 0 ARGS VERSION, do that manually (Just copy the one arg version and delete T)
    /*
	System.Func<string[], string> commenter = (string[] lines) => {
		string ret = "";
		
		ret +="/// <summary>\n";
        foreach (var line in lines) {
			ret +="/// " + line + "\n";
		}
        ret +="/// </summary>\n";
		
		return ret;
	};
	System.Func<int, string> formatter = (int length) => {
				string genericBlock = "";
				string argsBlock = "";
				string valsBlock = "";

				for (int i = 1; i <= length; i++) {
								string p = ((length == 1) ? "" : ""+i);
								genericBlock += "T" + p;
								argsBlock += "T" + p + " t" + p;
								valsBlock += "t" + p;
								if (i != length) {
												genericBlock += ", ";
												argsBlock += ", ";
												valsBlock += ", ";
								}
				}

				return "public class EventSystem<K, "+ genericBlock +"> : EventSystemBase<K> {\n" +
				commenter(new string[] {"Handles the occurence of an event by its Key"}) +
				"/// <returns>Set of objects returned by all invoked methods (one per invoke). 0 length means no listeners/invokes.</returns>\n" +
				"\tpublic object[] Handle(K key, " + argsBlock + ") => _Handle(key, (object func) => { return ((System.Func<" + genericBlock + ", object>)func)?.Invoke("+ valsBlock + "); });\n" +
				commenter(new string[] {"Registers a Func to be invoked when an Event occurs"}) +
				"\tpublic void Register(K key, System.Func<" + genericBlock + ", object> func) => _Register(key, func);\n" +
				commenter(new string[] {"Registers a Func from the EventSystem"}) +
				"\tpublic void Deregister(K key, System.Func<" + genericBlock + ", object> func) => _Deregister(key, func);\n" +
				"}\n";
};
for (int i = 1; i <= 16; i++) {
				System.Console.WriteLine(formatter(i));
}*/

				 
}