using System.Reflection;
using System.Runtime.Loader;

namespace Geek.Server.Core.Hotfix
{
    internal class DllLoader
    {
        public DllLoader(string dllPath,string hotfixDllName)
        {
            Context = new HostAssemblyLoadContext(dllPath);
            HotfixDll = Context.LoadFromAssemblyPath(dllPath + hotfixDllName);
        }

        public Assembly HotfixDll { get; }

        private HostAssemblyLoadContext Context { get; }

        public WeakReference Unload()
        {
            Context.Unload();
            return new WeakReference(Context);
        }

        class HostAssemblyLoadContext : AssemblyLoadContext
        {
            private string _dependencyPath;

            public HostAssemblyLoadContext(string dependencyPath)
            {
                _dependencyPath = dependencyPath;
                this.Resolving += OnResolving;
            }

            private Assembly OnResolving(AssemblyLoadContext context, AssemblyName assemblyName)
            {
                // 构造依赖项的完整路径
                string dependencyDllPath = Path.Combine(_dependencyPath, assemblyName.Name + ".dll");
                if (File.Exists(dependencyDllPath))
                {
                    return LoadFromAssemblyPath(dependencyDllPath);
                }
                return null;
            }
        }

    }
}
