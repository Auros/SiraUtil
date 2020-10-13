using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SiraUtil.Tools
{
	public class CachedSpriteMediaAsyncLoader
	{
		private readonly CachedMediaAsyncLoader _cachedMediaAsyncLoader;
		//sketchiest dictionary ever
		private readonly Dictionary<(string, Vector2, float, int, SpriteMeshType), Sprite> _spriteCache = new Dictionary<(string, Vector2, float, int, SpriteMeshType), Sprite>();

		public CachedSpriteMediaAsyncLoader(CachedMediaAsyncLoader mediaAsyncLoader)
		{
			_cachedMediaAsyncLoader = mediaAsyncLoader;
		}

		public async Task<Sprite> LoadSpriteAsync(string path, CancellationToken token, Vector2 pivot, float ppu = 100, uint extrude = 0, SpriteMeshType type = SpriteMeshType.FullRect)
		{
			if (!File.Exists(path))
			{
				if (Environment.GetCommandLineArgs().Contains("--siralog"))
				{
					Plugin.Log.Info("Could not find file at: " + path);
				}
				return null;
			}
			try
			{
				var tex = await _cachedMediaAsyncLoader.LoadImageAsync(path, token);
				if (_spriteCache.TryGetValue(((string, Vector2, float, int, SpriteMeshType))(path, pivot, ppu, extrude, type), out Sprite sprite))
				{
					return sprite;
				}
				var spr = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), pivot, ppu, extrude, type);
				_spriteCache.Add(((string, Vector2, float, int, SpriteMeshType))(path, pivot, ppu, extrude, type), spr);

				return spr;
			}
			catch
			{
				Plugin.Log.Error("Could not load sprite at: " + path);
			}
			return null;
		}
	}
}
