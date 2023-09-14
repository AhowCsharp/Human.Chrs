﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Human.Chrs.Domain.CommonModels;
using Human.Chrs.Domain.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Human.Chrs.Domain.Services
{
    public class GeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly ChrsConfig _config;

        public GeocodingService(HttpClient httpClient, IOptions<ChrsConfig> configOptions)
        {
            _httpClient = httpClient;
            _config = configOptions.Value;
        }

        public async Task<(double Latitude, double Longitude)> GetCoordinates(string address)
        {
            string requestUri = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={_config.GoogleMapKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(content);

                // 獲取座標
                double latitude = rootObject.Results[0].Geometry.Location.Lat;
                double longitude = rootObject.Results[0].Geometry.Location.Lng;

                return (latitude, longitude);
            }
            else
            {
                throw new Exception("Could not geocode address.");
            }
        }

    }
}
