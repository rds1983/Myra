using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI;
using Myra.Events;
using Xunit;

namespace Myra.Tests
{
	[Collection("Myra Tests")]
	public class EventsTests : IDisposable
	{
		public EventsTests()
		{
		}

		public void Dispose()
		{
			MyraEnvironment.MouseInfoGetter = MyraEnvironment.DefaultMouseInfoGetter;
		}

		private (Desktop desktop, Panel panel, Button button) CreateUI()
		{
			var desktop = new Desktop();
			var panel = new Panel();
			var button = new Button();

			desktop.Root = panel;
			panel.Widgets.Add(button);

			// Set button bounds
			button.Left = 10;
			button.Top = 10;
			button.Width = 50;
			button.Height = 50;

			return (desktop, panel, button);
		}

		[Fact]
		public void EventCapturingButtonHit()
		{
			// Arrange
			MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventCapturing;
			var (desktop, panel, button) = CreateUI();

			var eventOrder = new List<string>();
			desktop.TouchDown += (w, e) => eventOrder.Add("Desktop.TouchDown");
			panel.TouchDown += (w, e) => eventOrder.Add("Panel.TouchDown");
			button.TouchDown += (w, e) => eventOrder.Add("Button.TouchDown");

			MyraEnvironment.MouseInfoGetter = () => new MouseInfo
			{
				Position = new Microsoft.Xna.Framework.Point(35, 35),
				IsLeftButtonDown = true
			};

			// Act
			desktop.Render();

			// Assert - In EventCapturing, events start from Desktop (root)
			Assert.Equal(3, eventOrder.Count);
			Assert.Equal("Desktop.TouchDown", eventOrder[0]);
			Assert.Equal("Panel.TouchDown", eventOrder[1]);
			Assert.Equal("Button.TouchDown", eventOrder[2]);
		}

		[Fact]
		public void EventBubblingButtonHit()
		{
			// Arrange
			MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventBubbling;
			var (desktop, panel, button) = CreateUI();

			var eventOrder = new List<string>();
			desktop.TouchDown += (w, e) => eventOrder.Add("Desktop.TouchDown");
			panel.TouchDown += (w, e) => eventOrder.Add("Panel.TouchDown");
			button.TouchDown += (w, e) => eventOrder.Add("Button.TouchDown");

			MyraEnvironment.MouseInfoGetter = () => new MouseInfo
			{
				Position = new Microsoft.Xna.Framework.Point(35, 35),
				IsLeftButtonDown = true
			};

			// Act
			desktop.Render();

			// Assert - In EventBubbling, events start from the deepest widget and bubble up to Desktop
			Assert.Equal(3, eventOrder.Count);
			Assert.Equal("Button.TouchDown", eventOrder[0]);
			Assert.Equal("Panel.TouchDown", eventOrder[1]);
			Assert.Equal("Desktop.TouchDown", eventOrder[2]);
		}

		[Fact]
		public void EventCapturingButtonNotHit()
		{
			// Arrange
			MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventCapturing;
			var (desktop, panel, button) = CreateUI();

			var eventOrder = new List<string>();
			desktop.TouchDown += (w, e) => eventOrder.Add("Desktop.TouchDown");
			panel.TouchDown += (w, e) => eventOrder.Add("Panel.TouchDown");
			button.TouchDown += (w, e) => eventOrder.Add("Button.TouchDown");

			MyraEnvironment.MouseInfoGetter = () => new MouseInfo
			{
				Position = new Microsoft.Xna.Framework.Point(100, 100),
				IsLeftButtonDown = true
			};

			// Act
			desktop.Render();

			// Assert - Desktop and Panel should receive the event, but not Button (since click is outside it)
			Assert.Equal(2, eventOrder.Count);
			Assert.Equal("Desktop.TouchDown", eventOrder[0]);
			Assert.Equal("Panel.TouchDown", eventOrder[1]);
		}

		[Fact]
		public void EventBubblingButtonNotHit()
		{
			// Arrange
			MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventBubbling;
			var (desktop, panel, button) = CreateUI();

			var eventOrder = new List<string>();
			desktop.TouchDown += (w, e) => eventOrder.Add("Desktop.TouchDown");
			panel.TouchDown += (w, e) => eventOrder.Add("Panel.TouchDown");
			button.TouchDown += (w, e) => eventOrder.Add("Button.TouchDown");

			MyraEnvironment.MouseInfoGetter = () => new MouseInfo
			{
				Position = new Microsoft.Xna.Framework.Point(100, 100),
				IsLeftButtonDown = true
			};

			// Act
			desktop.Render();

			// Assert - Panel and Desktop should receive the event, but not Button
			Assert.Equal(2, eventOrder.Count);
			Assert.Equal("Panel.TouchDown", eventOrder[0]);
			Assert.Equal("Desktop.TouchDown", eventOrder[1]);
		}

		[Fact]
		public void EventCapturingStopPropagation()
		{
			// Arrange
			MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventCapturing;
			var (desktop, panel, button) = CreateUI();

			var eventOrder = new List<string>();

			desktop.TouchDown += (w, e) => eventOrder.Add("Desktop.TouchDown");
			panel.TouchDown += (w, e) =>
			{
				eventOrder.Add("Panel.TouchDown");
				e.StopPropagation();
			};
			button.TouchDown += (w, e) => eventOrder.Add("Button.TouchDown");

			MyraEnvironment.MouseInfoGetter = () => new MouseInfo
			{
				Position = new Microsoft.Xna.Framework.Point(35, 35),
				IsLeftButtonDown = true
			};

			// Act
			desktop.Render();

			// Assert - In EventCapturing, Desktop gets it first, then Panel stops propagation before Button
			Assert.Equal(2, eventOrder.Count);
			Assert.Equal("Desktop.TouchDown", eventOrder[0]);
			Assert.Equal("Panel.TouchDown", eventOrder[1]);
		}

		[Fact]
		public void EventBubblingStopPropagation()
		{
			// Arrange
			MyraEnvironment.EventHandlingModel = EventHandlingStrategy.EventBubbling;
			var (desktop, panel, button) = CreateUI();

			var eventOrder = new List<string>();
			var desktopTouchDownCalled = false;

			panel.TouchDown += (w, e) =>
			{
				eventOrder.Add("Panel.TouchDown");
				e.StopPropagation();
			};
			button.TouchDown += (w, e) => eventOrder.Add("Button.TouchDown");
			desktop.TouchDown += (w, e) => desktopTouchDownCalled = true;

			MyraEnvironment.MouseInfoGetter = () => new MouseInfo
			{
				Position = new Microsoft.Xna.Framework.Point(35, 35),
				IsLeftButtonDown = true
			};

			// Act
			desktop.Render();

			// Assert - Both button and panel should receive the event, but desktop should not due to StopPropagation
			Assert.Equal(2, eventOrder.Count);
			Assert.Equal("Button.TouchDown", eventOrder[0]);
			Assert.Equal("Panel.TouchDown", eventOrder[1]);
			Assert.False(desktopTouchDownCalled, "Desktop should not receive TouchDown due to StopPropagation");
		}
	}
}
