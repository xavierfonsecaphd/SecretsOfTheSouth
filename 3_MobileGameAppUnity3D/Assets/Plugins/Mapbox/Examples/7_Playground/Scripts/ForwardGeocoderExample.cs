//-----------------------------------------------------------------------
// <copyright file="ForwardGeocoderExample.cs" company="Mapbox">
//     Copyright (c) 2016 Mapbox. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Mapbox.Examples.Playground
{
	using UnityEngine;
	using UnityEngine.UI;
	using Mapbox.Json;
	using Mapbox.Utils.JsonConverters;
	using Mapbox.Geocoding;

	public class ForwardGeocoderExample : MonoBehaviour
    {
        [SerializeField]
		ForwardGeocodeUserInput _searchLocation = null;

        [SerializeField]
		Text _resultsText = null;

        void Awake()
        {
            _searchLocation.OnGeocoderResponse += SearchLocation_OnGeocoderResponse;
        }

        void OnDestroy()
        {
            if (_searchLocation != null)
            {
                _searchLocation.OnGeocoderResponse -= SearchLocation_OnGeocoderResponse;
            }
        }

        void SearchLocation_OnGeocoderResponse(ForwardGeocodeResponse response)
        {
            _resultsText.text = JsonConvert.SerializeObject(_searchLocation.Response, Formatting.Indented, JsonConverters.Converters);
        }
    }
}