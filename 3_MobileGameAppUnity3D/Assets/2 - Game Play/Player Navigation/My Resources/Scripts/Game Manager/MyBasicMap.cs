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



using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Map;
using System.Collections;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity;


public class MyBasicMap : AbstractMap
{

	public static MyBasicMap instance { set; get ; }
	// This is a singleton
	private static MyBasicMap singleton;


	new void Awake()
	{
		if (singleton == null) {
			singleton = this;

			//Sets this to not be destroyed when reloading scene
			DontDestroyOnLoad (gameObject);
			instance = MyBasicMap.singleton;



			_worldHeightFixed = false;
			_fileSource = MapboxAccess.Instance;
			_tileProvider.OnTileAdded += TileProvider_OnTileAdded;
			_tileProvider.OnTileRemoved += TileProvider_OnTileRemoved;
			_tileProvider.OnTileRepositioned += TileProvider_OnTileRepositioned;
			if (!_root)
			{
				_root = transform;
			}
		}
		else if (singleton != this) 
		{
			// Then destroy this. This enforces our singleton pattern, meaning there can only ever 
			// be one instance of a GameManager.
			Destroy (gameObject);    
		}



	}
	//public int zoom;

	/*new void Start ()
	{
		StartCoroutine (CreateMap());
	}*/

	//public void LaunchMap ()
	//{
//		StartCoroutine (CreateMap());
//	}

	public IEnumerator CreateMap()
	{
		MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ("Seting up map...");
		Debug.Log ("[MyBasicMap] Creating Map.");
		// 52.0022964477539  -  4.36880683898926
		/*#if UNITY_EDITOR
		Vector2d latLon = new Vector2d (52.0022964477539, 4.36880683898926); 
		#endif*/
	//	#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)

		while (!GPSLocationProvider_Xavier.instance.gotGPSLocation)
		{
			yield return new WaitForSeconds (1);
		}
		Debug.Log ("[MyBasicMap] got location.");
		MasterManager.instance._referenceDisplayManager.DisplaySystemMessage ( "Setting up map..." );
		// GPSLocationProvider_Xavier
//		GPSLocationProvider_Xavier gpsProvider = (MasterManager.instance._referenceGPSLocationProvider_Xavier.GetComponent<GPSLocationProvider_Xavier> ();

		Vector2d latLon = new Vector2d(GPSLocationProvider_Xavier.instance.latlong.x, GPSLocationProvider_Xavier.instance.latlong.y);
	//	#endif


		Initialize (latLon, (int) this.Zoom);
		_initialZoom = AbsoluteZoom;

		yield return null;
	}

	public override void Initialize (Vector2d latLon, int zoom)
	{
		_worldHeightFixed = false;
		_centerLatitudeLongitude = latLon;
		_zoom = zoom;
		_initialZoom = zoom;

		var referenceTileRect = Conversions.TileBounds(TileCover.CoordinateToTileId(_centerLatitudeLongitude, AbsoluteZoom));
		_centerMercator = referenceTileRect.Center;

		_worldRelativeScale = (float)(_unityTileSize / referenceTileRect.Size.x);
		_mapVisualizer.Initialize(this, _fileSource);
		_tileProvider.Initialize(this);

		SendInitialized();

	}

}