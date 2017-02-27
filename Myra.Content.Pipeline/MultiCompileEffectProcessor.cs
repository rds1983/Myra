using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Myra.Graphics;
using Newtonsoft.Json.Linq;

namespace Myra.Content.Pipeline
{
	/// <summary>
	/// Processes a string representation to a platform-specific compiled effect.
	/// </summary>
	[ContentProcessor(DisplayName = "MultiCompileEffect - MonoGame")]
	public class MultiCompileEffectProcessor : ContentProcessor<EffectContent, MultiCompileEffectContent>
	{
		private class VariantSetNode
		{
			private readonly List<VariantsNode> _sets = new List<VariantsNode>();
			private int _variantsCount;

			public int VariantsCount
			{
				get { return _variantsCount; }
			}

			public bool HasValues
			{
				get { return _sets[_sets.Count - 1].HasValues; }
			}

			public void Reset()
			{
				foreach (var v in _sets)
				{
					v.Reset();
				}
			}

			public void GetCurrentVariant(List<string> result)
			{
				foreach (var v in _sets)
				{
					v.GetCurrentVariant(result);
				}
			}

			public void Move()
			{
				var first = _sets[0];
				first.Move();

				if (first.HasValues)
				{
					return;
				}

				// Reached the end of first set
				// Now we need to move down in the chain
				// Resetting everything until we find moveable
				VariantsNode lastMovable = null;

				foreach (var v in _sets)
				{
					if (v.HasValues)
					{
						lastMovable = v;
						break;
					}
				}

				if (lastMovable == null)
				{
					return;
				}

				lastMovable.Move();

				// Reset all nodes before the last moveable
				foreach (var v in _sets)
				{
					if (v == lastMovable)
					{
						break;
					}

					v.Reset();
				}
			}

			public void Parse(JArray array)
			{
				for (var i = 0; i < array.Count; ++i)
				{
					var node = new VariantsNode();
					node.Parse(array[i]);
					_sets.Add(node);
				}

				_variantsCount = 0;
				foreach (var v in _sets)
				{
					if (v.VariantsCount == 0)
					{
						continue;
					}

					if (_variantsCount == 0)
					{
						_variantsCount = v.VariantsCount;
					}
					else
					{
						_variantsCount *= v.VariantsCount;
					}
				}
			}
		}

		private class VariantsNode
		{
			private readonly Dictionary<string, VariantNode> _variants = new Dictionary<string, VariantNode>();
			private int _variantsCount;

			public int VariantsCount
			{
				get { return _variantsCount; }
			}

			public bool HasValues
			{
				get
				{
					foreach (var v in _variants)
					{
						if (v.Value.HasValues)
						{
							return true;
						}
					}

					return false;
				}
			}

			public void Reset()
			{
				foreach (var v in _variants)
				{
					v.Value.Reset();
				}
			}

			public void GetCurrentVariant(List<string> result)
			{
				var got = false;
				foreach (var v in _variants)
				{
					if (!v.Value.HasValues) continue;

					got = true;
					v.Value.GetCurrentVariant(result);
					result.Add(v.Key);
					break;
				}

				if (!got)
				{
					throw new Exception("No more variants in this node");
				}

			}

			public void Move()
			{
				foreach (var v in _variants)
				{
					if (!v.Value.HasValues) continue;

					v.Value.Move();
					break;
				}
			}

			public void Parse(JToken variants)
			{
				var asArray = variants as JArray;
				if (asArray != null)
				{
					foreach (var v in asArray)
					{
						var parts = (from p in v.ToString().Split(';') select p.Trim()).ToArray();

						foreach (var p in parts)
						{
							var subNode = new VariantNode();
							_variants[p] = subNode;
						}
					}
				}

				var asObject = variants as JObject;
				if (asObject != null)
				{
					foreach (var v in asObject)
					{
						var parts = (from p in v.Key.Split(';') select p.Trim()).ToArray();

						foreach (var p in parts)
						{
							var subNode = new VariantNode();
							subNode.Parse((JArray) v.Value);
							_variants[p] = subNode;
						}
					}
				}

				_variantsCount = 0;

				foreach (var v in _variants)
				{
					_variantsCount += v.Value.VariantsCount;
				}
			}
		}

		private class VariantNode
		{
			private int _index;
			private VariantSetNode _dependendVariants;

			public int VariantsCount
			{
				get { return _dependendVariants != null ? _dependendVariants.VariantsCount : 1; }
			}

			public bool HasValues
			{
				get { return _dependendVariants != null ? _dependendVariants.HasValues : _index == 0; }
			}

			public void Reset()
			{
				_index = 0;

				if (_dependendVariants != null)
				{
					_dependendVariants.Reset();
				}
			}

			public void GetCurrentVariant(List<string> result)
			{
				if (_index >= VariantsCount)
				{
					throw new Exception("Index out of range");
				}

				if (_dependendVariants != null)
				{
					_dependendVariants.GetCurrentVariant(result);
				}
			}

			public void Move()
			{
				if (_dependendVariants != null)
				{
					_dependendVariants.Move();
				}
				else
				{
					++_index;
				}
			}

			public void Parse(JArray array)
			{
				if (array == null || array.Count == 0)
				{
					return;
				}

				_dependendVariants = new VariantSetNode();
				_dependendVariants.Parse(array);
			}
		}

		public override MultiCompileEffectContent Process(EffectContent input, ContentProcessorContext context)
		{
			var start = DateTime.Now;
			context.Logger.LogMessage("Processing a multi compile effect");
			context.Logger.LogMessage("Reading shader variants");

			var variantsPath = input.Identity.SourceFilename + ".variants";
			if (!File.Exists(variantsPath))
			{
				throw new Exception(string.Format("MultiCompileEffectProcessor: could not find variants file {0}", variantsPath));
			}

			// Parse json
			var data = File.ReadAllText(variantsPath);
			var root = JArray.Parse(data);

			var rootNode = new VariantSetNode();
			rootNode.Parse(root);

			var result = new MultiCompileEffectContent();
			var effectProcessor = new EffectProcessor();

			if (rootNode.VariantsCount == 0)
			{
				context.Logger.LogMessage("No variants had been found, only one default variant will be compiled");
				var ec = effectProcessor.Process(input, context);
				result.DefaultVariantKey = string.Empty;
				result.AddVariant(result.DefaultVariantKey, ec.GetEffectCode());
				return result;
			}

			context.Logger.LogMessage("Total shader variants is {0}", rootNode.VariantsCount);

			var i = 0;
			var defines = new List<string>();
			while (rootNode.HasValues)
			{
				// Build defines string
				defines.Clear();
				rootNode.GetCurrentVariant(defines);

				// Remove empty
				defines.RemoveAll(string.IsNullOrEmpty);
				effectProcessor.Defines = MultiCompileEffect.BuildKey(defines.ToArray());
				context.Logger.LogMessage("Compiling variant #{0} with defines '{1}'", i, effectProcessor.Defines);
				var ec = effectProcessor.Process(input, context);

				result.AddVariant(effectProcessor.Defines, ec.GetEffectCode());

				if (i == 0)
				{
					// First variant is default
					result.DefaultVariantKey = effectProcessor.Defines;
					context.Logger.LogMessage("Default variant key is '{0}'", result.DefaultVariantKey);
				}

				rootNode.Move();
				++i;
			}

			context.Logger.LogMessage("Multi compile effect processing done succesfully.");

			var span = (DateTime.Now - start).TotalSeconds;

			context.Logger.LogMessage("{0} seconds spent", span);

			return result;
		}
	}
}