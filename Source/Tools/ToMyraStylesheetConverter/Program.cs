using Microsoft.Xna.Framework;
using MonoGdx.Utils;
using Myra.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Myra.Tools.ToMyraStylesheetConverter
{
	static class Program
	{
		private class DrawableInfo
		{
			public string Name { get; set; }
			public Color Color { get; set; }
		}

		private class StyleInfo
		{
			public Dictionary<string, string> PropertyConversion { get; set; }
			public Dictionary<string, StyleInfo> SpecialVariants { get; set; }
			public bool HorizontalAndVertical { get; set; }
		}

		private static readonly string[] IgnoreVariants = new[]
		{
			"default",
			"default-horizontal",
			"default-vertical",
			"radio",
			"toggle"
		};

		private static readonly Dictionary<string, StyleInfo> _styles = new Dictionary<string, StyleInfo>
		{
			{
				"com.badlogic.gdx.scenes.scene2d.ui.CheckBox$CheckBoxStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "checkboxOn", "checkBox/image/pressedImage" },
						{ "checkboxOff", "checkBox/image/image" },
						{ "font", "checkBox/label/font" },
						{ "fontColor", "checkBox/label/textColor" },
						{ "disabledFontColor", "checkBox/label/disabledTextColor" },
					}
				}
			},
			{
				"com.kotcrab.vis.ui.widget.VisCheckBox$VisCheckBoxStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "checkBackground", "checkBox/image/background" },
						{ "checkBackgroundOver", "checkBox/image/overBackground" },
						{ "tick", "checkBox/image/pressedImage" },
						{ "font", "checkBox/label/font" },
						{ "fontColor", "checkBox/label/textColor" },
						{ "disabledFontColor", "checkBox/label/disabledTextColor" },
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.ImageButton$ImageButtonStyle|com.kotcrab.vis.ui.widget.VisImageButton$VisImageButtonStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "up", "imageButton/background" },
						{ "down", "imageButton/pressedBackground" },
						{ "over", "imageButton/overBackground" },
						{ "imageUp", "imageButton/image/image" },
					},
					SpecialVariants = new Dictionary<string, StyleInfo>
					{
						{
							"close", new StyleInfo
							{
								PropertyConversion = new Dictionary<string, string>
								{
									{ "up", "window/closeButton/background" },
									{ "down", "window/closeButton/pressedBackground" },
									{ "over", "window/closeButton/overBackground" },
									{ "imageUp", "window/closeButton/image/image" },
								},
							}
						}
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.ImageTextButton$ImageTextButtonStyle|com.kotcrab.vis.ui.widget.VisImageTextButton$VisImageTextButtonStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "up", "button/background" },
						{ "down", "button/pressedBackground" },
						{ "font", "button/label/font" },
						{ "fontColor", "button/label/textColor" },
						{ "disabledFontColor", "button/label/disabledTextColor" },
						{ "downFontColor", "button/label/downTextColor" },
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.Label$LabelStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "font", "textBlock/font" },
						{ "fontColor", "textBlock/textColor" },
						{ "disabledFontColor", "textBlock/disabledTextColor" },
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.List$ListStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "font", "listBox/listBoxItem/label/font" },
						{ "fontColor", "listBox/listBoxItem/label/textColor" },
						{ "disabledFontColor", "listBox/listBoxItem/label/disabledTextColor" },
						{ "selection", "listBox/listBoxItem/pressedBackground" }
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.ProgressBar$ProgressBarStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "ProgressBar/background" },
						{ "knobBefore", "ProgressBar/filled" }
					},
					HorizontalAndVertical = true
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.ScrollPane$ScrollPaneStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "scrollArea/background" },
						{ "hScroll", "scrollArea/horizontalScroll" },
						{ "hScrollKnob", "scrollArea/horizontalScrollKnob" },
						{ "vScroll", "scrollArea/verticalScroll" },
						{ "vScrollKnob", "scrollArea/verticalScrollKnob" }
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.SelectBox$SelectBoxStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "comboBox/background" },
						{ "font", "comboBox/label/font" },
						{ "fontColor", "comboBox/label/textColor" },
						{ "disabledFontColor", "comboBox/label/disabledTextColor" },
						{ "listStyle/background", "comboBox/itemsContainer/background" },
						{ "listStyle/font", "comboBox/comboBoxItem/label/font" },
						{ "listStyle/fontColor", "comboBox/comboBoxItem/label/textColor" },
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.Slider$SliderStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "Slider/background" },
						{ "knob", "Slider/knob/image/image" },
						{ "knobOver", "Slider/knob/image/overImage" },
						{ "knobDown", "Slider/knob/image/pressedImage" },
						{ "disabledKnob", "Slider/knob/image/disabledImage" },
					},
					HorizontalAndVertical = true
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.SplitPane$SplitPaneStyle|com.kotcrab.vis.ui.widget.VisSplitPane$VisSplitPaneStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "handle", "SplitPane/handle/background" },
						{ "handleOver", "SplitPane/handle/overBackground" },
					},
					HorizontalAndVertical = true
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.TextButton$TextButtonStyle|com.kotcrab.vis.ui.widget.VisTextButton$VisTextButtonStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "up", "textButton/background" },
						{ "down", "textButton/pressedBackground" },
						{ "over", "textButton/overBackground" },
						{ "font", "textButton/label/font" },
						{ "fontColor", "textButton/label/textColor" },
						{ "downFontColor", "textButton/label/downTextColor" },
						{ "disabledFontColor", "textButton/label/disabledTextColor" },
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.TextField$TextFieldStyle|com.kotcrab.vis.ui.widget.VisTextField$VisTextFieldStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "textField/background" },
						{ "font", "textField/font" },
						{ "fontColor", "textField/textColor" },
						{ "disabledFontColor", "textField/disabledTextColor" },
						{ "cursor", "textField/cursor" },
						{ "selection", "textField/selection" },
						{ "focusedFontColor", "textField/focusedTextColor" },
						{ "focusedBackground", "textField/focusedBackground" },
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.Tree$TreeStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "plus", "tree/mark/image/image" },
						{ "minus", "tree/mark/image/pressedImage" },
						{ "selection", "tree/selectionBackground" },
						{ "over", "tree/selectionHoverBackground" },
					}
				}
			},
			{
				"com.badlogic.gdx.scenes.scene2d.ui.Window$WindowStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "window/background" },
						{ "titleFont", "window/title/font" },
						{ "titleFontColor", "window/title/textColor" },
						{ "over", "window/selectionHoverBackground" },
					}
				}
			},
			{
				"com.kotcrab.vis.ui.widget.Menu$MenuStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "Menu/background" },
						{ "titleFont", "Menu/title/font" },
						{ "titleFontColor", "Menu/title/textColor" },
						{ "over", "Menu/selectionHoverBackground" },
					},
					HorizontalAndVertical = true
				}
			},
			{
				"com.kotcrab.vis.ui.widget.spinner.Spinner$SpinnerStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "spinButton/background" },
						{ "up", "spinButton/upButton/image/image" },
						{ "down", "spinButton/downButton/image/image" },
					}
				}
			},
			{
				"com.kotcrab.vis.ui.widget.Separator$SeparatorStyle/menu",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "background", "horizontalMenu/separator/image" },
						{ "thickness", "horizontalMenu/separator/thickness" },
					}
				}
			},
			{
				"com.kotcrab.vis.ui.widget.MenuItem$MenuItemStyle",
				new StyleInfo
				{
					PropertyConversion = new Dictionary<string, string>
					{
						{ "up", "Menu/menuItem/background" },
						{ "down", "Menu/menuItem/pressedBackground" },
						{ "over", "Menu/menuItem/overBackground" },
						{ "font", "Menu/menuItem/label/font" },
						{ "fontColor", "Menu/menuItem/label/textColor" },
						{ "disabledFontColor", "Menu/menuItem/label/disabledTextColor" },
						{ "downFontColor", "Menu/menuItem/label/downTextColor" },
					},
					HorizontalAndVertical = true
				}
			},
		};

		private static bool TryGetValue(this Dictionary<string, object> dict, string str, out float f)
		{
			object obj;

			f = 0;
			if (dict.TryGetValue(str, out obj))
			{
				f = (float)obj;
				return true;
			}

			return false;
		}

		private static Color FromInputColor(object data)
		{
			var inputData = (Dictionary<string, object>)data;

			float r = 0, g = 0, b = 0, a = 0;
			inputData.TryGetValue("r", out r);
			inputData.TryGetValue("g", out g);
			inputData.TryGetValue("b", out b);
			inputData.TryGetValue("a", out a);

			return new Color(r, g, b, a);
		}

		private static void ParseInto(JObject outputObj, string variantName, StyleInfo info, Dictionary<string, object> inputProperties, string itemName, string inputPrefix = "", string outputPrefix = "")
		{
			foreach (var pair in inputProperties)
			{
				var asDict = pair.Value as Dictionary<string, object>;
				if (asDict != null)
				{
					var p = inputPrefix;
					p += pair.Key + "/";

					ParseInto(outputObj, variantName, info, asDict, itemName, p);
				}
				else
				{
					string propName;
					if (info.PropertyConversion.TryGetValue(inputPrefix + pair.Key, out propName))
					{
						propName = outputPrefix + propName;

						if (!string.IsNullOrEmpty(variantName))
						{
							var idx = propName.IndexOf("/");

							if (idx != -1)
							{
								propName = propName.Substring(0, idx) + "/variants/" + variantName + propName.Substring(idx);
							}
						}

						var parts = propName.Split('/').ToList();

						var outputObj2 = outputObj;
						while (parts.Count > 1)
						{
							propName = parts[0];
							if (outputObj2[propName] == null)
							{
								outputObj2[propName] = new JObject();
							}

							outputObj2 = (JObject)outputObj2[propName];
							parts.RemoveAt(0);
						}

						outputObj2[parts[0]] = pair.Value.ToString();
					}
					else
					{
						Console.WriteLine("WARNING: Property '{0}' of item '{1}' wasn't processed.", inputPrefix + pair.Key, itemName);
					}
				}
			}
		}

		static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("ToMyraStylesheetConverter converts LibGDX UI stylesheet to Myra UI stylesheet.");
				Console.WriteLine("Usage: ToMyraStylesheetConverter <input.json> <output.json>");

				return;
			}

			try
			{
				var inputFile = args[0];
				var outputFile = args[1];

				var inputText = File.ReadAllText(inputFile);

				Dictionary<string, object> input;
				using (var reader = new StringReader(inputText))
				{
					input = (Dictionary<string, object>)Json.Deserialize(reader);
				}

				object obj;
				Dictionary<string, Color> colors = new Dictionary<string, Color>();
				if (input.TryGetValue("com.badlogic.gdx.graphics.Color", out obj))
				{
					var inputColors = (Dictionary<string, object>)obj;
					foreach (var pair in inputColors)
					{
						colors[pair.Key] = FromInputColor(pair.Value);
					}
				}

				Dictionary<string, Tuple<string, Color>> drawables = new Dictionary<string, Tuple<string, Color>>();
				if (input.TryGetValue("com.badlogic.gdx.scenes.scene2d.ui.Skin$TintedDrawable", out obj))
				{
					var inputDrawables = (Dictionary<string, object>)obj;
					foreach (var pair in inputDrawables)
					{
						var inputData = (Dictionary<string, object>)pair.Value;

						var name = (string)inputData["name"];

						obj = inputData["color"];
						Color color;
						if (obj is string)
						{
							color = colors[(string)obj];
						} else
						{
							color = FromInputColor(obj);
						}


						drawables[pair.Key] = new Tuple<string, Color>(name, color);
					}
				}

				var output = new JObject();

				foreach (var pair in colors)
				{
					if (output["colors"] == null)
					{
						output["colors"] = new JObject();
					}

					output["colors"][pair.Key] = pair.Value.ToHexString();
				}

				foreach (var pair in drawables)
				{
					if (output["drawables"] == null)
					{
						output["drawables"] = new JObject();
					}

					output["drawables"][pair.Key] = new JObject();

					output["drawables"][pair.Key]["name"] = pair.Value.Item1;
					output["drawables"][pair.Key]["color"] = pair.Value.Item2.ToHexString();
				}

				foreach (var pair in _styles)
				{
					var parts = pair.Key.Split('|');

					for (var i = parts.Length - 1; i >= 0; --i)
					{
						var itemName = parts[i];

						if (!input.TryGetValue(itemName, out obj))
						{
							continue;
						}

						var inputVariants = (Dictionary<string, object>)obj;

						if (inputVariants.TryGetValue("default", out obj))
						{
							var inputProps = (Dictionary<string, object>)obj;
							if (!pair.Value.HorizontalAndVertical)
							{
								ParseInto(output, string.Empty, pair.Value, inputProps, itemName, string.Empty, string.Empty);
							}
							else
							{
								ParseInto(output, string.Empty, pair.Value, inputProps, itemName, string.Empty, "horizontal");
								ParseInto(output, string.Empty, pair.Value, inputProps, itemName, string.Empty, "vertical");
							}
						}

						if (inputVariants.TryGetValue("default-horizontal", out obj))
						{
							var inputProps = (Dictionary<string, object>)obj;
							ParseInto(output, string.Empty, pair.Value, inputProps, itemName, string.Empty, "horizontal");
						}

						if (inputVariants.TryGetValue("default-vertical", out obj))
						{
							var inputProps = (Dictionary<string, object>)obj;
							ParseInto(output, string.Empty, pair.Value, inputProps, itemName, string.Empty, "vertical");
						}

						foreach (var pair2 in inputVariants)
						{
							if (IgnoreVariants.Contains(pair2.Key))
							{
								continue;
							}

							var style = pair.Value;
							var variantName = pair2.Key;
							StyleInfo specialVariant = null;
							if (pair.Value.SpecialVariants != null &&
								pair.Value.SpecialVariants.TryGetValue(pair2.Key, out specialVariant))
							{
								variantName = string.Empty;

								style = specialVariant;
							}

							var inputProps = (Dictionary<string, object>)pair2.Value;

							if (!pair.Value.HorizontalAndVertical)
							{
								ParseInto(output, variantName, style, inputProps, itemName, string.Empty, string.Empty);
							}
							else
							{
								ParseInto(output, variantName, style, inputProps, itemName, string.Empty, "horizontal");
								ParseInto(output, variantName, style, inputProps, itemName, string.Empty, "vertical");
							}
						}

						break;
					}
				}

				File.WriteAllText(outputFile, output.ToString());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}