using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;

using Gendarme.Framework;

namespace Gendarme.Rules.Xamarin
{
	[Problem ("A Xamarin.iOS assembly was built with a pre-release version, instead of the latest stable release.")]
	[Solution ("Rebuild the assemby using Xamarin.iOS 6.2.x.")]
	public sealed class TargetsPrereleaseXamarinIOS : Rule, IAssemblyRule
	{
		public RuleResult CheckAssembly (AssemblyDefinition assembly)
		{
			// Look for the TargetPlatform attribute, which indicates
			// .NET 4.0 support. This is only available in XI 6.3+.
			// e.g. TargetFramework ("MonoTouch,Version=v4.0", DisplayName = "MonoTouch")
			// This is seen as warning MT0011.
			var result = CheckForMT0011 (assembly);
			if (result)
				Runner.Report (assembly, Severity.Critical, Confidence.High, "This assembly targets the .NET 4.0 runtime, but stable release targets the .NET 2.0 runtime (MT0011).");

			// Now we attempt to detect the use of type/members that were not available in MonoTouch 6.2.x.
			// There's no point in checking if we already know we have a violation.
			if (!result && TargetsPrereleaseXamarinIOS.CheckForMT2002 (assembly))
			{
				result = true;
				Runner.Report (assembly, Severity.Critical, Confidence.High, "This assembly was not compiled using Xamarin.iOS Stable (MT2002).");
			}

			return result ? RuleResult.Failure : RuleResult.Success;
		}

		static bool CheckForMT0011 (AssemblyDefinition assembly)
		{
			return assembly.MainModule.Runtime > TargetRuntime.Net_2_0;
		}

		/// <summary>
		/// Checks for the conditions that result in MT2002.
		/// </summary>
		/// <remarks>
		/// error MT2002: Failed to resolve "System.Boolean System.Type::op_Equality(System.Type,System.Type)" reference from "mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"
		/// </remarks>
		/// <returns><c>true</c>, if the module references the equality operator overload on System.Type, <c>false</c> otherwise.</returns>
		/// <param name="assembly">Assembly.</param>
		static bool CheckForMT2002 (AssemblyDefinition assembly)
		{
			var result = false;
			// We need to see if this module's member references table
			// has an entry to a method that we know was not implemented
			// in Stable, but was implemented in Beta or Alpha.
			var refs = assembly.MainModule.GetMemberReferences ();

			var badRef = refs.SingleOrDefault (m => m.FullName == "System.Boolean System.Type::op_Equality(System.Type,System.Type)");
			if (badRef != null)
				result = true;

			return result;
		}
	}
}