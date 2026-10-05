using HarmonyLib;
using RimWorld;
using Verse;

namespace FemaleApparelVariants;

// Swaps the worn apparel graphic of a female Thin, Fat or Hulk pawn for the "_Female" variant.
// The getter only runs when a pawn's render tree is rebuilt, never per frame.
// This is a postfix on the getter itself, not on PawnRenderNode_Apparel.GraphicsFor, so it sees the
// graphic before any later postfix wraps it. Apparel Damage Visuals replaces the record's graphic
// with a Graphic_TornWrapper whose path is null, so a swap further downstream never matched it.
// Only a graphic still on the plain body-typed path is swapped, so anything Sized Apparel already
// replaced with a sized texture is left alone; it runs first and keys its lookup on the base path.
[HarmonyPatch(typeof(ApparelGraphicRecordGetter), nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel))]
[HarmonyPriority(Priority.High)]
[HarmonyAfter("SizedApparelforRJW")]
[HarmonyBefore("thumb.APV")]
public static class ApparelGraphicRecordGetter_TryGetGraphicApparel_Patch
{
	public static void Postfix(Apparel apparel, BodyTypeDef bodyType, bool __result, ref ApparelGraphicRecord rec)
	{
		Graphic graphic = rec.graphic;
		if (!__result || graphic == null || bodyType == null || apparel?.Wearer?.gender != Gender.Female)
		{
			return;
		}
		string basePath = apparel.WornGraphicPath + "_" + bodyType.defName;
		if (graphic.path == basePath && FemaleVariantPath.TryGet(bodyType, basePath, out string femalePath))
		{
			rec = new ApparelGraphicRecord(GraphicDatabase.Get<Graphic_Multi>(femalePath, graphic.Shader, graphic.drawSize, graphic.color, graphic.colorTwo), rec.sourceApparel);
		}
	}
}
