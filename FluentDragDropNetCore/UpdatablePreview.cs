﻿using FluentDragDrop;
using System;
using System.Drawing;
using System.Threading;

namespace FluentDragDrop
{
	class UpdatablePreview : IPreview
	{
		public event EventHandler<Preview> Updated;

		public Bitmap _originalImage;
		private Timer _timer;
		private Preview _preview;
		private readonly Point _mouseStartPosition;

		public UpdatablePreview(Bitmap original, Point mouseStartPosition)
		{
			_originalImage = original ?? throw new ArgumentNullException(nameof(original));
			_mouseStartPosition = mouseStartPosition;

			UpdatePreview(null);
		}

		public Preview Get() => _preview;

		public void Start()
		{
			_timer = new Timer(UpdatePreview, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
		}

		public void Stop()
		{
			_timer?.Dispose();
			_timer = null;
		}

		private void UpdatePreview(object state)
		{
			var previewImage = new Bitmap(_originalImage);

			var currentMousePosition = System.Windows.Forms.Control.MousePosition;
			var distanceX = Math.Abs(currentMousePosition.X - _mouseStartPosition.X);
			var distanceY = Math.Abs(currentMousePosition.Y - _mouseStartPosition.Y);
			var distance = Math.Round(Math.Sqrt(Math.Pow(distanceX, 2) + Math.Pow(distanceY, 2)));

			using (var graphics = Graphics.FromImage(previewImage))
			{
				using (var font = new Font("Tahoma", 11))
				{
					using (var format = new StringFormat())
					{
						format.Alignment = StringAlignment.Center;
						format.LineAlignment = StringAlignment.Far;

						var bounds = new Rectangle(Point.Empty, _originalImage.Size);
						graphics.DrawString($"Distance: {distance}px", font, Brushes.White, bounds, format);
					}
				}
			}

			// at 750 distance, we want it to be transparent
			var opacity = (750 - distance) / 750;
			_preview = new Preview(previewImage, opacity);

			Updated?.Invoke(this, _preview);
		}
	}
}