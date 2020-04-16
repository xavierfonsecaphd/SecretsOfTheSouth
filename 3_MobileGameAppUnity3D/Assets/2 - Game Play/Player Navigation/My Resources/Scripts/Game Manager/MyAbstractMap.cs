/**
Copyright (c)  2019, Francisco Xavier Dos Santos Fonseca (Ordem dos Engenheiros n.º 84598), and Technical University of Delft. 
All rights reserved. 

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. 

3. All advertising materials mentioning features or use of this software must display the following acknowledgement: 
This product includes software developed by the Technical University of Delft. 

4. Neither the name of  the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY  COPYRIGHT HOLDER "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL  COPYRIGHT HOLDER BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/



namespace Mapbox.Unity.Map
{
	using System;
	using Mapbox.Unity.Utilities;
	using Utils;
	using UnityEngine;
	using System.Collections;
	using Mapbox.Map;
	using Mapbox.Unity.Map;


	public abstract class MyAbstractMap : MonoBehaviour, IMap
	{
		//[SerializeField]
		//bool _initializeOnStart = true;

		//[Geocode]
		//[SerializeField]
		//protected string _latitudeLongitudeString;

		[SerializeField]
		[Range(0, 22)]
		protected float _zoom;
		public float Zoom
		{
			get
			{
				return _zoom;
			}
		}

		[SerializeField]
		protected Transform _root;
		public Transform Root
		{
			get
			{
				return _root;
			}
		}

		[SerializeField]
		protected AbstractTileProvider _tileProvider;

		[SerializeField]
		protected AbstractMapVisualizer _mapVisualizer;
		public AbstractMapVisualizer MapVisualizer
		{
			get
			{
				return _mapVisualizer;
			}
		}

		[SerializeField]
		protected float _unityTileSize = 100;
		public float UnityTileSize
		{
			get
			{
				return _unityTileSize;
			}
		}

		[SerializeField]
		protected bool _snapMapHeightToZero = true;

		protected bool _worldHeightFixed = false;

		protected MapboxAccess _fileSouce;

		protected Vector2d _centerLatitudeLongitude;
		public Vector2d CenterLatitudeLongitude
		{
			get
			{
				return _centerLatitudeLongitude;
			}
		}

		protected Vector2d _centerMercator;
		public Vector2d CenterMercator
		{
			get
			{
				return _centerMercator;
			}
		}

		protected float _worldRelativeScale;
		public float WorldRelativeScale
		{
			get
			{
				return _worldRelativeScale;
			}
		}

		public int InitialZoom
		{
			get
			{
				return InitialZoom;
			}
		}

		public int AbsoluteZoom
		{
			get
			{
				return AbsoluteZoom;
			}
		}

	

		public void SetCenterMercator(Vector2d centerMercator)
		{
			_centerMercator = centerMercator;
		}

		public void SetCenterLatitudeLongitude(Vector2d centerLatitudeLongitude)
		{
			//_latitudeLongitudeString = string.Format("{0}, {1}", centerLatitudeLongitude.x, centerLatitudeLongitude.y);
			_centerLatitudeLongitude = centerLatitudeLongitude;
		}

		public void SetZoom(float zoom)
		{
			_zoom = zoom;
		}

		public event Action OnInitialized = delegate { };

		void Awake()
		{
			_worldHeightFixed = false;
			_fileSouce = MapboxAccess.Instance;
			_tileProvider.OnTileAdded += TileProvider_OnTileAdded;
			_tileProvider.OnTileRemoved += TileProvider_OnTileRemoved;
			if (!_root)
			{
				_root = transform;
			}

		}

		void Start()
		{
			//if (_initializeOnStart)
			//{
				//var latLonSplit = _latitudeLongitudeString.Split(',');
				//Initialize(new Vector2d(double.Parse(latLonSplit[0]), double.Parse(latLonSplit[1])), _zoom);

			//}
		}

		// TODO: implement IDisposable, instead?
		void OnDestroy()
		{
			if (_tileProvider != null)
			{
				_tileProvider.OnTileAdded -= TileProvider_OnTileAdded;
				_tileProvider.OnTileRemoved -= TileProvider_OnTileRemoved;
			}

			_mapVisualizer.Destroy();
		}

		void TileProvider_OnTileAdded(UnwrappedTileId tileId)
		{
			if (_snapMapHeightToZero && !_worldHeightFixed)
			{
				_worldHeightFixed = true;
				var tile = _mapVisualizer.LoadTile(tileId);
				if (tile.HeightDataState == MeshGeneration.Enums.TilePropertyState.Loaded)
				{
					var h = tile.QueryHeightData(.5f, .5f);
					Root.transform.position = new Vector3(
						Root.transform.position.x,
						-h,
						Root.transform.position.z);
				}
				else
				{
					tile.OnHeightDataChanged += (s) =>
					{
						var h = s.QueryHeightData(.5f, .5f);
						Root.transform.position = new Vector3(
							Root.transform.position.x,
							-h,
							Root.transform.position.z);
					};
				}
			}
			else
			{
				_mapVisualizer.LoadTile(tileId);
			}
		}

		public void SetWorldRelativeScale (float scale)
		{
			_worldRelativeScale = scale;
		}

		void TileProvider_OnTileRemoved(UnwrappedTileId tileId)
		{
			_mapVisualizer.DisposeTile(tileId);
		}

		protected void SendInitialized()
		{
			OnInitialized();
		}

		// this is what was here before, before me wanting to provide Lat/Lon through another script
		private void Initialize(Vector2d latLon, float zoom)
		{
			_worldHeightFixed = false;
			_centerLatitudeLongitude = latLon;
			_zoom = zoom;

			var referenceTileRect = Conversions.TileBounds(TileCover.CoordinateToTileId(_centerLatitudeLongitude, _zoom));
			_centerMercator = referenceTileRect.Center;

			_worldRelativeScale = (float)(_unityTileSize / referenceTileRect.Size.x);
			_mapVisualizer.Initialize(this, _fileSouce);
			_tileProvider.Initialize(this);

			SendInitialized();
		}

		// in here, I provide with the latitude and longitude myself
		//public abstract void BuildMapInPlayersGPSLocation(int zoom);

	}
}