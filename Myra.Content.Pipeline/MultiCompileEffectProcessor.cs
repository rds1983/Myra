using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Myra.Graphics;

namespace Myra.Content.Pipeline
{
	/// <summary>
	/// Processes a string representation to a platform-specific compiled effect.
	/// </summary>
	[ContentProcessor(DisplayName = "MultiCompileEffect - MonoGame")]
	public class MultiCompileEffectProcessor : ContentProcessor<EffectContent, MultiCompileEffectContent>
	{
		private static string ReadFileWithIncludes(string filePath)
		{
			var folder = Path.GetDirectoryName(filePath);
			var regex = new Regex(@"#include\s+[""<]([^"">]+)*["">]");

			var content = File.ReadAllText(filePath);
			var matches = regex.Matches(content);

			foreach (Match m in matches)
			{
				var includeFile = m.Groups[1].Value;
				var includePath = Path.Combine(folder, includeFile);

				var includeContent = ReadFileWithIncludes(includePath);

				content = content.Replace(m.Groups[0].Value, includeContent);
			}

			return content;
		}

		private static string Collect(ContentProcessorContext context, string code, out string[][] defineSets)
		{
			// #pragma multi_compile
			var regex = new Regex(@"#pragma\s+multi_compile\s+(.*)$", RegexOptions.Multiline);
			var matches = regex.Matches(code);

			var result = new List<string[]>();

			foreach (Match m in matches)
			{
				context.Logger.LogMessage("Found #pragma multi_compile");
				// Remove #pragma
				code = code.Replace(m.Groups[0].Value, string.Empty);

				var newSet = (from d in m.Groups[1].Value.Split(' ', '\t') where !string.IsNullOrEmpty(d.Trim()) select d.Trim()).ToArray();
				if (newSet.Length > 0)
				{
					context.Logger.LogMessage("Added defines' set: {0}", string.Join(", ", newSet));
					result.Add(newSet);
				}
			}

			regex = new Regex(@"#pragma\s+shader_feature\s+(.*)$", RegexOptions.Multiline);
			matches = regex.Matches(code);

			foreach (Match m in matches)
			{
				// Remove #pragma
				code = code.Replace(m.Groups[0].Value, string.Empty);

				var featureName = m.Groups[1].Value.Trim();

				context.Logger.LogMessage("Found #pragma shader_feature");
				if (!string.IsNullOrEmpty(featureName))
				{
					var newSet = new[] { string.Empty, featureName };
					context.Logger.LogMessage("Added defines' set: {0}", string.Join(", ", newSet));
					result.Add(newSet);
				}
			}

			defineSets = result.ToArray();

			return code;

		}

		public override MultiCompileEffectContent Process(EffectContent input, ContentProcessorContext context)
		{
			var start = DateTime.Now;

			var result = new MultiCompileEffectContent();
			var effectProcessor = new EffectProcessor();

			context.Logger.LogMessage("Processing a multi compile effect");
			context.Logger.LogMessage("Resolving #includes");
			var code = ReadFileWithIncludes(input.Identity.SourceFilename);
			context.Logger.LogMessage("Processed #includes, resulting code size is {0}", code.Length);

			context.Logger.LogMessage("Collecting shader variants");
			string[][] defineSets;
			code = Collect(context, code, out defineSets);

			context.Logger.LogMessage("Collected {0} defineSets", defineSets.Length);

			if (defineSets.Length == 0)
			{
				context.Logger.LogMessage("No multi compile pragmas found, only one default variant will be compiled");
				var ec = effectProcessor.Process(input, context);
				result.DefaultVariantKey = string.Empty;
				result.AddVariant(result.DefaultVariantKey, ec.GetEffectCode());
				return result;
			}

			var variantsCount = 1;
			foreach (var d in defineSets)
			{
				variantsCount *= d.Length;
			}

			context.Logger.LogMessage("Total shader variants is {0}", variantsCount);

			var indices = new int[defineSets.Length];
			Array.Clear(indices, 0, indices.Length);

			var temporaryPath = string.Empty;
			try
			{
				// Save code to temporary file
				var codeFolder = Path.GetDirectoryName(input.Identity.SourceFilename);
				var codeFile = Path.GetFileName(input.Identity.SourceFilename);
				temporaryPath = Path.Combine(codeFolder, "temp." + codeFile);
				File.WriteAllText(temporaryPath, code);

				context.Logger.LogMessage("Starting compilation", variantsCount);
				input.Identity.SourceFilename = temporaryPath;
				input.EffectCode = code;

				var defines = new List<string>();

				for (var i = 0; i < variantsCount; ++i)
				{
					defines.Clear();
					for (var j = 0; j < indices.Length; ++j)
					{
						var index = indices[j];
						var define = defineSets[j][index];
						defines.Add(define);
					}

					// Build defines string
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

					// Update indices
					for (var j = 0; j < indices.Length; ++j)
					{
						++indices[j];
						if (indices[j] < defineSets[j].Length)
						{
							break;
						}

						// Reset this index as higher index will be raised
						indices[j] = 0;
					}
				}

				context.Logger.LogMessage("Multi compile effect processing done succesfully.");
			}
			finally
			{
				if (!string.IsNullOrEmpty(temporaryPath) && File.Exists(temporaryPath))
				{
					File.Delete(temporaryPath);
				}
			}

			var span = (DateTime.Now - start).TotalSeconds;

			context.Logger.LogMessage("{0} seconds spent", span);

			return result;
		}
	}
}
