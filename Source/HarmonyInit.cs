using HarmonyLib;
using Verse;

namespace FemaleApparelVariants;

[StaticConstructorOnStartup]
public static class HarmonyInit
{
	static HarmonyInit()
	{
		new Harmony("FemaleBodyVariants").PatchAll();
	}
}
