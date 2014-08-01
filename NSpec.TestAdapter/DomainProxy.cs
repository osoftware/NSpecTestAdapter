using System;
using System.Reflection;

namespace NSpec.TestAdapter
{
	/// <summary>
	/// A base class for any piece of code that should run in a Sandbox.
	/// All input and output in derived classes should be [Serializable] or inherit MarshalByRefObject.
	/// </summary>
	abstract class DomainProxy : MarshalByRefObject
	{
		public DomainProxy()
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		public string Source { get; private set; }

		protected Assembly SandboxedAssembly { get; private set; }

		public void Load(string path)
		{
			this.Source = path;
			this.SandboxedAssembly = Assembly.LoadFrom(path);
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (args.Name.Contains("NSpec.TestAdapter"))
			{
				return Assembly.GetExecutingAssembly();
			}

			return null;
		}
	}
}
