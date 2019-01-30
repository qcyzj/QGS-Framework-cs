using NLua;

namespace Share.Script.Lua
{
    public class LuaHelper
    {
        private NLua.Lua m_LuaState;


        public LuaHelper()
        {
            m_LuaState = new NLua.Lua();
        }


        // using lua script in c#
        public object GetGlobalVariable(string var)
        {
            return m_LuaState[var];
        }
       

        public object[] ExecScriptCode(string script_code)
        {
            return m_LuaState.DoString(script_code);
        }

        public object ExecScriptCode(string script_code, int ret_index = 0)
        {
            return m_LuaState.DoString(script_code)[ret_index];
        }

        public object[] ExecScriptFile(string file_name)
        {
            return m_LuaState.DoFile(file_name);
        }


        public LuaFunction GetFunction(string func_name)
        {
            return m_LuaState.GetFunction(func_name);
        }

        public object[] CallFunction(string func_name, params object[] args)
        {
            LuaFunction func = GetFunction(func_name);
            return func.Call(args);
        }

        public object CallFunctionRetFirst(string func_name, params object[] args)
        {
            LuaFunction func = GetFunction(func_name);
            return func.Call(args)[0];
        }


        // using c# objects in lua script
        public void SetGlobalVariable(string var, object value)
        {
            m_LuaState[var] = value;
        }


        public LuaFunction RegisterFunction(string func_name, object target)
        {
            return m_LuaState.RegisterFunction(func_name, target, target.GetType().GetMethod(func_name));
        }

        public LuaFunction RegisterStaticFunction<T>(string func_name) where T : class 
        {
            return m_LuaState.RegisterFunction(func_name, typeof(T).GetMethod(func_name));
        }
    }
}
