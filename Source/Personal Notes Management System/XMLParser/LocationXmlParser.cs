using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Personal_Notes_Management_System.XMLParser
{
    public class LocationXmlParser
    {
        public static MyLocation Parse(string xmlstring)
        {
            if (xmlstring == null || xmlstring.Length <= 0)
                return null;
            MyLocation result = new MyLocation();
            XDocument hehe = XDocument.Parse(xmlstring);
            XElement location = hehe.Root.Element("geocoordinate");
            result.GeoCoordinate = new System.Device.Location.GeoCoordinate(double.Parse(location.Attribute("lat").Value), double.Parse(location.Attribute("long").Value));
            result.Address = hehe.Root.Element("address").Value;
            return result;
        }

        public static string CreateXmlString(MyLocation location)
        {
            if (location == null)
                return "";
            XElement root = new XElement("location");
            XElement geo = new XElement("geocoordinate");
            geo.SetAttributeValue("lat", location.GeoCoordinate.Latitude);
            geo.SetAttributeValue("long", location.GeoCoordinate.Longitude);
            root.Add(geo);
            XElement address = new XElement("address");
            address.Value = location.Address;
            root.Add(address);
            return root.ToString();
        }
    }

    public class MyLocation
    {
        public GeoCoordinate GeoCoordinate { get; set; }
        public string Address { get; set; }
    }
}
