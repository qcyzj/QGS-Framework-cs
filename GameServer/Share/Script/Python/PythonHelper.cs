
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


        public dynamic LoadScriptFromFile(string path)
        {
            ScriptSource source = m_Engine.CreateScriptSourceFromFile(path);
            return source.Execute(m_Scope);
        }

        public dynamic LoadScriptFromString(string expr)
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
    }
}
