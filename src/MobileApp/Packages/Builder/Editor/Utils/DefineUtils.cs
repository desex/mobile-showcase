using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Build;

namespace Sotbifay.Builder.Editor
{
    public static class DefineUtils
    {
        public static HashSet<string> GetDefines()
        {
            var buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var definesString = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
            return new HashSet<string>(definesString.Split(';'));
        }

        public static void SetDefines(IEnumerable<string> defines)
        {
            var definesStringBuilder = new StringBuilder();
            definesStringBuilder.AppendJoin(';', defines);
            
            var buildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            PlayerSettings.SetScriptingDefineSymbols(buildTarget, definesStringBuilder.ToString());
        }
        
        public static void AddDefines(IEnumerable<string> defines)
        {
            if(defines is null)
                return;
            
            foreach (var define in defines) 
                AddDefine(define);
        }
        
        public static bool AddDefine(string define)
        {
            var defines = GetDefines();
            if(defines.Add(define) == false)
                return false;

            SetDefines(defines);
            return true;
        }

        public static void RemoveDefines(IEnumerable<string> defines)
        {
            if(defines is null)
                return;
            
            foreach (var define in defines) 
                RemoveDefine(define);
        }

        public static bool RemoveDefine(string define)
        {
            var defines = GetDefines();
            
            if (defines.Remove(define) == false)
                return false;
            
            SetDefines(defines);
            return true;
        }
    }
}