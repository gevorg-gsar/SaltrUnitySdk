using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace saltr_unity_sdk
{

	public class SLTMatchingLevelParser : SLTLevel {
		private static SLTMatchingLevelParser INSTANCE = null;

		private SLTMatchingLevelParser(){}

		public static SLTMatchingLevelParser getInstance()
		{
			if(INSTANCE == null)
			{
				INSTANCE = new SLTMatchingLevelParser();
			}
			return INSTANCE;
		}

		
		private static void initializeCells(SLTCells cells, object boardNode)
		{
			Dictionary<string, object> boardNodeDict = boardNode.toDictionaryOrNull();
			
			IEnumerable<object> blockedCells = boardNodeDict.ContainsKey("blockedCells") ? (IEnumerable<object>)boardNodeDict["blockedCells"] : new List<object>();
			IEnumerable<object> cellProperties = boardNodeDict.ContainsKey("properties") && boardNodeDict["properties"].toDictionaryOrNull().ContainsKey("cell") ? (IEnumerable<object>)boardNodeDict["properties"].toDictionaryOrNull()["cell"] : new List<object>();
			int cols = cells.width;
			int rows = cells.height;
			
			for (int i = 0; i < cols; i++)
			{
				for (int j = 0; j < rows; j++)
				{
					SLTCell cell = new SLTCell(i, j);
					cells.insert(i, j, cell);
				}
			}
			
			//assigning cell properties
			for (int p = 0; p < cellProperties.Count(); p++)
			{
				object property = cellProperties.ElementAt(p);
				IEnumerable<object> coords = (IEnumerable<object>)property.toDictionaryOrNull()["coords"];
				SLTCell cell2 = cells.retrieve(coords.ElementAt(0).toIntegerOrZero(), coords.ElementAt(1).toIntegerOrZero());
				if (cell2 != null)
					cell2.properties = property.toDictionaryOrNull()["value"];
			}
			
			
			//blocking cells
			for (int b = 0; b < blockedCells.Count(); b++)
			{
				IEnumerable<object> blokedCell = (IEnumerable<object>)blockedCells.ElementAt(b);
				var cell3 = cells.retrieve(blockedCells.ElementAt(0).toIntegerOrZero(), blockedCells.ElementAt(1).toIntegerOrZero());
				if (cell3 != null)
				{
					cell3.isBlocked = true;
				}
			}
		}

		
		private static void parseLayerChunks(SLTMatchingBoardLayer layer, IEnumerable<object> chunkNodes, SLTCells cells, Dictionary<string,object> assetMap)
		{
			for (int i = 0; i < chunkNodes.Count(); i++)
			{
				Dictionary<string,object> chunkNode = chunkNodes.ElementAt(i).toDictionaryOrNull();
				IEnumerable<object> cellNodes = new List<object>();
				if (chunkNode != null && chunkNode.ContainsKey("cells"))
					cellNodes = (IEnumerable<object>)chunkNode["cells"];
				
				List<SLTCell> chunkCells = new List<SLTCell>();
				foreach (var cellNode in cellNodes)
				{
					int width = 0;
					int height = 0;
					
					width = ((IEnumerable<object>)cellNode).ElementAt(0).toIntegerOrZero();
					height = ((IEnumerable<object>)cellNode).ElementAt(1).toIntegerOrZero();
					
					chunkCells.Add(cells.retrieve(width, height) as SLTCell);
				}
				
				IEnumerable<object> assetNodes = (IEnumerable<object>)chunkNode["assets"];
				List<SLTChunkAssetRule> chunkAssetRules = new List<SLTChunkAssetRule>();
				foreach (var assetNode in assetNodes)
				{
					string assetId = "";
					string distribytionType = "";
					int distributionVale = 0;
					IEnumerable<object> states = new IEnumerable<object>();
					
					if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("assetId"))
						assetId = assetNode.toDictionaryOrNull()["assetId"].ToString();
					
					if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("distributionType"))
						distribytionType = assetNode.toDictionaryOrNull()["distributionType"].ToString();
					
					if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("distributionValue"))
						distributionVale = assetNode.toDictionaryOrNull()["distributionValue"].toIntegerOrZero();
					
					if (assetNode.toDictionaryOrNull() != null && assetNode.toDictionaryOrNull().ContainsKey("states"))
						states = (IEnumerable<object>)assetNode.toDictionaryOrNull()["states"];
					
					chunkAssetRules.Add(new SLTChunkAssetRule(assetId, distribytionType, distributionVale, states));
				}
				layer.addChunk(new SLTChunk(layer, chunkCells, chunkAssetRules, assetMap));
			}
		}

	}

}
