
using Microsoft.Scripting.Hosting;

namespace Share.Script.Python
{
    public class PythonHelper
    {
        private ScriptEngine m_Engine;
        private ScriptScope m_Scope;


        public PythonHelper()
        {
            m_Engine = IronPython.Hosting.Python.CreateEngine();
            m_Scope = m_Engine.CreateScope();
        }


        // using python script in c#
        public dynamic CreateScriptFromFile(string path)
        {
            ScriptSource source = m_Engine.CreateScriptSourceFromFile(path);
            return source.Execute(m_Scope);
        }

        public dynamic CreateScriptFromString(string expr)
        {
            ScriptSource source = m_Engine.CreateScriptSourceFromString(expr);
            return source.Execute(m_Scope);
        }


        public dynamic GetVariable(string name)
        {
            return m_Scope.GetVariable(name);
        }

        public T GetFunction<T>(string name)
        {
            return m_Scope.GetVariable<T>(name);
        }

        public T GetClass<T>(string name)
        {
            return m_Scope.GetVariable<T>(name);
        }


        // using c# objects in python script
        public void SetVariable(string name, object value)
        {
            m_Scope.SetVariable(name, value);
        }
    }
}
