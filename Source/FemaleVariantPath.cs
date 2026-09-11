using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace FemaleApparelVariants;

// Resolves "<path>_Female" for the body-typed worn path of a Thin, Fat or Hulk pawn when that
// texture exists. The answer is cached per path for the session, so the texture is probed once
// instead of on every render tree rebuild.
//
// Eligibility is decided by the pawn's body type def, not by how the path is spelled. The
// vanilla naming is kept as a fallback for a mod-defined body type that follows it.
public static class FemaleVariantPath
{
	private static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

	public static bool TryGet(BodyTypeDef bodyType, string basePath, out string femalePath)
	{
		femalePath = null;
		if (basePath == null || !IsSharedBodyType(bodyType, basePath))
		{
			return false;
		}
		if (cache.TryGetValue(basePath, out femalePath))
		{
			return femalePath != null;
		}
		if (!basePath.Contains("_Female"))
		{
			string candidate = basePath + "_Female";
			if (ContentFinder<Texture2D>.Get(candidate + "_south", reportFailure: false) != null)
			{
				femalePath = candidate;
			}
		}
		cache[basePath] = femalePath;
		return femalePath != null;
	}

	// Thin, Fat and Hulk are the body types both genders share, so they are the ones a female
	// variant can exist for. Checked before the cache so the answer cannot leak from an eligible
	// body type to an ineligible one that happens to share a path.
	private static bool IsSharedBodyType(BodyTypeDef bodyType, string path)
	{
		if (bodyType == BodyTypeDefOf.Thin || bodyType == BodyTypeDefOf.Fat || bodyType == BodyTypeDefOf.Hulk)
		{
			return true;
		}
		return path.Contains("_Thin") || path.Contains("_Fat") || path.Contains("_Hulk");
	}
}
