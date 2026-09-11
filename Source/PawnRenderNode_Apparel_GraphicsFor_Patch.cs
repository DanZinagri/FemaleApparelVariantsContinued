using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace FemaleApparelVariants;

[HarmonyPatch(typeof(PawnRenderNode_Apparel), "GraphicsFor")]
public static class PawnRenderNode_Apparel_GraphicsFor_Patch
{
	private static bool TryGetGraphicApparelWithFemaleVariant(Apparel apparel, BodyTypeDef bodyType, bool forStatue, Pawn pawn, out ApparelGraphicRecord rec)
	{
		string[] obj = new string[6] { "[FemaleApparelVariants] Processing apparel: ", null, null, null, null, null };
		int num = 1;
		string text = apparel?.def?.defName;
		obj[num] = text ?? "null";
		obj[2] = ", bodyType: ";
		obj[3] = bodyType?.defName ?? "null";
		obj[4] = ", pawn: ";
		int num2 = 5;
		string text2 = pawn?.Name?.ToStringShort;
		obj[num2] = text2 ?? "null";
		if (bodyType == null)
		{
			bodyType = BodyTypeDefOf.Male;
		}
		if (GenText.NullOrEmpty(apparel.WornGraphicPath))
		{
			_ = apparel?.def?.defName;
			rec = new ApparelGraphicRecord(null, null);
			return false;
		}
		string text3 = ((apparel.def.apparel.LastLayer != ApparelLayerDefOf.Overhead && apparel.def.apparel.LastLayer != ApparelLayerDefOf.EyeCover && !apparel.RenderAsPack() && !(apparel.WornGraphicPath == BaseContent.PlaceholderImagePath) && !(apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath)) ? (apparel.WornGraphicPath + "_" + bodyType.defName) : apparel.WornGraphicPath);
		Shader shader = ShaderDatabase.Cutout;
		if (!forStatue)
		{
			if (apparel.StyleDef?.graphicData.shaderType != null)
			{
				shader = apparel.StyleDef.graphicData.shaderType.Shader;
			}
			else if ((apparel.StyleDef == null && apparel.def.apparel.useWornGraphicMask) || (apparel.StyleDef != null && apparel.StyleDef.UseWornGraphicMask))
			{
				shader = ShaderDatabase.CutoutComplex;
			}
		}
		Graphic graphic = null;
		bool num3 = pawn.gender != Gender.Male && text3 != null && !text3.Contains("_Female") && (text3.Contains("_Thin") || text3.Contains("_Fat") || text3.Contains("_Hulk"));
		if (text3 != null)
		{
			if (text3.Contains("_Thin") || text3.Contains("_Fat"))
			{
				_ = 1;
			}
			else
				text3.Contains("_Hulk");
		}
		else
			_ = 0;
		if (num3 && ContentFinder<Texture2D>.Get(text3 + "_Female_south", reportFailure: false) != null)
		{
			string text4 = text3 + "_Female";
			try
			{
				graphic = GraphicDatabase.Get<Graphic_Multi>(text4, shader, apparel.def.graphicData.drawSize, apparel.DrawColor);
				if (graphic != null && !graphic.path.Contains("EmptyImage"))
				{
					text3 = text4;
				}
				else
				{
					graphic = null;
				}
			}
			catch (Exception)
			{
				graphic = null;
			}
		}
		if (graphic == null)
		{
			bool num4 = ContentFinder<Texture2D>.Get(text3 + "_south", reportFailure: false) != null;
			string text5 = text3;
			if (!num4 && text3 != apparel.WornGraphicPath)
			{
				text5 = apparel.WornGraphicPath;
			}
			try
			{
				graphic = GraphicDatabase.Get<Graphic_Multi>(text5, shader, apparel.def.graphicData.drawSize, apparel.DrawColor);
				text3 = text5;
			}
			catch (Exception)
			{
				rec = new ApparelGraphicRecord(null, null);
				return false;
			}
		}
		rec = new ApparelGraphicRecord(graphic, apparel);
		return true;
	}

	[HarmonyPrefix]
	public static bool Prefix(PawnRenderNode_Apparel __instance, Pawn pawn, ref IEnumerable<Graphic> __result)
	{
		if (pawn != null && pawn.gender == Gender.Male)
		{
			return true;
		}
		if (__instance.apparel == null)
		{
			__result = Enumerable.Empty<Graphic>();
			return false;
		}
		__result = GetGraphicsForWithFemaleVariant(__instance, pawn);
		return false;
	}

	private static IEnumerable<Graphic> GetGraphicsForWithFemaleVariant(PawnRenderNode_Apparel instance, Pawn pawn)
	{
		if (TryGetGraphicApparelWithFemaleVariant(instance.apparel, pawn.story.bodyType, pawn.Drawer.renderer.StatueColor.HasValue, pawn, out var rec))
		{
			yield return rec.graphic;
		}
	}
}
