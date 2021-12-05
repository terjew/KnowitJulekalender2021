using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Luke2
{
    struct Pos
    {
        const double EARTH_RADIUS = 6371;
        const double D2R = Math.PI / 180;
        public string Name;
        public double Lon;
        public double Lat;

        static readonly Regex re = new Regex("(?<city>.*),Point\\((?<lon>[^ ]+) (?<lat>[^\\)]+)\\)", RegexOptions.Compiled);
        public double GetDistanceInKm(Pos other)
        {
            var dLat = other.Lat - Lat;
            var dLon = other.Lon - Lon;
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(Lat) * Math.Cos(other.Lat) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = EARTH_RADIUS * c;
            return d;
        }

        public Pos(string Name, double Lon, double Lat)
        {
            this.Name = Name;
            this.Lon = Lon;
            this.Lat = Lat;
        }

        public Pos(string line)
        {
            var m = re.Match(line);
            Name = m.Groups["city"].Value;
            Lon = double.Parse(m.Groups["lon"].Value, CultureInfo.InvariantCulture) * D2R;
            Lat = double.Parse(m.Groups["lat"].Value, CultureInfo.InvariantCulture) * D2R;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("cities.csv").Skip(1);
            var cities = lines.Select(line => new Pos(line)).ToList();

            double totalDistance = 0;
            var northPole = new Pos("North pole", 0, 90 * Math.PI / 180);
            var pos = northPole;
            while (cities.Any())
            {
                var closestCity = cities.OrderBy(c => c.GetDistanceInKm(pos)).First();
                var dist = closestCity.GetDistanceInKm(pos);
                totalDistance += dist;
                pos = closestCity;
                cities.Remove(closestCity);
            }

            totalDistance += pos.GetDistanceInKm(northPole);
            Console.WriteLine(Math.Round(totalDistance, 0));
        }

    }
}
