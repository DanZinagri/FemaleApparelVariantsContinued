using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace FemaleApparelVariants;

// Swaps the worn apparel graphic of a female Thin, Fat or Hulk pawn for the "_Female" variant.
// GraphicsFor only runs when the pawn's render tree is rebuilt, never per frame.
// This is a pass-through postfix, not the original prefix. Vanilla picks the base graphic through
// ApparelGraphicRecordGetter.TryGetGraphicApparel, where Vanilla Expanded Framework's unified
// apparel transpiler and Sized Apparel's sizing postfix also run. Only a graphic still on the
// plain body-typed path is swapped, so anything another mod already replaced is left alone.
[HarmonyPatch(typeof(PawnRenderNode_Apparel), "GraphicsFor")]
public static class PawnRenderNode_Apparel_GraphicsFor_Patch
{
	public static IEnumerable<Graphic> Postfix(IEnumerable<Graphic> graphics, PawnRenderNode_Apparel __instance, Pawn pawn)
	{
		Apparel apparel = __instance.apparel;
		BodyTypeDef bodyType = pawn?.story?.bodyType;
		if (apparel == null || bodyType == null || pawn.gender != Gender.Female || apparel.WornGraphicPath.NullOrEmpty())
		{
			return graphics;
		}
		return WithFemaleVariant(graphics, apparel.WornGraphicPath + "_" + bodyType.defName, bodyType);
	}

	private static IEnumerable<Graphic> WithFemaleVariant(IEnumerable<Graphic> graphics, string basePath, BodyTypeDef bodyType)
	{
		foreach (Graphic graphic in graphics)
		{
			if (graphic != null && graphic.path == basePath && FemaleVariantPath.TryGet(bodyType, basePath, out string femalePath))
			{
				yield return GraphicDatabase.Get<Graphic_Multi>(femalePath, graphic.Shader, graphic.drawSize, graphic.color);
			}
			else
			{
				yield return graphic;
			}
		}
	}
}
