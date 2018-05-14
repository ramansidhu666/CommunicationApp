using System;
using System.Collections.Generic;
using GoogleMaps.LocationServices;
using GoogleMapsApi;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Data;
using CommunicationApp.Core.Infrastructure;

namespace CommunicationApp.Core.UtilityManager
{
    public sealed class GoogleDistance
    {
        public static double Calc(double Lat1,
        double Long1, double Lat2, double Long2, int KmOrMilesOrMeters = (int) EnumValue.GoogleDistanceType.Km)
        {
            /*
                The Haversine formula according to Dr. Math.
                http://mathforum.org/library/drmath/view/51879.html
                
                dlon = lon2 - lon1
                dlat = lat2 - lat1
                a = (sin(dlat/2))^2 + cos(lat1) * cos(lat2) * (sin(dlon/2))^2
                c = 2 * atan2(sqrt(a), sqrt(1-a)) 
                d = R * c
                
                Where
                    * dlon is the change in longitude
                    * dlat is the change in latitude
                    * c is the great circle distance in Radians.
                    * R is the radius of a spherical Earth.
                    * The locations of the two points in 
                        spherical coordinates (longitude and 
                        latitude) are lon1,lat1 and lon2, lat2.
            */
            double dDistance = Double.MinValue;
            double dLat1InRad = Lat1 * (Math.PI / 180.0);
            double dLong1InRad = Long1 * (Math.PI / 180.0);
            double dLat2InRad = Lat2 * (Math.PI / 180.0);
            double dLong2InRad = Long2 * (Math.PI / 180.0);

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Asin(Math.Sqrt(a));

            // Distance.
            const Double kEarthRadiusMiles = 3956.0;
            const Double kEarthRadiusKms = 6376.5;
            if (KmOrMilesOrMeters == (int)EnumValue.GoogleDistanceType.Miles)
            {
                dDistance = kEarthRadiusMiles * c;
            }
            else if (KmOrMilesOrMeters == (int)EnumValue.GoogleDistanceType.Km)
            {
                dDistance = kEarthRadiusKms * c;
            }
            else if (KmOrMilesOrMeters == (int)EnumValue.GoogleDistanceType.Meters)
            {
                dDistance = kEarthRadiusKms * c * 1000;
            }
            return dDistance;
        }
        public static int GetDistance(string origin, string destination)
        {
            System.Threading.Thread.Sleep(1000);
            int distance = 0;
            //string from = origin.Text;
            //string to = destination.Text;
            string requesturl = "http://maps.googleapis.com/maps/api/directions/json?origin=" + origin + "&destination=" + destination + "&sensor=false";
            string content = FileGetContents(requesturl);
            JObject json = JObject.Parse(content);
            try
            {
                distance = (int)json.SelectToken("routes[0].legs[0].distance.value");
                return distance;
            }
            catch
            {
                return distance;
            }
        }

        public static string FileGetContents(string fileName)
        {
            string sContents = string.Empty;
            string me = string.Empty;
            try
            {
                if (fileName.ToLower().IndexOf("http:") > -1)
                {
                    System.Net.WebClient wc = new System.Net.WebClient();
                    byte[] response = wc.DownloadData(fileName);
                    sContents = System.Text.Encoding.ASCII.GetString(response);

                }
                else
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(fileName);
                    sContents = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch { sContents = "unable to connect to server "; }
            return sContents;
        }
    }

    public sealed class GoogleOperation
    {
        public static List<double> GetLatLong(string Address)
        {
            List<double> _list = new List<double>();
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(Address);

            //Add Latitude
            _list.Add(point.Latitude);
            //Add Longitude
            _list.Add(point.Longitude);
            //var latitude = point.Latitude;
            //var longitude = point.Longitude;
            return _list;
        }
        public static string GetAddressFromLatLong(double Latitute, double Longitute)
        {
            string Address = "";
            try
            {
                string url = "http://maps.google.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
                url = string.Format(url, Latitute, Longitute);
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        DataSet dsResult = new DataSet();
                        dsResult.ReadXml(reader);
                        Address = dsResult.Tables["result"].Rows[0]["formatted_address"].ToString();
                    }
                }
            }
            catch (Exception)
            {
            }
            return Address;
        }
    }
}